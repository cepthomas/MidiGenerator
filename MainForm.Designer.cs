
using Ephemera.MidiLib;

namespace MidiGenerator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            btnKillMidi = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            btnSettings = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            tvInfo = new Ephemera.NBagOfUis.TextViewer();
            VkeyControl = new ChannelControl();
            ClClControl = new ChannelControl();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnKillMidi, toolStripSeparator2, btnSettings, toolStripSeparator3 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1081, 27);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnKillMidi
            // 
            btnKillMidi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnKillMidi.Image = Properties.Resources.glyphicons_242_flash;
            btnKillMidi.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnKillMidi.Name = "btnKillMidi";
            btnKillMidi.Size = new System.Drawing.Size(26, 24);
            btnKillMidi.Text = "kill";
            btnKillMidi.ToolTipText = "Kill all midi channels";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSettings
            // 
            btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnSettings.Image = Properties.Resources.glyphicons_137_cogwheel;
            btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new System.Drawing.Size(26, 24);
            btnSettings.Text = "settings";
            btnSettings.Click += Settings_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // tvInfo
            // 
            tvInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tvInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tvInfo.Location = new System.Drawing.Point(399, 202);
            tvInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tvInfo.MatchUseBackground = true;
            tvInfo.MaxText = 5000;
            tvInfo.Name = "tvInfo";
            tvInfo.Prompt = "";
            tvInfo.Size = new System.Drawing.Size(671, 253);
            tvInfo.TabIndex = 58;
            tvInfo.WordWrap = true;
            // 
            // VkeyControl
            // 
            VkeyControl.BackColor = System.Drawing.SystemColors.Control;
            VkeyControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            VkeyControl.Controller = 0;
            VkeyControl.ControllerValue = 0;
            VkeyControl.Location = new System.Drawing.Point(399, 39);
            VkeyControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            VkeyControl.Name = "VkeyControl";
            VkeyControl.Size = new System.Drawing.Size(305, 50);
            VkeyControl.TabIndex = 93;
            // 
            // ClClControl
            // 
            ClClControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ClClControl.BackColor = System.Drawing.SystemColors.Control;
            ClClControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ClClControl.Controller = 0;
            ClClControl.ControllerValue = 0;
            ClClControl.Location = new System.Drawing.Point(13, 39);
            ClClControl.Name = "ClClControl";
            ClClControl.Size = new System.Drawing.Size(305, 50);
            ClClControl.TabIndex = 102;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1081, 461);
            Controls.Add(ClClControl);
            Controls.Add(VkeyControl);
            Controls.Add(tvInfo);
            Controls.Add(toolStrip1);
            Location = new System.Drawing.Point(300, 50);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Midi Generator";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private Ephemera.NBagOfUis.TextViewer tvInfo;
        private System.Windows.Forms.ToolStripButton btnKillMidi;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private ChannelControl VkeyControl;
        private ChannelControl ClClControl;
    }
}

