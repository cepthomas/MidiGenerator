using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;





namespace MidiGenerator
{
    public class ControllerControl : UserControl
    {
        #region Designer variables
        readonly Container? components = null;
        readonly Label lblControllerInfo;
        readonly Slider sldValue;
        readonly ToolTip toolTip;
        #endregion

// controller:
// channel
// picker for controller id 0-127 w/wo translate - custom list like Channel.Instruments
// slider 0-127 value

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
        public ControllerControl()
        {
            components = new Container();
            SuspendLayout();

            lblControllerInfo = new()
            {
                Location = new(4, 8),
                Size = new(200, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(lblControllerInfo);

            sldValue = new()
            {
                Location = new(lblControllerInfo.Right + 4, 3),
                Size = new(83, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Orientation = Orientation.Horizontal,
                Minimum = 0.0,
                Maximum = MiscDefs.MAX_VOLUME,
                Resolution = 0.05,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(sldValue);

            toolTip = new ToolTip(components);

            Size = new Size(sldValue.Right + 4, 38); // default

            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Apply customization. Channel should be valid now.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sldValue.Value = BoundChannel.Volume;
            sldValue.DrawColor = ControlColor;
            sldValue.ValueChanged += Volume_ValueChanged;

            lblControllerInfo.Click += ChannelEd_Click;
            lblControllerInfo.BackColor = Color.LightBlue;

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Volume_ValueChanged(object? sender, EventArgs e)
        {
            // No need to check limits.
            BoundChannel.Volume = (sender as Slider)!.Value;
        }

        /// <summary>
        /// Edit channel properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelEd_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(BoundChannel, "Channel", 300);

            // Detect changes of interest.
            ChannelChangeEventArgs args = new()
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
            // General.
            lblControllerInfo.Text = ToString().Left(30);
            toolTip.SetToolTip(lblControllerInfo, "??????");
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
