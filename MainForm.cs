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
using Ephemera.MidiLib;


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("Main");

        /// <summary>Midi boss.</summary>
        readonly Manager _mgr = new();

        /// <summary>User settings.</summary>
        readonly UserSettings _settings;

        /// <summary>Low level midi output device.</summary>
        MidiOut? _midiOut = null;
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
            LogManager.Run(Path.Combine(appDir, "log.txt"), 50000);

            ///// Configure UI. /////
            toolStrip1.Renderer = new ToolStripCheckBoxRenderer() { SelectedColor = _settings.DrawColor };
            txtViewer.Font = Font;
            txtViewer.MatchText.Add("ERR", Color.LightPink);
            txtViewer.MatchText.Add("WRN", Color.Plum);

            btnLogMidi.Checked = _settings.LogMidi;
            btnKillMidi.Click += (_, __) => _mgr.Kill();

            _mgr.MessageReceive += Mgr_MessageReceive;
            _mgr.MessageSend += Mgr_MessageSend;

            ///// Init the device and channels.
            var dev = _mgr.GetOutputDevice(_settings.OutputDevice);
            var vkeyChannel = _mgr.OpenMidiOutput(_settings.OutputDevice, 1, "Virtual Key", _settings.VkeyChannel.ChannelNumber);
            var clclChannel = _mgr.OpenMidiOutput(_settings.OutputDevice, 2, "Click Clack", _settings.ClClChannel.ChannelNumber);

            var rend1 = new VirtualKeyboard()
            {
                DrawColor = _settings.DrawColor,
                KeySize = 12,
                HighNote = 108,
                LowNote = 21,
                ShowNoteNames = true
            };
            rend1.SendMidi += ChannelControl_SendMidi;
            VkeyControl.UserRenderer = rend1;
            VkeyControl.BorderStyle = BorderStyle.FixedSingle;
            VkeyControl.DrawColor = _settings.DrawColor;
            VkeyControl.SelectedColor = _settings.SelectedColor;
            VkeyControl.Volume = Defs.DEFAULT_VOLUME;
            VkeyControl.ChannelChange += ChannelControl_ChannelChange;
            VkeyControl.SendMidi += ChannelControl_SendMidi;
            VkeyControl.BoundChannel = vkeyChannel;

            var rend2 = new ClickClack()
            {
                DrawColor = _settings.DrawColor
            };
            rend2.SendMidi += ChannelControl_SendMidi;
            ClClControl.UserRenderer = rend2;
            ClClControl.BorderStyle = BorderStyle.FixedSingle;
            ClClControl.DrawColor = _settings.DrawColor;
            ClClControl.SelectedColor = _settings.SelectedColor;
            ClClControl.Volume = Defs.DEFAULT_VOLUME;
            ClClControl.ChannelChange += ChannelControl_ChannelChange;
            ClClControl.SendMidi += ChannelControl_SendMidi;
            ClClControl.BoundChannel = clclChannel;

            ///// Finish up. /////
            SendPatch(vkeyChannel.ChannelNumber, vkeyChannel.Patch);
            SendPatch(clclChannel.ChannelNumber, clclChannel.Patch);

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
            _midiOut?.Dispose();

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
        /// Send some midi. Works for different sources.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelControl_SendMidi(object? sender, BaseMidi e)
        {
            var channel = (sender as ChannelControl)!.BoundChannel;

            if (channel is not null && channel.Enable)
            {
                // Fill in the channel number.
                e.ChannelNumber = channel.ChannelNumber;
                _logger.Debug($"Channel send [{e}]");
                channel.Device.Send(e);
            }
        }

        /// <summary>
        /// UI clicked something to configure channel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelControl_ChannelChange(object? sender, ChannelChangeEventArgs e)
        {
            var cc = sender as ChannelControl;
            var channel = cc!.BoundChannel!;

            //if (e.StateChange) // TODO1
            //{
            //    // Update all channels.
            //    bool anySolo = _channelControls.Where(c => c.State == ChannelControl.ChannelState.Solo).Any();

            //    foreach (var cciter in _channelControls)
            //    {
            //        bool enable = anySolo ?
            //            cciter.State == ChannelControl.ChannelState.Solo :
            //            cciter.State != ChannelControl.ChannelState.Mute;

            //        channel.Enable = enable;
            //        if (!enable)
            //        {
            //            // Kill just in case.
            //            _mgr.Kill(channel);
            //        }
            //    }
            //}

            //if (e.PatchChange)
            //{
            //    _logger.Debug(INFO, $"PatchChange [{channel.Patch}]");
            //    channel.Device.Send(new Patch(channel.ChannelNumber, channel.Patch));
            //}

            //if (e.AliasFileChange)
            //{
            //    _logger.Debug(INFO, $"AliasFileChange [{channel.AliasFile}]");
            //}
        }

        /// <summary>
        /// Something arrived from a midi device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Mgr_MessageReceive(object? sender, BaseMidi e)
        {
            _logger.Debug($"Receive [{e}]");
        }

        /// <summary>
        /// Something sent to a midi device. This is what was actually sent, not what the
        /// channel thought it was sending.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Mgr_MessageSend(object? sender, BaseMidi e)
        {
            _logger.Debug($"Send actual [{e}]");
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
            _logger.Trace($"Note Ch:{chanNum} N:{note} V:{velocity}");
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
            _logger.Trace($"Patch Ch:{chanNum} P:{patch}");
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
            _logger.Trace($"Controller Ch:{chanNum} C:{controller} V:{val}");
            ControlChangeEvent evt = new(0, chanNum, controller, val);
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
    }
}
