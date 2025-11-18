using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    public class ChannelControl : UserControl
    {
        #region Designer variables
        readonly Container? components = null;
        readonly Label lblChannelInfo;
        readonly Slider sldVolume;
        readonly ToolTip toolTip;
        #endregion

        #region Properties
        /// <summary>Everything about me.</summary>
        public Channel Channel { get; set; } = new();
        // public ChannelSettings Settings { get; set; } = new();

        /// <summary>Cosmetics.</summary>
        public Color ControlColor { get; set; } = Color.Red;
        #endregion

        #region Events
        /// <summary>Notify host of asynchronous changes from user.</summary>
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
                Location = new(2, 8),
                Size = new(48, 20)
            };
            Controls.Add(lblChannelInfo);

            sldVolume = new()
            {
                Location = new(56, 3),
                Size = new(83, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Orientation = Orientation.Horizontal,
                Minimum = 0.0,
                Maximum = MidiLibDefs.MAX_VOLUME,
                Resolution = 0.05,
            };
            Controls.Add(sldVolume);

            toolTip = new ToolTip(components);

            Size = new Size(142, 38);

            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Apply customization. Channel should be valid now.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sldVolume.Value = Channel.Volume;
            sldVolume.DrawColor = ControlColor;
            sldVolume.ValueChanged += Volume_ValueChanged;

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Volume_ValueChanged(object? sender, EventArgs e)
        {
            // No need to check limits.
            Channel.Volume = (sender as Slider)!.Value;
        }

        /// <summary>
        /// Handle selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelEd_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(Channel, "Channel", 300);

            // Detect changes of interest.
            foreach (var (name, cat) in changes)
            {
                switch (name)
                {
                    case "ChannelNumber":
                        ChannelChange?.Invoke(this, new() { ChannelNumberChange = true });
                        Channel.SendPatch();
                        break;
                    case "Patch":
                        ChannelChange?.Invoke(this, new() { PatchChange = true });
                        Channel.SendPatch();
                        break;
                    case "PresetFile":
                        // Handled in property setter.
                        break;
                }
            }

            UpdateUi();
        }
        #endregion


        /// <summary>
        /// Draw mode checkboxes etc.
        /// </summary>
        void UpdateUi()
        {
            // General.
            lblChannelInfo.Text = $"Ch{Channel.ChannelNumber}";
            toolTip.SetToolTip(this, ToString());
        }

        /// <summary>
        /// Read me.
         /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Ch:{Channel.ChannelNumber} Patch:{Channel.GetPatchName()}({Channel.Patch})";
        }
    }
}
