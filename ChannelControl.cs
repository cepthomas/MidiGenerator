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
            InitializeComponent();

            sldVolume.Minimum = 0.0;
            sldVolume.Maximum = MiscDefs.MAX_VOLUME;
            sldVolume.Resolution = 0.05;
            sldVolume.Value = BoundChannel.Volume;
            sldVolume.DrawColor = ControlColor;
            sldVolume.ValueChanged += Volume_ValueChanged;

            sldControllerValue.Minimum = 0;
            sldControllerValue.Maximum = MidiDefs.MAX_MIDI;
            sldControllerValue.Resolution = 1;
            sldControllerValue.Value = BoundChannel.ControllerValue;
            sldControllerValue.DrawColor = ControlColor;
            sldControllerValue.ValueChanged += Controller_ValueChanged;

            txtChannelInfo.Click += ChannelEd_Click;
            txtChannelInfo.BackColor = Color.LightBlue;

            UpdateUi();
        }

        // /// <summary>
        // /// Apply customization. Channel should be valid now.
        // /// </summary>
        // /// <param name="e"></param>
        // protected override void OnLoad(EventArgs e)
        // {
        //     sldVolume.Value = BoundChannel.Volume;
        //     sldVolume.DrawColor = ControlColor;
        //     sldVolume.ValueChanged += Volume_ValueChanged;

        //     sldControllerValue.Value = BoundChannel.ControllerValue;
        //     sldControllerValue.DrawColor = ControlColor;
        //     sldControllerValue.ValueChanged += Controller_ValueChanged;

        //     txtChannelInfo.Click += ChannelEd_Click;
        //     txtChannelInfo.BackColor = Color.LightBlue;

        //     UpdateUi();

        //     base.OnLoad(e);
        // }

        // /// <summary> 
        // /// Clean up any resources being used.
        // /// </summary>
        // /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        // protected override void Dispose(bool disposing)
        // {
        //     if (disposing && (components != null))
        //     {
        //         components.Dispose();
        //     }
        //     base.Dispose(disposing);
        // }
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
            txtChannelInfo.Text = ToString().Left(30);
            toolTip.SetToolTip(txtChannelInfo, "TODO1 ??????");
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
