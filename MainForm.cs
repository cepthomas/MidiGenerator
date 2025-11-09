using System;
using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
// using System.Threading.Tasks;
using System.Windows.Forms;
// using System.IO;
using System.Diagnostics;
// using System.Drawing.Drawing2D;
// using System.Drawing.Imaging;
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>User settings.</summary>
        readonly UserSettings _settings;

        /// <summary>Midi output component.</summary>
        readonly IOutputDevice _outputDevice = new NullOutputDevice();

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
            // Tell the libs about their settings.
            MidiSettings.LibSettings = _settings.MidiSettings;

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
            _outputDevice.LogEnable = _settings.LogMidi;
            btnKillMidi.Click += (_, __) =>
            {
                ctrlVkey.BoundChannel.Kill();
                ctrlCc.BoundChannel.Kill();
            };

            ///// Set up midi device. /////
            foreach (var dev in _settings.MidiSettings.OutputDevices)
            {
                _outputDevice = new MidiOutput(dev.DeviceName);
                if (_outputDevice.Valid)
                {
                    break;
                }
            }
            if (!_outputDevice.Valid)
            {
                _logger.Error($"Invalid midi output device:{_outputDevice.DeviceName}");
            }

            ///// Create the channels and their corresponding controls. /////
            Channel chVkey = new()
            {
                ChannelName = $"vkey",
                ChannelNumber = _settings.VkeyChannel.ChannelNumber,
                Device = _outputDevice,
                DeviceId = _outputDevice.DeviceName,
                Volume = MidiLibDefs.VOLUME_DEFAULT,
                State = ChannelState.Normal,
                Patch = _settings.VkeyChannel.Patch,
                IsDrums = false,
                Selected = false,
            };
            ctrlVkey.BoundChannel = chVkey;
            ctrlVkey.ChannelChange += Channel_ChannelChange;
            // Good time to send initial patch.
            chVkey.SendPatch();

            Channel chCc = new()
            {
                ChannelName = $"cc",
                ChannelNumber = _settings.ClickClackChannel.ChannelNumber,
                Device = _outputDevice,
                DeviceId = _outputDevice.DeviceName,
                Volume = MidiLibDefs.VOLUME_DEFAULT,
                State = ChannelState.Normal,
                Patch = _settings.ClickClackChannel.Patch,
                IsDrums = false,
                Selected = false,
            };
            ctrlCc.BoundChannel = chCc;
            ctrlCc.ChannelChange += Channel_ChannelChange;
            // Good time to send initial patch.
            chCc.SendPatch();

            ///// Init the fancy controls. /////
            // cc.MinX = 24; // C0
            // cc.MaxX = 96; // C6
            // cc.GridX = [36, 48, 60, 72, 84];
            // cc.MinY = 0; // min velocity == note off
            // cc.MaxY = 127; // max velocity
            // cc.GridY = [32, 64, 96]; //TODO1?? make these doubles

            cc.CcClick += CcClickEvent;
            cc.CcMove += CcMoveEvent;

            vkey.Enabled = true;
            vkey.VkeyClick += VkeyClickEvent;

            ///// Finish up. /////
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
            // Fast timer for future use.
            SetTimer(100);
        }

        /// <summary>
        /// Form is legal now. Init things that want to log or play with controls..
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.Info($"Hello!");

            if (!_outputDevice.Valid)
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

            // Wait a bit in case there are some lingering events.
            System.Threading.Thread.Sleep(100);

            _outputDevice.Dispose();

            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

/*
        /// <summary>
        /// Create all I/O devices from user settings.
        /// </summary>
        /// <returns>Success</returns>
        bool CreateDevices()
        {
            bool ok = true;

            // First...
            DestroyDevices();

            // Set up input device.
            foreach (var dev in _settings.MidiSettings.InputDevices)
            {
                switch (dev.DeviceName)
                {
                    case nameof(VirtualKeyboard):
                        vkey.InputReceive += Listener_InputReceive;
                        _inputDevice = vkey;
                        break;

                    case nameof(BingBong):
                        bb.InputReceive += Listener_InputReceive;
                        _inputDevice = bb;
                        break;

                    default:
                        // Should be a real device.
                        _inputDevice = new MidiInput(dev.DeviceName);

                        if (!_inputDevice.Valid)
                        {
                            _logger.Error($"Something wrong with your input device:{dev.DeviceName}");
                            ok = false;
                        }
                        else
                        {
                            _inputDevice.CaptureEnable = true;
                            _inputDevice.InputReceive += Listener_InputReceive;
                        }
                        break;
                }
            }

            // Set up output device.
            foreach (var dev in _settings.MidiSettings.OutputDevices)
            {
                switch (dev.DeviceName)
                {
                    default:
                        // Try midi.
                        _outputDevice = new MidiOutput(dev.DeviceName);
                        if (!_outputDevice.Valid)
                        {
                            _logger.Error($"Something wrong with your output device:{_outputDevice.DeviceName}");
                            ok = false;
                        }
                        else
                        {
                            _outputDevice.LogEnable = btnLogMidi.Checked;
                        }
                        break;
                }
            }

            return ok;
        }


        /// <summary>
        /// Clean up.
        /// </summary>
        void DestroyDevices()
        {
            _inputDevice?.Dispose();
            _outputDevice?.Dispose();
        }
*/




        #region MM timer
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempo"></param>
        void SetTimer(double tempo)
        {
            MidiTimeConverter mt = new(0, tempo);
            double period = mt.RoundedInternalPeriod();
            _mmTimer.SetTimer((int)Math.Round(period), MmTimerCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalElapsed"></param>
        /// <param name="periodElapsed"></param>
        void MmTimerCallback(double totalElapsed, double periodElapsed)
        {
            try
            {
                // Could do some timed work here.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            _settings.VkeyChannel.Volume = ctrlVkey.Volume;

            _settings.ClickClackChannel.ChannelNumber = ctrlCc.ChannelNumber;
            _settings.ClickClackChannel.Patch = ctrlCc.Patch;
            _settings.ClickClackChannel.Volume = ctrlCc.Volume;

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




        //private void Send(MidiGenEventArgs e)
        //{
        //    _logger.Trace($"VirtDev N:{e.Note} V:{e.Value}");

        //    int chanNum = sender == vkey ?
        //        ctrlVkey.ChannelNumber :
        //        ctrlCc.ChannelNumber;

        //    NoteEvent nevt = e.Value > 0 ?
        //        new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
        //        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

        //    _outputDevice.SendEvent(nevt);

        //    //  .InputReceive += Device_InputReceive;
        //}




        #region Event handlers

        void VkeyClickEvent(object? sender, MidiGenEventArgs e) // TODO1 consolidate both of these
        {
            _logger.Trace($"Vkey N:{e.Note} V:{e.Value}");

            int chanNum = sender == vkey ?
                ctrlVkey.ChannelNumber :
                ctrlCc.ChannelNumber;

            NoteEvent nevt = e.Value > 0 ?
                new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
                new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

            _outputDevice.SendEvent(nevt);

            //  .InputReceive += Device_InputReceive;
        }

        ///// <summary>
        ///// Do something with events.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void Device_InputReceive(object? sender, InputReceiveEventArgs e)
        //{
        //    _logger.Trace($"VirtDev N:{e.Note} V:{e.Value}");

        //    int chanNum = sender == vkey ?
        //        ctrlVkey.ChannelNumber :
        //        ctrlCc.ChannelNumber;

        //    NoteEvent nevt = e.Value > 0 ?
        //        new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
        //        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

        //    _outputDevice.SendEvent(nevt);
        //}




        /// <summary>
        /// User clicked something. Send some midi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CcClickEvent(object? sender, MidiGenEventArgs e)
        {
            if (e.X is not null && e.Y is not null)
            {
                string name = ((ClickClack)sender!).Name;
                int x = (int)e.X; // note
                int y = (int)e.Y; // velocity

               // InjectMidiInEvent(name, 1, x, y);

                _logger.Trace($"Vkey N:{e.Note} V:{e.Value}");

                int chanNum = sender == vkey ?
                    ctrlVkey.ChannelNumber :
                    ctrlCc.ChannelNumber;

                NoteEvent nevt = e.Value > 0 ?
                    new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
                    new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

                _outputDevice.SendEvent(nevt);
            }
        }

        /// <summary>
        /// Provide tool tip text to control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CcMoveEvent(object? sender, MidiGenEventArgs e)
        {
            e.Text = $"{MusicDefinitions.NoteNumberToName((int)e.X!)} V:{e.Y}";
        }

        /// <summary>
        /// User requests a patch change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Channel_ChannelChange(object? sender, ChannelChangeEventArgs e)
        {
            var cc = sender as ChannelControl;
            if(e.PatchChange || e.ChannelNumberChange)
            {
                cc!.BoundChannel.SendPatch();
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
            _outputDevice.LogEnable = _settings.LogMidi;
        }
        #endregion
    }
}
