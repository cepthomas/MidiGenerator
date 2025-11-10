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
    /// <summary>Channel events and other properties.</summary>  // TODO1 needs channel/patch selectors.
    public class ChannelControl : UserControl
    {
        #region Events
        /// <summary>Notify host of asynchronous changes from user.</summary>
        public event EventHandler<ChannelChangeEventArgs>? ChannelChange;
        #endregion

        #region Properties

        /// <summary>Actual 1-based midi channel number for UI.</summary>
        public int ChannelNumber { get; set; }

        /// <summary>Current patch.</summary>
        public int Patch { get; set; } // UpdateUi()

        /// <summary>Current volume. Channel.Volume performs the constraints.</summary>
        public double Volume { get; set; }

        /// <summary>Drum channel changed.</summary>
        public bool IsDrums { get; set; }

        /// <summary>User has selected this channel.</summary>
        public bool Selected { get; set; }

        /// <summary>Indicate user selected.</summary>
        public Color SelectedColor { get; set; } = Color.Aquamarine;

        /// <summary>Indicate user not selected.</summary>
        public Color UnselectedColor { get; set; } = DefaultBackColor;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public ChannelControl()
        {
            InitializeComponent();
            sldVolume.ValueChanged += Volume_ValueChanged;
            // lblSolo.Click += SoloMute_Click;
            // lblMute.Click += SoloMute_Click;
            lblChannelNumber.Click += ChannelNumber_Click;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {

            sldVolume.Value = 0.8;// BoundChannel.Volume;
            sldVolume.DrawColor = SelectedColor;
            sldVolume.Minimum = MidiDefs.VOLUME_MIN;
            sldVolume.Maximum = MidiDefs.MAX_GAIN;

            UpdateUi();

            base.OnLoad(e);
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
                Volume = (sender as Slider)!.Value;
            }
        }

        /// <summary>
        /// User wants to change the patch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Patch_Click(object sender, EventArgs e) // TODO1
        {
            // PatchPicker pp = new();
            // pp.ShowDialog();
            // if (pp.PatchNumber != -1)
            // {
            //     BoundChannel.Patch = pp.PatchNumber;
            //     UpdateUi();
            //     ChannelChange?.Invoke(this, new() { PatchChange = true });
            // }
        }

        /// <summary>
        /// Handle selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelNumber_Click(object? sender, EventArgs e)
        {
            Selected = !Selected;
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
            lblChannelNumber.Text = $"Ch{ChannelNumber}";
            lblChannelNumber.BackColor = Selected ? SelectedColor : UnselectedColor;
 //           lblPatch.Text = IsDrums ? "Drums" : MidiDefs.GetInstrumentName(BoundChannel.Patch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ChannelControl: ChannelNumber:{ChannelNumber} Patch:{Patch}";
        }
        #endregion

/////////////////////// designer /////////////////////

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblChannelNumber = new System.Windows.Forms.Label();
            this.lblPatch = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.sldVolume = new Slider();
            this.SuspendLayout();
            // 
            // lblChannelNumber
            // 
            this.lblChannelNumber.AutoSize = true;
            this.lblChannelNumber.Location = new System.Drawing.Point(2, 8);
            this.lblChannelNumber.Name = "lblChannelNumber";
            this.lblChannelNumber.Size = new System.Drawing.Size(18, 20);
            this.lblChannelNumber.TabIndex = 3;
            this.lblChannelNumber.Text = "#";
            this.toolTip1.SetToolTip(this.lblChannelNumber, "Select channel");
            // 
            // lblPatch
            // 
            this.lblPatch.Location = new System.Drawing.Point(44, 7);
            this.lblPatch.Name = "lblPatch";
            this.lblPatch.Size = new System.Drawing.Size(144, 25);
            this.lblPatch.TabIndex = 44;
            this.lblPatch.Text = "?????";
            this.lblPatch.Click += new System.EventHandler(this.Patch_Click);
            // 
            // 
            // 
            // 
            // 
            // sldVolume
            // 
            this.sldVolume.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sldVolume.DrawColor = System.Drawing.Color.White;
            this.sldVolume.Label = "";
            this.sldVolume.Location = new System.Drawing.Point(194, 3);
            this.sldVolume.Maximum = 10D;
            this.sldVolume.Minimum = 0D;
            this.sldVolume.Name = "sldVolume";
            this.sldVolume.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sldVolume.Resolution = 0.1D;
            this.sldVolume.Size = new System.Drawing.Size(83, 30);
            this.sldVolume.TabIndex = 47;
            this.sldVolume.Value = 5D;
            // 
            // ChannelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sldVolume);
            this.Controls.Add(this.lblPatch);
            this.Controls.Add(this.lblChannelNumber);
            this.Name = "ChannelControl";
            this.Size = new System.Drawing.Size(345, 38);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblChannelNumber;
        private System.Windows.Forms.Label lblPatch;
        private System.Windows.Forms.ToolTip toolTip1;
        private Slider sldVolume;


    }
}
