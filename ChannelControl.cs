using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    /// <summary>Properties for a midi channel. TODO2 refactor Channel object?</summary>
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
                Maximum = MidiLibDefs.MAX_VOLUME,
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
            // No need to check limits.
            Settings.Volume = (sender as Slider)!.Value;
        }

        /// <summary>
        /// Handle selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelEd_Click(object? sender, EventArgs e)
        {
            var changes = SettingsEditor.Edit(Settings, "Channel Settings", 300);

            // Detect changes of interest.
            //bool restart = false;

            foreach (var (name, cat) in changes)
            {
                switch (name)
                {
                    case "ChannelNumber":
                        ChannelChange?.Invoke(this, new() { ChannelNumberChange = true });
                        break;
                    case "Patch":
                        ChannelChange?.Invoke(this, new() { PatchChange = true });
                        break;
                    case "PresetFile":
                        //restart = true;
                        break;
                }
            }

            // if (restart)
            // {
            //     MessageBox.Show("Restart required for device changes to take effect");
            // }

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
        }

        /// <summary>
        /// Read me.
         /// </summary>
        /// <returns></returns>
        //public override string ToString()
        //{
//TODO1            return $"Ch:{Settings.ChannelNumber} Patch:{MidiDefs.GetPatchName(Settings.Patch)} ({Settings.Patch})";
        //}
    }
}
