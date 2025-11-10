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
        Label lblChannelNumber;
        Label lblPatch;
        Slider sldVolume;
        #endregion

        #region Properties
        /// <summary>Actual 1-based midi channel number for UI.</summary>
        public int ChannelNumber { get; set; }

        /// <summary>Current patch.</summary>
        public int Patch { get; set; } // UpdateUi() ???

        /// <summary>Current volume. Channel.Volume performs the constraints.</summary>
        public double Volume { get; set; }

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
            sldVolume.Value = Defs.VOLUME_DEFAULT;// BoundChannel.Volume;
            sldVolume.DrawColor = ControlColor;
            sldVolume.Minimum = Defs.VOLUME_MIN;
            sldVolume.Maximum = Defs.MAX_GAIN;

            sldVolume.ValueChanged += Volume_ValueChanged;

            lblPatch.Click += Patch_Click;
            lblChannelNumber.Click += ChannelNumber_Click;

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
        void Patch_Click(object? sender, EventArgs e) // TODO1
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
        void ChannelNumber_Click(object? sender, EventArgs e) // TODO1
        {
            // for (int i = 0; i < Defs.NUM_CHANNELS; i++)
            // {
            //     cmbChannel.Items.Add($"{i + 1}");
            // }
            // cmbChannel.SelectedIndex = ChannelNumber - 1;
            // cmbChannel.SelectedIndexChanged += (_, __) => _channelNumber = cmbChannel.SelectedIndex + 1;

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
            //lblChannelNumber.BackColor = Selected ? SelectedColor : UnselectedColor;
            //lblPatch.Text = IsDrums ? "Drums" : MidiDefs.GetInstrumentName(BoundChannel.Patch);
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

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            components = new Container();

            lblChannelNumber = new Label();
            lblPatch = new Label();
            sldVolume = new Slider();

            SuspendLayout();

            lblChannelNumber.Location = new Point(2, 8);
            lblChannelNumber.Size = new Size(48, 20);
            //lblChannelNumber.BackColor = Color.LightBlue;

            lblPatch.Location = new Point(48, 8);
            lblPatch.Size = new Size(144, 20);
            lblPatch.Text = "WTF?";
            //lblPatch.BackColor = Color.LightBlue;

            sldVolume.BorderStyle = BorderStyle.FixedSingle;
            sldVolume.Location = new Point(194, 3);
            sldVolume.Orientation = Orientation.Horizontal;
            sldVolume.Size = new Size(83, 30);

            Controls.Add(sldVolume);
            Controls.Add(lblPatch);
            Controls.Add(lblChannelNumber);

            Size = new Size(345, 38);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
