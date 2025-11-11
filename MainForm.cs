using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>User settings.</summary>
        readonly UserSettings _settings;

        /// <summary>Midi output device.</summary>
        MidiOut? _midiOut = null;

        /// <summary>The fast timer.</summary>
        readonly MmTimerEx _mmTimer = new();
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("MidiGenerator", "Ephemera");
            _settings = (UserSettings)SettingsCore.Load(appDir, typeof(UserSettings));

            InitializeComponent();

            // Init logging.
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogMessage += LogManager_LogMessage;
            LogManager.Run();

            ///// Configure UI. /////
            toolStrip1.Renderer = new ToolStripCheckBoxRenderer() { SelectedColor = _settings.ControlColor };
            txtViewer.Font = Font;
            txtViewer.MatchText.Add("ERR", Color.LightPink);
            txtViewer.MatchText.Add("WRN", Color.Plum);

            btnLogMidi.Checked = _settings.LogMidi;
            btnKillMidi.Click += (_, __) =>
            {
                Kill(_settings.VkeyChannel.ChannelNumber);
                Kill(_settings.ClickClackChannel.ChannelNumber);
            };

            // Figure out which midi output device. TODO1
            string deviceName = "VirtualMIDISynth #1";
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (deviceName == MidiOut.DeviceInfo(i).ProductName)
                {
                    _midiOut = new MidiOut(i);
                    break;
                }
            }

            if (_midiOut is null)
            {
                _logger.Error($"Invalid midi output device:{deviceName}");
            }

            ///// Create the channels and their corresponding controls. /////
            ctrlVkey.ControlColor = _settings.ControlColor;
            ctrlVkey.Gain = _settings.VkeyChannel.Gain;
            ctrlVkey.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
            ctrlVkey.Patch = _settings.VkeyChannel.Patch;
            ctrlVkey.ChannelChange += Channel_ChannelChange;
            SendPatch(ctrlVkey.ChannelNumber, ctrlVkey.Patch);

            ctrlCc.ControlColor = _settings.ControlColor;
            ctrlVkey.Gain = _settings.VkeyChannel.Gain;
            ctrlCc.ChannelNumber = _settings.ClickClackChannel.ChannelNumber;
            ctrlCc.Patch = _settings.ClickClackChannel.Patch;
            ctrlCc.ChannelChange += Channel_ChannelChange;
            SendPatch(ctrlCc.ChannelNumber, ctrlCc.Patch);

            cc.UserClick += UserClickEvent;
            vkey.UserClick += UserClickEvent;

            vkey.Enabled = true;

            ///// Finish up. /////
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;

            // Fast timer for future use.
            //SetTimer(100);
        }

        /// <summary>
        /// Form is legal now. Init things that want to log or play with controls..
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.Info($"Hello!");

            if (_midiOut is null)
            {
                _logger.Error($"Invalid midi output device");
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Save stuff.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogManager.Stop();
            SaveSettings();

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Resources.
            _mmTimer.Stop();
            _mmTimer.Dispose();

            _midiOut?.Dispose();

            // Wait a bit in case there are some lingering events.
            System.Threading.Thread.Sleep(100);

            _midiOut?.Dispose();

            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region User settings
        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location, Size);

            _settings.VkeyChannel.ChannelNumber = ctrlVkey.ChannelNumber;
            _settings.VkeyChannel.Patch = ctrlVkey.Patch;
            _settings.VkeyChannel.Gain = ctrlVkey.Gain;

            _settings.ClickClackChannel.ChannelNumber = ctrlCc.ChannelNumber;
            _settings.ClickClackChannel.Patch = ctrlCc.Patch;
            _settings.ClickClackChannel.Gain = ctrlCc.Gain;

            _settings.Save();
        }

        /// <summary>
        /// Edit the common options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            SettingsEditor.Edit(_settings, "User Settings", 400);

            MessageBox.Show("Restart required for changes to take effect");

            SaveSettings();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// User clicked something. Send some midi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserClickEvent(object? sender, NoteEventArgs e)
        {
            if (e.Note != -1 && e.Velocity != -1)
            {
                try
                {
                    int chanNum = sender == vkey ?
                    ctrlVkey.ChannelNumber :
                    ctrlCc.ChannelNumber;

                    _logger.Trace($"Ch:{chanNum} N:{e.Note} V:{e.Velocity}");

                    NoteEvent nevt = e.Velocity > 0 ?
                        new NoteOnEvent(0, chanNum, e.Note, e.Velocity, 0) :
                        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

                    SendEvent(nevt);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                }
            }
        }

        /// <summary>
        /// User requests a patch change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Channel_ChannelChange(object? sender, ChannelChangeEventArgs e) // TODO1
        {
            var cc = sender as ChannelControl;
            if(e.PatchChange || e.ChannelNumberChange)
            {
               // cc!.BoundChannel.SendPatch();
            }
        }

        /// <summary>
        /// Show log events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogManager_LogMessage(object? sender, LogMessageEventArgs e)
        {
            // Usually come from a different thread.
            if (IsHandleCreated)
            {
                this.InvokeIfRequired(_ => { txtViewer.AppendLine($"{e.Message}"); });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogMidi_Click(object? sender, EventArgs e)
        {
            _settings.LogMidi = btnLogMidi.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public void SendEvent(MidiEvent evt)
        {
            //if(patch >= Defs.MIN_MIDI && patch <= Defs.MAX_MIDI) TODO1 check everywhere?

            _midiOut?.Send(evt.GetAsShortMessage());
            if (_settings.LogMidi)
            {
                _logger.Trace(evt.ToString());
                //_logger.Trace($"Send {evt.GetType()} N:{e.Note} V:{e.Value}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chan"></param>
        /// <param name="note"></param>
        /// <param name="velocity"></param>
        public void SendNote(int chan, int note, int velocity)
        {

            NoteEvent evt = velocity > 0 ?
                new NoteOnEvent(0, chan, note, velocity, 0) :
                new NoteEvent(0, chan, MidiCommandCode.NoteOff, note, 0);
            SendEvent(evt);
        }

        /// <summary>
        /// Patch sender.
        /// </summary>
        public void SendPatch(int chan, int patch)
        {
            PatchChangeEvent evt = new(0, chan, patch);
            SendEvent(evt);
        }

        /// <summary>
        /// Send a controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="val"></param>
        public void SendController(int chan, MidiController controller, int val)
        {
            ControlChangeEvent evt = new(0, chan, controller, val);
            SendEvent(evt);
        }

        /// <summary>
        /// Send midi all notes off.
        /// </summary>
        public void Kill(int chan)
        {
            ControlChangeEvent evt = new(0, chan, MidiController.AllNotesOff, 0);
            SendEvent(evt);
        }
        #endregion

        /// <summary>
        /// The meaning of life.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void About_Click(object? sender, EventArgs e) //TODO
        {
            // Show the builtin definitions and user devices.
            List<string> ls = [];

            // Show them what they have.
            ls.Add($"# Your Midi Devices");
            ls.Add($"");
            ls.Add($"## Outputs");
            ls.Add($"");
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                ls.Add($"- \"{MidiOut.DeviceInfo(i).ProductName}\"");
            }

            ls.Add($"");
            ls.Add($"## Inputs");
            ls.Add($"");
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                ls.Add($"- \"{MidiIn.DeviceInfo(i).ProductName}\"");
            }

            MessageBox.Show(string.Join(Environment.NewLine, ls));
        }

        //#region MM timer TODO?
        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="tempo"></param>
        // void SetTimer(double tempo)
        // {
        //     MidiTimeConverter mt = new(0, tempo);
        //     double period = mt.RoundedInternalPeriod();
        //     _mmTimer.SetTimer((int)Math.Round(period), MmTimerCallback);
        // }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="totalElapsed"></param>
        // /// <param name="periodElapsed"></param>
        // void MmTimerCallback(double totalElapsed, double periodElapsed)
        // {
        //     try
        //     {
        //         // Could do some timed work here.
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show(ex.Message);
        //     }
        // }
        //#endregion
    }
}
