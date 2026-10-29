
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
            vkeyChannelControl = new ChannelControl();
            clclChannelControl = new ChannelControl();
            clclControl = new ClickClack();
            vkeyControl = new VirtualKeyboard();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnKillMidi, toolStripSeparator2, btnSettings, toolStripSeparator3 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1004, 27);
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
            tvInfo.Location = new System.Drawing.Point(12, 305);
            tvInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tvInfo.Name = "tvInfo";
            tvInfo.Size = new System.Drawing.Size(979, 179);
            tvInfo.TabIndex = 58;
            // 
            // vkeyChannelControl
            // 
            vkeyChannelControl.BackColor = System.Drawing.SystemColors.Control;
            vkeyChannelControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            vkeyChannelControl.Location = new System.Drawing.Point(234, 39);
            vkeyChannelControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            vkeyChannelControl.Name = "vkeyChannelControl";
            vkeyChannelControl.Size = new System.Drawing.Size(116, 74);
            vkeyChannelControl.TabIndex = 93;
            // 
            // clclChannelControl
            // 
            clclChannelControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            clclChannelControl.BackColor = System.Drawing.SystemColors.Control;
            clclChannelControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            clclChannelControl.Location = new System.Drawing.Point(13, 39);
            clclChannelControl.Name = "clclChannelControl";
            clclChannelControl.Size = new System.Drawing.Size(116, 110);
            clclChannelControl.TabIndex = 102;
            // 
            // clclControl
            // 
            clclControl.Location = new System.Drawing.Point(13, 119);
            clclControl.Name = "clclControl";
            clclControl.Size = new System.Drawing.Size(197, 171);
            clclControl.TabIndex = 103;
            // 
            // virtualKeyboard1
            // 
            vkeyControl.Location = new System.Drawing.Point(234, 121);
            vkeyControl.Name = "virtualKeyboard1";
            vkeyControl.Size = new System.Drawing.Size(757, 115);
            vkeyControl.TabIndex = 104;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1004, 497);
            Controls.Add(vkeyControl);
            Controls.Add(clclControl);
            Controls.Add(clclChannelControl);
            Controls.Add(vkeyChannelControl);
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
        private ChannelControl vkeyChannelControl;
        private ChannelControl clclChannelControl;
        private ClickClack clclControl;
        private VirtualKeyboard vkeyControl;
    }
}

