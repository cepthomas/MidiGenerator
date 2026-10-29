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

            btnKillMidi.Click += (_, __) => MidiManager.Instance.Kill();

            MidiManager.Instance.MessageReceived += Mgr_MessageReceived;
            MidiManager.Instance.MessageSent += Mgr_MessageSent;

            try
            {
                // Init the device.
                var dev = MidiManager.Instance.GetOutputDevice(_settings.OutputDevice);

                // Init the channels.
                var vkeyChannel = MidiManager.Instance.OpenOutputChannel(_settings.OutputDevice, _settings.VkeyChannel.ChannelNumber, "Virtual Key", _settings.VkeyChannel.Patch);
                var clclChannel = MidiManager.Instance.OpenOutputChannel(_settings.OutputDevice, _settings.ClClChannel.ChannelNumber, "Click Clack", _settings.ClClChannel.Patch);

                // Init the vkey controls..
                vkeyChannelControl.BoundChannel = vkeyChannel;
                vkeyChannelControl.DrawColor = _settings.DrawColor;
                vkeyChannelControl.SelectedColor = _settings.SelectedColor;
                vkeyChannelControl.Volume = VolumeDefs.DEFAULT_VOLUME;
                vkeyChannelControl.ChannelChange += ChannelControl_ChannelChange;

                vkeyControl.DrawColor = _settings.DrawColor;
                vkeyControl.SendMidi += (object? sender, BaseEvent e) =>
                {
                    e.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
                    vkeyChannel.Send(e);
                };

                // Init the clickclack controls..
                clclChannelControl.BoundChannel = clclChannel;
                clclChannelControl.DrawColor = _settings.DrawColor;
                clclChannelControl.SelectedColor = _settings.SelectedColor;
                clclChannelControl.Volume = VolumeDefs.DEFAULT_VOLUME;
                clclChannelControl.ChannelChange += ChannelControl_ChannelChange;

                clclControl.DrawColor = _settings.DrawColor;
                clclControl.SendMidi += (object? sender, BaseEvent e) =>
                {
                    e.ChannelNumber = _settings.ClClChannel.ChannelNumber;
                    clclChannel.Send(e);
                };
            }
            catch (Exception ex)
            {
                tvInfo.Append($"Something went wrong");
                tvInfo.Append(ex.Message);
            }
            
            ///// Finish up. /////
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Resources.
            MidiManager.Instance.DestroyDevices();

            // Wait a bit in case there are some lingering events.
            System.Threading.Thread.Sleep(100);

            if (disposing && (components is not null))
            {
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
            Dictionary<int, string> vals = [];
            Enumerable.Range(0, MidiDefs.MAX_MIDI + 1).ForEach(i => vals.Add(i, MidiDefs.Instruments.GetName(i) ?? "UNKNOWN"));
            var instList = MidiUtils.CreateOrderedMidiList(vals, false, true);

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
        /// UI clicked something to configure channel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelControl_ChannelChange(object? sender, ChannelChangeEventArgs e)
        {
            var cc = sender as ChannelControl;
            var channel = cc!.BoundChannel!;

            if (e.State) // TODO1 does???
            {
            }
        }

        /// <summary>
        /// Something arrived from a midi device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Mgr_MessageReceived(object? sender, BaseEvent e)
        {
            _logger.Debug($"MM Received [{e}]");
        }

        /// <summary>
        /// Something sent to a midi device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Mgr_MessageSent(object? sender, BaseEvent e)
        {
            _logger.Debug($"MM Sent [{e}]");
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
            // Usually come from a different thread.
            if (IsHandleCreated)
            {
                this.InvokeIfRequired(_ => { tvInfo.Append($"{e.Message}"); });
            }
        }
        #endregion
    }
}
