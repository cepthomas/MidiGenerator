using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ephemera.NBagOfUis;


namespace MidiGenerator
{
    /// <summary>Channel events and other properties.</summary>
    public class ChannelControl : UserControl
    {
        #region Designer variables
        IContainer components = null;
        Label lblChannelInfo;
        // Label lblPatch;
        Slider sldGain;
        readonly ToolTip toolTip;
        #endregion

        #region Properties
        /// <summary>Actual 1-based midi channel number for UI.</summary>
        public int ChannelNumber { get; set; }

        /// <summary>Current patch.</summary>
        public int Patch { get; set; } // UpdateUi() ???

        /// <summary>Current gain.</summary>
        public double Gain { get; set; }

        /// <summary>Drum channel.</summary>
        public bool IsDrums { get; set; }

        public Color ControlColor { get; set; } = Color.Orange;
        #endregion

        #region Events
        /// <summary>Notify host of asynchronous changes from user.</summary>
        public event EventHandler<ChannelChangeEventArgs>? ChannelChange;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public ChannelControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sldGain.Value = Defs.DEFAULT_GAIN;
            sldGain.DrawColor = ControlColor;
            sldGain.Minimum = 0.0;
            sldGain.Maximum = Defs.MAX_GAIN;

            sldGain.ValueChanged += Gain_ValueChanged;

            // lblPatch.Click += Patch_Click;
            lblChannelInfo.Click += ChannelInfo_Click;

            toolTip.SetToolTip(this, string.Join(Environment.NewLine, "Info"));

            UpdateUi();

            base.OnLoad(e);
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
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
        void Gain_ValueChanged(object? sender, EventArgs e)
        {
            if (sender is not null)
            {
                Gain = (sender as Slider)!.Value;
            }
        }

        /// <summary>
        /// Handle selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelInfo_Click(object? sender, EventArgs e)
        {
            // TODO1 user selects channum and patch.

            
            // for (int i = 0; i < Defs.NUM_CHANNELS; i++)
            // {
            //     cmbChannel.Items.Add($"{i + 1}");
            // }
            // cmbChannel.SelectedIndex = ChannelNumber - 1;
            // cmbChannel.SelectedIndexChanged += (_, __) => _channelNumber = cmbChannel.SelectedIndex + 1;


            // PatchPicker pp = new();
            // pp.ShowDialog();
            // if (pp.PatchNumber != -1)
            // {
            //     BoundChannel.Patch = pp.PatchNumber;
            //     UpdateUi();
            //     ChannelChange?.Invoke(this, new() { PatchChange = true });
            // }

            UpdateUi();
        }
        #endregion

        #region Misc
        /// <summary>
        /// Draw mode checkboxes etc.
        /// </summary>
        void UpdateUi()
        {
            // General.
            lblChannelInfo.Text = $"Ch{ChannelNumber}";
            //lblChannelInfo.BackColor = Selected ? SelectedColor : UnselectedColor;
            //lblPatch.Text = IsDrums ? "Drums" : MidiDefs.GetInstrumentName(BoundChannel.Patch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ChannelControl: Ch:{ChannelNumber} Patch:{Patch}";
        }
        #endregion

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            components = new Container();

            lblChannelInfo = new Label();
            // lblPatch = new Label();
            sldGain = new Slider();

            SuspendLayout();

            lblChannelInfo.Location = new Point(2, 8);
            lblChannelInfo.Size = new Size(48, 20);
            //lblChannelInfo.BackColor = Color.LightBlue;

            // lblPatch.Location = new Point(48, 8);
            // lblPatch.Size = new Size(144, 20);
            // lblPatch.Text = "WTF?";
            //lblPatch.BackColor = Color.LightBlue;

            sldGain.BorderStyle = BorderStyle.FixedSingle;
            sldGain.Location = new Point(194, 3);
            sldGain.Orientation = Orientation.Horizontal;
            sldGain.Size = new Size(83, 30);

            Controls.Add(sldGain);
   //         Controls.Add(lblPatch);
            Controls.Add(lblChannelInfo);

            Size = new Size(345, 38);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
