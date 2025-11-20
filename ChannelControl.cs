using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace MidiGenerator
{
    public class ChannelControl : UserControl
    {
        #region Designer variables
        readonly Container? components = null;
        readonly Label lblChannelInfo;
        readonly Slider sldVolume;
        readonly Slider sldController;
        readonly ToolTip toolTip;
        #endregion

        #region Properties
        /// <summary>Everything about me.</summary>
        public Channel BoundChannel { get; set; } = new();

        /// <summary>Cosmetics.</summary>
        public Color ControlColor { get; set; } = Color.Red;
        #endregion

        #region Events
        /// <summary>Notify host of changes from user.</summary>
        public event EventHandler<ChannelChangeEventArgs>? ChannelChange;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor. Create controls.
        /// </summary>
        public ChannelControl()
        {
            components = new Container();
            SuspendLayout();

            lblChannelInfo = new()
            {
                Location = new(4, 8),
                Size = new(200, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(lblChannelInfo);

            sldVolume = new()
            {
                Location = new(lblChannelInfo.Right + 4, 3),
                Size = new(83, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Orientation = Orientation.Horizontal,
                Minimum = 0.0,
                Maximum = MiscDefs.MAX_VOLUME,
                Resolution = 0.05,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(sldVolume);

            sldController = new()
            {
                Location = new(sldVolume.Right + 4, 3),
                Size = new(83, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Orientation = Orientation.Horizontal,
                Minimum = 0,
                Maximum = MidiDefs.MAX_MIDI,
                Resolution = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(sldController);

            toolTip = new ToolTip(components);

            Size = new Size(sldController.Right + 4, 38); // default

            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Apply customization. Channel should be valid now.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sldVolume.Value = BoundChannel.Volume;
            sldVolume.DrawColor = ControlColor;
            sldVolume.ValueChanged += Volume_ValueChanged;

            sldController.Value = BoundChannel.ControllerValue;
            sldController.DrawColor = ControlColor;
            sldController.ValueChanged += Controller_ValueChanged;

            lblChannelInfo.Click += ChannelEd_Click;
            lblChannelInfo.BackColor = Color.LightBlue;

            UpdateUi();

            base.OnLoad(e);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Handlers for user selections
        /// <summary>
        /// No need to notify.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Volume_ValueChanged(object? sender, EventArgs e)
        {
            // No need to check limits.
            BoundChannel.Volume = (sender as Slider)!.Value;
        }

        /// <summary>
        /// TODO1 notify client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Controller_ValueChanged(object? sender, EventArgs e)
        {
            // No need to check limits.
            BoundChannel.ControllerValue = (int)(sender as Slider)!.Value;
        }

        /// <summary>
        /// Edit channel properties. Notifies client of any changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelEd_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(BoundChannel, "Channel", 300);

            // Detect changes of interest.
            //bool restart = false;
            ChannelChangeEventArgs args = new()
            {
                ChannelNumberChange = changes.Any(ch => ch.name == "ChannelNumber"),
                PatchChange = changes.Any(ch => ch.name == "Patch"),
                PresetFileChange = changes.Any(ch => ch.name == "PresetFile"),
                ControllerIdChange = changes.Any(ch => ch.name == "ControllerId"),
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
            // General.
            lblChannelInfo.Text = ToString().Left(30);
            toolTip.SetToolTip(lblChannelInfo, "TODO1 ??????");
        }

        /// <summary>
        /// Read me.
         /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Ch {BoundChannel.ChannelNumber} {BoundChannel.Instruments[BoundChannel.Patch]}({BoundChannel.Patch})";
        }
    }
}
