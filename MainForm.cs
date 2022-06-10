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

        /// <summary>Midi component.</summary>
        readonly MidiSender? _sender1;

        /// <summary>Midi component.</summary>
        readonly MidiSender? _sender2;

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

            // The channels.
            vkeyCh.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
            vkeyCh.DeviceNumber = _settings.VkeyChannel.DeviceNumber;
            vkeyCh.Patch = _settings.VkeyChannel.Patch;
            vkeyCh.Volume = _settings.VkeyChannel.Volume;
            vkeyCh.ControlColor = _settings.ControlColor;

            // Set up midi.
            btnLogMidi.Checked = _settings.LogMidi;

            if(_settings.MidiOutDevice1 != Definitions.NONE)
            {
                _sender1 = new(_settings.MidiOutDevice1);
            }

            if (_settings.MidiOutDevice2 != Definitions.NONE)
            {
                _sender2 = new(_settings.MidiOutDevice2);
            }
            LogMidi_Click(null, EventArgs.Empty);

            // Hook up some simple UI handlers.
            btnKillMidi.Click += (_, __) => { _sender1?.KillAll(); _sender2?.KillAll(); };

            // Init patch.
            vkeyCh.PatchChange += Channel_PatchChange;
            GetSender(vkeyCh.DeviceNumber)?.SendPatch(vkeyCh.ChannelNumber, vkeyCh.Patch);

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

            if (_sender1 is not null && !_sender1.Valid)
            {
                _logger.LogError($"Something wrong with your midi output device:{_settings.MidiOutDevice1}");
            }

            if (_sender2 is not null && !_sender2.Valid)
            {
                _logger.LogError($"Something wrong with your midi output device:{_settings.MidiOutDevice2}");
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

            _sender1?.Dispose();
            _sender2?.Dispose();

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
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);

            _settings.VkeyChannel.ChannelNumber = vkeyCh.ChannelNumber;
            _settings.VkeyChannel.DeviceNumber = vkeyCh.DeviceNumber;
            _settings.VkeyChannel.Patch = vkeyCh.Patch;
            _settings.VkeyChannel.Volume = vkeyCh.Volume;

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
            var ms = GetSender(vkeyCh.DeviceNumber);
            if (ms is not null)
            {
                //_logger.LogTrace($"Vkey C:{vkeyCh.ChannelNumber} D:{vkeyCh.DeviceNumber} N:{e.NoteId} V:{e.Velocity}");

                NoteEvent nevt;

                if (e.Velocity > 0)
                {
                    nevt = new NoteOnEvent(0, vkeyCh.ChannelNumber, e.NoteId, Math.Min((int)(e.Velocity * vkeyCh.Volume), MidiDefs.MAX_MIDI), 0);
                    //evt.OffEvent is null ? 0 : evt.NoteLength); // Fix NAudio NoteLength bug.
                }
                else // off
                {
                    nevt = new NoteEvent(0, vkeyCh.ChannelNumber, MidiCommandCode.NoteOff, e.NoteId, 0);
                }

                ms.SendMidi(nevt);
            }
            else
            {
                _logger.LogWarn($"Invalid device:{vkeyCh.DeviceNumber}");
            }
        }

        /// <summary>
        /// User requests a patch change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Channel_PatchChange(object? sender, EventArgs e)
        {
            var cc = sender as ChannelControl;

            if (cc == vkeyCh)
            {
                var ms = GetSender(vkeyCh.DeviceNumber);
                if (ms is not null)
                {
                    ms.SendPatch(vkeyCh.ChannelNumber, vkeyCh.Patch);
                }
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

            if(_sender1 is not null)
            {
                _sender1.LogMidi = _settings.LogMidi;
            }

            if (_sender2 is not null)
            {
                _sender2.LogMidi = _settings.LogMidi;
            }
        }
        #endregion

        #region Misc functions
        /// <summary>
        /// Utility.
        /// </summary>
        /// <param name="devNum"></param>
        /// <returns></returns>
        MidiSender? GetSender(int devNum)
        {
            MidiSender? ret = devNum switch
            {
                0 => null,
                1 => _sender1,
                2 => _sender2,
                _ => throw new ArgumentOutOfRangeException(nameof(devNum)),
            };
            return ret;
        }

        /// <summary>
        /// Tell me what you have.
        /// </summary>
        void DumpMidiDevices()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                _logger.LogTrace($"Midi In {i} \"{MidiIn.DeviceInfo(i).ProductName}\"");
            }

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                _logger.LogTrace($"Midi Out {i} \"{MidiOut.DeviceInfo(i).ProductName}\"");
            }
        }
        #endregion
    }
}
