using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("Main");

        /// <summary>User settings.</summary>
        readonly UserSettings _settings;

        /// <summary>Midi output device.</summary>
        /// <summary>Low level midi output device.</summary>
        readonly MidiOut? _midiOut = null;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            ///// Must do this first before anything else. /////
            string appDir = MiscUtils.GetAppDataDir("MidiGenerator", "Ephemera");
            _settings = (UserSettings)SettingsCore.Load(appDir, typeof(UserSettings));

            InitializeComponent();

            Icon = Properties.Resources.toro;

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
                Kill(_settings.ClClChannel.ChannelNumber);
            };

            ///// Figure out which midi output device. /////
            string deviceName = _settings.OutputDevice; // "VirtualMIDISynth #1";
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
                _logger.Error($"Invalid midi output device");
            }

            ///// Init the channels and their corresponding controls. /////

            // Channel.
            _settings.VkeyChannel.UpdatePresets();
            // Control.
            VkeyControl.UserClickNote += UserClickNoteEvent;
            VkeyControl.ControlColor = _settings.ControlColor;
            VkeyControl.Enabled = true;
            // ChannelControl.
            VkeyChannelControl.ControlColor = _settings.ControlColor;
            VkeyChannelControl.BoundChannel = _settings.VkeyChannel;
            VkeyChannelControl.ChannelChange += Channel_ChannelChange;

            // Channel.
            _settings.ClClChannel.UpdatePresets();
            // Control.
            ClClControl.UserClickNote += UserClickNoteEvent;
            ClClControl.ControlColor = _settings.ControlColor;
            ClClControl.Enabled = true;
            // ChannelControl.
            ClClChannelControl.ControlColor = _settings.ControlColor;
            ClClChannelControl.BoundChannel = _settings.ClClChannel;
            ClClChannelControl.ChannelChange += Channel_ChannelChange;

            ///// Finish up. /////
            SendPatch(VkeyChannelControl.BoundChannel.ChannelNumber, VkeyChannelControl.BoundChannel.Patch);
            SendPatch(ClClChannelControl.BoundChannel.ChannelNumber, ClClChannelControl.BoundChannel.Patch);

            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
        }

        /// <summary>
        /// Form is legal now. Tie up some loose ends.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.Info($"It's alive!");

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
            _midiOut?.Dispose();

            // Wait a bit in case there are some lingering events.
            System.Threading.Thread.Sleep(100);

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

            _settings.Save();
        }

        /// <summary>
        /// Edit the common options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(_settings, "User Settings", 300);

            // Detect changes of interest.
            bool restart = changes.Any(ch => ch.name == "ControlColor" || ch.name == "OutputDevice");

            if (restart)
            {
                MessageBox.Show("Restart required for device changes to take effect");
            }

            SaveSettings();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// User clicked something. Send some midi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserClickNoteEvent(object? sender, UserClickNoteEventArgs e)
        {
            try
            {
                int chanNum = sender == VkeyControl ? _settings.VkeyChannel.ChannelNumber : _settings.ClClChannel.ChannelNumber;

                _logger.Trace($"Ch:{chanNum} N:{e.Note} V:{e.Velocity}");

                SendNote(chanNum, e.Note, e.Velocity);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }
        }

        /// <summary>
        /// User edits the channel params.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Channel_ChannelChange(object? sender, ChannelChangeEventArgs e)
        {
            var cc = sender as ChannelControl;
            if (e.PatchChange || e.ChannelNumberChange)
            {
                SendPatch(cc.BoundChannel.ChannelNumber, cc.BoundChannel.Patch);
            }

            if (e.PresetFileChange)
            {
                // Update channel presets.
                cc.BoundChannel.UpdatePresets();
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
        #endregion

        #region Send midi
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chan"></param>
        /// <param name="note"></param>
        /// <param name="velocity"></param>
        void SendNote(int chanNum, int note, int velocity)
        {
           NoteEvent evt = velocity > 0 ?
               new NoteOnEvent(0, chanNum, note, velocity, 0) :
               new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, note, 0);
           SendEvent(evt);
        }

        /// <summary>
        /// Patch sender.
        /// </summary>
        void SendPatch(int chanNum, int patch)
        {
           PatchChangeEvent evt = new(0, chanNum, patch);
           SendEvent(evt);
        }

        /// <summary>
        /// Send a controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="val"></param>
        void SendController(int chanNum, MidiController controller, int val)
        {
           ControlChangeEvent evt = new(0, chanNum, controller, val);
           SendEvent(evt);
        }

        /// <summary>
        /// Send midi all notes off.
        /// </summary>
        void Kill(int chanNum)
        {
            ControlChangeEvent evt = new(0, chanNum, MidiController.AllNotesOff, 0);
            SendEvent(evt);
        }

        /// <summary>
        /// Send the event.
        /// </summary>
        /// <param name="evt"></param>
        void SendEvent(MidiEvent evt)
        {
            _midiOut?.Send(evt.GetAsShortMessage());
        }
        #endregion

        /// <summary>
        /// The meaning of life.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void About_Click(object? sender, EventArgs e)
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
    }
}
