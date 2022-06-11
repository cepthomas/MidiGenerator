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
        readonly MidiSender _sender;

        /// <summary>The fast timer.</summary>
        readonly MmTimerEx _mmTimer = new();
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Get settings and set up paths.
            string appDir = MiscUtils.GetAppDataDir("MidiGenerator", "Ephemera");
            _settings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));

            // Init logging.
            LogManager.MinLevelFile = Level.Debug;
            LogManager.MinLevelNotif = Level.Trace;
            LogManager.LogEvent += LogManager_LogEvent;
            LogManager.Run();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(_settings.FormGeometry.X, _settings.FormGeometry.Y);
            Size = new Size(_settings.FormGeometry.Width, _settings.FormGeometry.Height);
            //KeyPreview = true; // for routing kbd strokes through OnKeyDown

            // Configure UI.
            toolStrip1.Renderer = new NBagOfUis.CheckBoxRenderer() { SelectedColor = _settings.ControlColor };

            // The text output.
            txtViewer.Font = Font;
            txtViewer.WordWrap = true;
            txtViewer.Colors.Add("ERR", Color.LightPink);
            txtViewer.Colors.Add("WRN", Color.Plum);

            // The channel controlss.
            ccVkey.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
            ccVkey.Patch = _settings.VkeyChannel.Patch;
            ccVkey.Volume = _settings.VkeyChannel.Volume;
            ccVkey.ControlColor = _settings.ControlColor;

            ccBingBong.ChannelNumber = _settings.BingBongChannel.ChannelNumber;
            ccBingBong.Patch = _settings.BingBongChannel.Patch;
            ccBingBong.Volume = _settings.BingBongChannel.Volume;
            ccBingBong.ControlColor = _settings.ControlColor;

            // Set up midi.
            btnLogMidi.Checked = _settings.LogMidi;

            _sender = new(_settings.MidiOutDevice);
            // Init logging.
            LogMidi_Click(null, EventArgs.Empty);

            // Hook up some simple UI handlers.
            btnKillMidi.Click += (_, __) => { _sender?.KillAll(); };

            // Init patches.
            ccVkey.ChannelChangeEvent += Channel_ChannelChangeEvent;
            _sender.SendPatch(ccVkey.ChannelNumber, ccVkey.Patch);
            ccBingBong.ChannelChangeEvent += Channel_ChannelChangeEvent;
            _sender.SendPatch(ccBingBong.ChannelNumber, ccBingBong.Patch);

            vkey.ShowNoteNames = true;

            SetTimer(100);
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.LogInfo($"Hello!");

            DumpMidiDevices();

            if (!_sender.Valid)
            {
                _logger.LogError($"Something wrong with your midi output device:{_settings.MidiOutDevice}");
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
            _sender.Dispose();

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


        void BingBong_Click(object sender, EventArgs e) // TODOX
        {

        }


        #region User settings
        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);

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
            var changes = _settings.Edit("User Settings");

            // Detect changes of interest.
            //bool restart = false;

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
        void Vkey_KeyboardEvent(object? sender, VirtualKeyboard.KeyboardEventArgs e)
        {
            NoteEvent nevt;
            if (e.Velocity > 0)
            {
                nevt = new NoteOnEvent(0, ccVkey.ChannelNumber, e.NoteId, Math.Min((int)(e.Velocity * ccVkey.Volume), MidiDefs.MAX_MIDI), 0);
            }
            else // off
            {
                nevt = new NoteEvent(0, ccVkey.ChannelNumber, MidiCommandCode.NoteOff, e.NoteId, 0);
            }

            _sender.SendMidi(nevt);
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
                _sender.SendPatch(cc!.ChannelNumber, cc!.Patch);
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
                this.InvokeIfRequired(_ => { txtViewer.AppendLine($"> {e.Message}"); });
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
            _sender.LogMidi = _settings.LogMidi;
        }
        #endregion

        #region Misc functions
        /// <summary>
        /// Tell me what you have.
        /// </summary>
        void DumpMidiDevices()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                _logger.LogTrace($"Your Midi In {i} \"{MidiIn.DeviceInfo(i).ProductName}\"");
            }

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                _logger.LogTrace($"Your Midi Out {i} \"{MidiOut.DeviceInfo(i).ProductName}\"");
            }
        }
        #endregion
    }
}
