using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using NAudio.Midi;
using NBagOfTricks;
using NBagOfTricks.Slog;
using NBagOfUis;
using MidiLib;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
        readonly MidiSender _outputDevice;

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
            _settings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            MidiSettings.LibSettings = _settings.MidiSettings;

            InitializeComponent();

            // Init logging.
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogEvent += LogManager_LogEvent;
            LogManager.Run();

            // Configure UI.
            toolStrip1.Renderer = new NBagOfUis.CheckBoxRenderer() { SelectedColor = _settings.ControlColor };
            txtViewer.Font = Font;
            txtViewer.Colors.Add("ERR", Color.LightPink);
            txtViewer.Colors.Add("WRN", Color.Plum);

            // Set up midi.
            _outputDevice = new(_settings.MidiSettings.OutputDevice);

            // Create the channels and controls.
            Channel chVkey = new()
            {
                ChannelName = $"vkey",
                ChannelNumber = _settings.VkeyChannel.ChannelNumber,
                Device = _outputDevice,
                DeviceId = _outputDevice.DeviceName,
                Volume = VolumeDefs.DEFAULT,
                State = ChannelState.Normal,
                Patch = _settings.VkeyChannel.Patch,
                IsDrums = false,
                Selected = false,
            };
            ccVkey.BoundChannel = chVkey;
            ccVkey.ChannelChangeEvent += Channel_ChannelChangeEvent;
            // Good time to send initial patch.
            chVkey.SendPatch();

            // Make new channel and device.
            Channel chVBb = new()
            {
                ChannelName = $"bb",
                ChannelNumber = _settings.BingBongChannel.ChannelNumber,
                Device = _outputDevice,
                DeviceId = _outputDevice.DeviceName,
                Volume = VolumeDefs.DEFAULT,
                State = ChannelState.Normal,
                Patch = _settings.BingBongChannel.Patch,
                IsDrums = false,
                Selected = false,
            };
            ccBingBong.BoundChannel = chVBb;
            ccBingBong.ChannelChangeEvent += Channel_ChannelChangeEvent;
            // Good time to send initial patch.
            chVBb.SendPatch();

            btnLogMidi.Checked = _settings.LogMidi;
            LogMidi_Click(null, EventArgs.Empty);
            btnKillMidi.Click += (_, __) => { ccVkey.BoundChannel.Kill(); ccBingBong.BoundChannel.Kill(); };

            // Init main form from settings.
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;

            // Virtual device events.
            bb.InputEvent += Device_InputEvent;
            vkey.InputEvent += Device_InputEvent;

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















            //ccVkey.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
            //ccVkey.Patch = _settings.VkeyChannel.Patch;
            //ccVkey.Volume = _settings.VkeyChannel.Volume;
            //ccVkey.ControlColor = _settings.ControlColor;

            //ccBingBong.ChannelNumber = _settings.BingBongChannel.ChannelNumber;
            //ccBingBong.Patch = _settings.BingBongChannel.Patch;
            //ccBingBong.Volume = _settings.BingBongChannel.Volume;
            //ccBingBong.ControlColor = _settings.ControlColor;

            //// Init patches.
            //ccVkey.ChannelChangeEvent += Channel_ChannelChangeEvent;
            //_outputDevice.SendPatch(ccVkey.ChannelNumber, ccVkey.Patch);
            //ccBingBong.ChannelChangeEvent += Channel_ChannelChangeEvent;
            //_outputDevice.SendPatch(ccBingBong.ChannelNumber, ccBingBong.Patch);

            if (!_outputDevice.Valid)
            {
                _logger.Error($"Something wrong with your midi output device:{_settings.MidiSettings.OutputDevice}");
            }
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
                //TODO some work?
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

            _settings.VkeyChannel.ChannelNumber = ccVkey.ChannelNumber;
            _settings.VkeyChannel.Patch = ccVkey.Patch;
            _settings.VkeyChannel.Volume = ccVkey.Volume;

            _settings.BingBongChannel.ChannelNumber = ccBingBong.ChannelNumber;
            _settings.BingBongChannel.Patch = ccBingBong.Patch;
            _settings.BingBongChannel.Volume = ccBingBong.Volume;

            _settings.Save();
        }

        /// <summary>
        /// Edit the common options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            _settings.Edit("User Settings", 400);

            MessageBox.Show("Restart required for changes to take effect");

            SaveSettings();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Do something with events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Device_InputEvent(object? sender, InputEventArgs e)
        {
            _logger.Trace($"VirtDev N:{e.Note} V:{e.Value}");

            int chanNum = sender == vkey ?
                ccVkey.ChannelNumber :
                ccBingBong.ChannelNumber;

            NoteEvent nevt = e.Value > 0 ?
                new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
                new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

            _outputDevice.SendEvent(nevt);
        }

        /// <summary>
        /// User requests a patch change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Channel_ChannelChangeEvent(object? sender, ChannelChangeEventArgs e)
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
        void LogManager_LogEvent(object? sender, LogEventArgs e)
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
