using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace MidiGenerator
{
    public partial class ChannelControl : UserControl
    {
        #region Properties
        /// <summary>Everything about me.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Channel BoundChannel { get; set; } = new();

        /// <summary>Cosmetics.</summary>
        public Color ControlColor { get; set; } = Color.Red;

        /// <summary>The graphics draw area.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected Rectangle DrawRect { get { return new Rectangle(0, sldVolume.Bottom + 4, Width, Height - (sldVolume.Bottom + 4)); } }
        #endregion

        #region Events
        /// <summary>Notify host of changes from user.</summary>
        public event EventHandler<ChannelEventArgs>? ChannelChange;

        /// <summary>Click info.</summary>
        public event EventHandler<NoteEventArgs>? NoteSend;

        /// <summary>Notify host of changes from user.</summary>
        public event EventHandler<ControllerEventArgs>? ControllerSend;

        /// <summary>Derived class helper.</summary>
        protected virtual void OnNoteSend(NoteEventArgs e)
        {
            NoteSend?.Invoke(this, e);
        }

        /// <summary>Derived class helper.</summary>
        protected virtual void OnControllerSend(ControllerEventArgs e)
        {
            ControllerSend?.Invoke(this, e);
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor. Create controls.
        /// </summary>
        public ChannelControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Apply customization. Channel should be valid now.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sldVolume.Minimum = 0.0;
            sldVolume.Maximum = Defs.MAX_VOLUME;
            sldVolume.Resolution = 0.05;
            sldVolume.Value = BoundChannel.Volume;
            sldVolume.DrawColor = ControlColor;
            sldVolume.ValueChanged += (object? sender, EventArgs e) => BoundChannel.Volume = (sender as Slider)!.Value;

            sldControllerValue.Minimum = 0;
            sldControllerValue.Maximum = MidiDefs.MAX_MIDI;
            sldControllerValue.Resolution = 1;
            sldControllerValue.Value = BoundChannel.ControllerValue;
            sldControllerValue.DrawColor = ControlColor;
            sldControllerValue.ValueChanged += Controller_ValueChanged;

            txtChannelInfo.Click += ChannelInfo_Click;
            //txtChannelInfo.BackColor = ControlColor;

            UpdateUi();

            base.OnLoad(e);
        }
        #endregion

        #region Handlers for user selections
        /// <summary>
        /// Notify client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Controller_ValueChanged(object? sender, EventArgs e)
        {
            // No need to check limits.
            BoundChannel.ControllerValue = (int)(sender as Slider)!.Value;
            OnControllerSend(new() { ControllerId = BoundChannel.ControllerId, Value = BoundChannel.ControllerValue });
        }

        /// <summary>
        /// Edit channel properties. Notifies client of any changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelInfo_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(BoundChannel, "Channel", 300);

            // Notify client.
            ChannelEventArgs args = new()
            {
                ChannelNumberChange = changes.Any(ch => ch.name == "ChannelNumber"),
                PatchChange = changes.Any(ch => ch.name == "Patch"),
                PresetFileChange = changes.Any(ch => ch.name == "PresetFile"),
            };

            ChannelChange?.Invoke(this, new() { ChannelNumberChange = true });

            UpdateUi();
        }
        #endregion

        /// <summary>
        /// Draw mode checkboxes etc.
        /// </summary>
        void UpdateUi()
        {
            txtChannelInfo.Text = ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Channel {BoundChannel.ChannelNumber}");
            sb.AppendLine($"{BoundChannel.GetPatchName(BoundChannel.Patch)} {BoundChannel.Patch}");
            toolTip.SetToolTip(txtChannelInfo, sb.ToString());
        }

        /// <summary>
        /// Read me.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Ch{BoundChannel.ChannelNumber} P{BoundChannel.Patch}";
        }
    }
}
