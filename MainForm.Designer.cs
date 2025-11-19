
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            btnLogMidi = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            btnKillMidi = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            btnSettings = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            txtViewer = new Ephemera.NBagOfUis.TextViewer();
            VkeyControl = new VirtualKeyboard();
            ClClControl = new ClickClack();
            VkeyChannelControl = new ChannelControl();
            ClClChannelControl = new ChannelControl();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnLogMidi, toolStripSeparator1, btnKillMidi, toolStripSeparator2, btnSettings, toolStripSeparator3 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1235, 27);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnLogMidi
            // 
            btnLogMidi.CheckOnClick = true;
            btnLogMidi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnLogMidi.Image = Properties.Resources.glyphicons_170_record;
            btnLogMidi.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnLogMidi.Name = "btnLogMidi";
            btnLogMidi.Size = new System.Drawing.Size(26, 24);
            btnLogMidi.Text = "log";
            btnLogMidi.ToolTipText = "Enable logging midi events";
            btnLogMidi.Click += LogMidi_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
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
            // txtViewer
            // 
            txtViewer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            txtViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtViewer.Location = new System.Drawing.Point(399, 222);
            txtViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtViewer.MaxText = 5000;
            txtViewer.Name = "txtViewer";
            txtViewer.Prompt = "";
            txtViewer.Size = new System.Drawing.Size(814, 291);
            txtViewer.TabIndex = 58;
            txtViewer.WordWrap = true;
            // 
            // VkeyControl
            // 
            VkeyControl.ControlColor = System.Drawing.Color.Red;
            VkeyControl.KeySize = 14;
            VkeyControl.Location = new System.Drawing.Point(22, 90);
            VkeyControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            VkeyControl.Name = "VkeyControl";
            VkeyControl.ShowNoteNames = true;
            VkeyControl.Size = new System.Drawing.Size(1200, 113);
            VkeyControl.TabIndex = 93;
            // 
            // ClClControl
            // 
            ClClControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ClClControl.ControlColor = System.Drawing.Color.Red;
            ClClControl.Location = new System.Drawing.Point(22, 222);
            ClClControl.Name = "ClClControl";
            ClClControl.Size = new System.Drawing.Size(351, 291);
            ClClControl.TabIndex = 102;
            // 
            // VkeyChannelControl
            // 
            VkeyChannelControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            VkeyChannelControl.Channel = (Channel)resources.GetObject("VkeyChannelControl.Channel");
            VkeyChannelControl.ControlColor = System.Drawing.Color.Red;
            VkeyChannelControl.Location = new System.Drawing.Point(22, 41);
            VkeyChannelControl.Name = "VkeyChannelControl";
            VkeyChannelControl.Size = new System.Drawing.Size(302, 42);
            VkeyChannelControl.TabIndex = 94;
            // 
            // ClClChannelControl
            // 
            ClClChannelControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ClClChannelControl.Channel = (Channel)resources.GetObject("ClClChannelControl.Channel");
            ClClChannelControl.ControlColor = System.Drawing.Color.Red;
            ClClChannelControl.Location = new System.Drawing.Point(399, 40);
            ClClChannelControl.Name = "ClClChannelControl";
            ClClChannelControl.Size = new System.Drawing.Size(301, 42);
            ClClChannelControl.TabIndex = 101;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1235, 532);
            Controls.Add(ClClControl);
            Controls.Add(ClClChannelControl);
            Controls.Add(VkeyChannelControl);
            Controls.Add(VkeyControl);
            Controls.Add(txtViewer);
            Controls.Add(toolStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
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
        private Ephemera.NBagOfUis.TextViewer txtViewer;
        private System.Windows.Forms.ToolStripButton btnLogMidi;
        private System.Windows.Forms.ToolStripButton btnKillMidi;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private ChannelControl VkeyChannelControl;
        private ChannelControl ClClChannelControl;
        private ClickClack ClClControl;
        private VirtualKeyboard VkeyControl;
    }
}

