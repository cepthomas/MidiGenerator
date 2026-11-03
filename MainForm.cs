using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("Main");

        /// <summary>User settings.</summary>
        readonly UserSettings _settings;

        /// <summary>Shorthand.</summary>
        readonly ChannelManager CM = ChannelManager.Instance;
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

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // Init logging.
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogMessage += LogManager_LogMessage;
            LogManager.Run(Path.Combine(appDir, "log.txt"), 50000);

            ///// Configure UI. /////
            toolStrip1.Renderer = new ToolStripCheckBoxRenderer() { SelectedColor = _settings.DrawColor };
            tvInfo.Font = Font;
            tvInfo.Matchers =
            [
                new("ERR", Color.Red),
                new("WRN", Color.Green),
            ];

            btnKillMidi.Click += (_, __) => CM.KillAll();

            CM.MessageReceived += Mgr_MessageReceived;
            CM.MessageSent += Mgr_MessageSent;
            
            ///// Finish up. /////
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
        }

        /// <summary>
        /// Configure channels defined in designer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                var chan = CM.OpenOutputChannel(_settings.OutputDevice, _settings.VkeyChannel.ChannelNumber, _settings.VkeyChannel.Patch);
                CM.Bind(chan, vkeyCC);
                // Init ChannelControl.
                vkeyCC.Name = "Virtual Key";
                vkeyCC.Volume = VolumeDefs.DEFAULT_VOLUME;
                vkeyCC.State = ChannelState.Normal;
                vkeyCC.DrawColor = _settings.DrawColor;
                vkeyCC.SelectedColor = _settings.SelectedColor;
                vkeyCC.Info = ($"Ch {_settings.VkeyChannel.ChannelNumber} {vkeyCC.Name}", $"{chan}");
                // vkeyCC.InfoClick += ChannelControl_InfoClick;
                // Init UI surface.
                vkeyControl.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
                vkeyControl.SendMidi += SendMidiVkey;

                chan = CM.OpenOutputChannel(_settings.OutputDevice, _settings.ClClChannel.ChannelNumber, _settings.ClClChannel.Patch);
                CM.Bind(chan, clclCC);
                // Init ChannelControl.
                clclCC.Name = "Click Clack";
                clclCC.Volume = VolumeDefs.DEFAULT_VOLUME;
                clclCC.State = ChannelState.Normal;
                clclCC.DrawColor = _settings.DrawColor;
                clclCC.SelectedColor = _settings.SelectedColor;
                clclCC.Info = ($"Ch {_settings.ClClChannel.ChannelNumber} {clclCC.Name}", $"{chan}");
                // clclCC.InfoClick += ChannelControl_InfoClick;
                // Init UI surface.
                clclControl.ChannelNumber = _settings.ClClChannel.ChannelNumber;
                clclControl.SendMidi += SendMidiClCl;
            }
            catch (Exception ex)
            {
                tvInfo.Append($"Something went wrong");
                tvInfo.Append(ex.Message);
            }

            CM.UpdateState();

            base.OnLoad(e);
        }

        /// <summary>
        /// Midi sender. Adjusts volume as needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="MidiLibException"></exception>
        void SendMidiVkey(object? sender, BaseEvent e)
        {
            if (sender is not VirtualKeyboard) throw new MidiLibException("Unknown/null control");
            var chan = CM.GetChannel(vkeyCC) ?? throw new MidiLibException("Unknown/null channel");

            if (e is NoteOn)
            {
                var non = e as NoteOn;
                // Adjust volume.
                var vol = vkeyCC.Volume * non!.Velocity;
                non.Velocity = MathUtils.Constrain((int)vol, 0, MidiDefs.MAX_MIDI);
            }

            chan.Send(e);
        }

        /// <summary>
        /// Midi sender. Adjusts volume as needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="MidiLibException"></exception>
        void SendMidiClCl(object? sender, BaseEvent e)
        {
            if (sender is not ClickClack) throw new MidiLibException("Unknown/null control");
            var chan = CM.GetChannel(clclCC) ?? throw new MidiLibException("Unknown/null channel");

            if (e is NoteOn)
            {
                var non = e as NoteOn;
                // Adjust volume.
                var vol = clclCC.Volume * non!.Velocity;
                non.Velocity = MathUtils.Constrain((int)vol, 0, MidiDefs.MAX_MIDI);
            }

            chan.Send(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Wait a bit in case there are some lingering events.
            System.Threading.Thread.Sleep(100);

            if (disposing && (components is not null))
            {
                CM.Destroy(Controls);
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Clean up.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings();
            base.OnFormClosing(e);
        }
        #endregion

        #region User settings
        /// <summary>
        /// Edit the options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            // Make ordered list of patch names.
            //Dictionary<int, string> vals = [];
            //Enumerable.Range(0, MidiDefs.MAX_MIDI + 1).ForEach(i => vals.Add(i, MidiDefs.Instruments.GetName(i) ?? "UNKNOWN"));

            List<string> instList = [];
            Enumerable.Range(0, MidiDefs.MAX_MIDI + 1).ForEach(i => instList.Add(MidiDefs.Instruments.GetName(i) ?? $"{i:000}"));

            GenericListTypeEditor.SetOptions("Patch", instList);
            GenericListTypeEditor.SetOptions("OutputDevice", MidiOutputDevice.GetAvailableDevices());

            var changes = SettingsEditor.Edit(_settings, "User Settings", 300);

            // Detect changes of interest.
            bool restart = changes.Any(ch => ch.name == "DrawColor" || ch.name == "OutputDevice");

            if (restart)
            {
                MessageBox.Show("Restart required for device changes to take effect");
            }

            SaveSettings();
        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location, Size);
            _settings.Save();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Something arrived from a midi device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Mgr_MessageReceived(object? sender, BaseEvent e)
        {
            _logger.Debug($"Midi Received [{e}]");
        }

        /// <summary>
        /// Something sent to a midi device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Mgr_MessageSent(object? sender, BaseEvent e)
        {
            _logger.Debug($"Midi Sent [{e}]");
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Show log events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogManager_LogMessage(object? sender, LogMessageEventArgs e)
        {
            this.InvokeIfRequired(_ => { tvInfo.Append($"{e.Message}"); });
        }
        #endregion
    }
}
