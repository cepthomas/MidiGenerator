using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ephemera.NBagOfUis;


using MidiLibNew;


namespace MidiGenerator
{
    /// <summary>Properties for a midi channel.</summary>
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
        public ChannelSettings Settings { get; set; } = new();

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
                Location = new(194, 3),
                Size = new(83, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Orientation = Orientation.Horizontal,
                Minimum = 0.0,
                Maximum = Defs.MAX_VOLUME,
                Resolution = 0.05,
            };
            Controls.Add(sldVolume);

            toolTip = new ToolTip(components);

            Size = new Size(345, 38);

            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Apply customization. Settings should be valid now.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sldVolume.Value = Settings.Volume;
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
            if (sender is not null)
            {
                Settings.Volume = (sender as Slider)!.Value;
            }
        }

        /// <summary>
        /// Handle selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelEd_Click(object? sender, EventArgs e)
        {
            //var ed = new ChannelEd(Settings);
            //ed.ShowDialog();

            SettingsEditor.Edit(Settings, "User Settings", 400);

            UpdateUi();
        }
        #endregion








        /// <summary>
        /// Draw mode checkboxes etc.
        /// </summary>
        void UpdateUi()
        {
            // General.
            lblChannelInfo.Text = $"Ch{Settings.ChannelNumber}";

            toolTip.SetToolTip(this, string.Join(Environment.NewLine, "Info"));

            //lblChannelInfo.BackColor = Selected ? SelectedColor : UnselectedColor;
            //lblPatch.Text = IsDrums ? "Drums" : MidiDefs.GetInstrumentName(BoundChannel.Patch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Ch:{Settings.ChannelNumber} Patch:{Settings.Patch}"; // TODO1 patch name
        }
    }
}
