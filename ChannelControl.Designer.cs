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
            SuspendLayout();
            // 
            // sldControllerValue
            // 
            sldControllerValue.BackColor = System.Drawing.SystemColors.Control;
            sldControllerValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            sldControllerValue.DrawColor = System.Drawing.Color.White;
            sldControllerValue.Label = "";
            sldControllerValue.Location = new System.Drawing.Point(95, 5);
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
            txtChannelInfo.Location = new System.Drawing.Point(185, 8);
            txtChannelInfo.Name = "txtChannelInfo";
            txtChannelInfo.ReadOnly = true;
            txtChannelInfo.Size = new System.Drawing.Size(182, 26);
            txtChannelInfo.TabIndex = 3;
            // 
            // sldVolume
            // 
            sldVolume.BackColor = System.Drawing.SystemColors.Control;
            sldVolume.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            sldVolume.DrawColor = System.Drawing.Color.White;
            sldVolume.Label = "";
            sldVolume.Location = new System.Drawing.Point(5, 5);
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
            // ChannelControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Control;
            Controls.Add(txtChannelInfo);
            Controls.Add(sldControllerValue);
            Controls.Add(sldVolume);
            Name = "ChannelControl";
            Size = new System.Drawing.Size(372, 42);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        //private System.Windows.Forms.Label lblChannelInfo;
        private System.Windows.Forms.TextBox txtChannelInfo;
        private Ephemera.NBagOfUis.Slider sldVolume;
        private Ephemera.NBagOfUis.Slider sldControllerValue;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
