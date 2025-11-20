namespace MidiGenerator
{
    partial class ChannelControl
    {
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
            components = new System.ComponentModel.Container();
            sldControllerValue = new Ephemera.NBagOfUis.Slider();
            txtChannelInfo = new System.Windows.Forms.TextBox();
            sldVolume = new Ephemera.NBagOfUis.Slider();
            toolTip = new System.Windows.Forms.ToolTip(components);
            picDraw = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)picDraw).BeginInit();
            SuspendLayout();
            // 
            // sldControllerValue
            // 
            sldControllerValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            sldControllerValue.BackColor = System.Drawing.Color.RosyBrown;
            sldControllerValue.DrawColor = System.Drawing.Color.White;
            sldControllerValue.Label = "";
            sldControllerValue.Location = new System.Drawing.Point(252, 4);
            sldControllerValue.Maximum = 10D;
            sldControllerValue.Minimum = 0D;
            sldControllerValue.Name = "sldControllerValue";
            sldControllerValue.Orientation = System.Windows.Forms.Orientation.Horizontal;
            sldControllerValue.Resolution = 0.1D;
            sldControllerValue.Size = new System.Drawing.Size(80, 32);
            sldControllerValue.TabIndex = 2;
            sldControllerValue.TabStop = false;
            sldControllerValue.Value = 5D;
            // 
            // txtChannelInfo
            // 
            txtChannelInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtChannelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtChannelInfo.Location = new System.Drawing.Point(7, 7);
            txtChannelInfo.Name = "txtChannelInfo";
            txtChannelInfo.ReadOnly = true;
            txtChannelInfo.Size = new System.Drawing.Size(152, 26);
            txtChannelInfo.TabIndex = 3;
            // 
            // sldVolume
            // 
            sldVolume.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            sldVolume.BackColor = System.Drawing.Color.RosyBrown;
            sldVolume.DrawColor = System.Drawing.Color.White;
            sldVolume.Label = "";
            sldVolume.Location = new System.Drawing.Point(165, 4);
            sldVolume.Maximum = 10D;
            sldVolume.Minimum = 0D;
            sldVolume.Name = "sldVolume";
            sldVolume.Orientation = System.Windows.Forms.Orientation.Horizontal;
            sldVolume.Resolution = 0.1D;
            sldVolume.Size = new System.Drawing.Size(80, 32);
            sldVolume.TabIndex = 1;
            sldVolume.TabStop = false;
            sldVolume.Value = 5D;
            // 
            // picDraw
            // 
            picDraw.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            picDraw.Location = new System.Drawing.Point(7, 43);
            picDraw.Name = "picDraw";
            picDraw.Size = new System.Drawing.Size(325, 55);
            picDraw.TabIndex = 4;
            picDraw.TabStop = false;
            // 
            // ChannelControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(picDraw);
            Controls.Add(txtChannelInfo);
            Controls.Add(sldControllerValue);
            Controls.Add(sldVolume);
            Name = "ChannelControl";
            Size = new System.Drawing.Size(338, 101);
            ((System.ComponentModel.ISupportInitialize)picDraw).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        //private System.Windows.Forms.Label lblChannelInfo;
        private System.Windows.Forms.TextBox txtChannelInfo;
        private Ephemera.NBagOfUis.Slider sldVolume;
        private Ephemera.NBagOfUis.Slider sldControllerValue;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picDraw;
    }
}
