
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
using NBagOfUis;
using MidiLib;



// out midi devices need channel sel, patch, vol


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Types

        #endregion

        #region Fields - internal
        /// <summary>All the channels. Index is 0-based, not channel number.</summary>
        readonly Channel[] _channels = new Channel[MidiDefs.NUM_CHANNELS];

        /// <summary>Midi player.</summary>
        MidiSender _sender;

        /// <summary>Adjust to taste.</summary>
        readonly string _exportPath = @"C:\Dev\repos\MidiLib\out";
        #endregion

        #region Fields - user custom
        /// <summary>Cosmetics.</summary>
        readonly Color _controlColor = Color.Aquamarine;

        /// <summary>My midi out.</summary>
        readonly string _midiOutDevice = "VirtualMIDISynth #1";
        //readonly string _midiOutDevice = "loopMIDI Port";
        #endregion

        UserSettings _settings;

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
            _exportPath = Path.Combine(appDir, "export");
            DirectoryInfo di = new(_exportPath);
            di.Create();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(_settings.FormGeometry.X, _settings.FormGeometry.Y);
            Size = new Size(_settings.FormGeometry.Width, _settings.FormGeometry.Height);
            //KeyPreview = true; // for routing kbd strokes through OnKeyDown

            // Configure UI.
            toolStrip1.Renderer = new NBagOfUis.CheckBoxRenderer() { SelectedColor = _controlColor };

            // The text output.
            txtViewer.Font = Font;
            txtViewer.WordWrap = true;
            txtViewer.Colors.Add("ERR", Color.LightPink);
            txtViewer.Colors.Add("WRN", Color.Plum);

            // Set up midi.
            DumpMidiDevices();
            _sender = new(_midiOutDevice, _exportPath);
            if (!_sender.Valid)
            {
                LogMessage($"ERR Something wrong with your midi output device:{_midiOutDevice}");
            }

            // Hook up some simple UI handlers.
            btnKillMidi.Click += (_, __) => { _sender.KillAll(); };
            btnLogMidi.Click += (_, __) => { _sender.LogMidi = btnLogMidi.Checked; };

            vkey.ShowNoteNames = true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Resources.
            _sender.Dispose();

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
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            _settings.Save();
        }

        /// <summary>
        /// Edit the common options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            var changes = _settings.Edit("User Settings");

            // Detect changes of interest.
            bool restart = false;

            foreach (var (name, cat) in changes)
            {
                restart |= name.EndsWith("Device");
            }

            if (restart)
            {
                MessageBox.Show("Restart required for device changes to take effect");
            }

            SaveSettings();
        }
        #endregion

        /// <summary>
        /// Something you should know.
        /// </summary>
        /// <param name="msg"></param>
        void LogMessage(string msg)
        {
            string s = $"> {msg}";
            txtViewer.AppendLine(s);
        }

        /// <summary>
        /// Tell me what you have.
        /// </summary>
        void DumpMidiDevices()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                LogMessage($"Midi In {i} \"{MidiIn.DeviceInfo(i).ProductName}\"");
            }

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                LogMessage($"Midi Out {i} \"{MidiOut.DeviceInfo(i).ProductName}\"");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Vkey_KeyboardEvent(object? sender, VirtualKeyboard.KeyboardEventArgs e)
        {
            LogMessage($"INF Vkey C:{e.ChannelNumber} N:{e.NoteId} V:{e.Velocity}");
        }
    }
}

