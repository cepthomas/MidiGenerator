
using MidiLib;

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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLogMidi = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnKillMidi = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.txtViewer = new NBagOfUis.TextViewer();
            this.vkey = new MidiLib.VirtualKeyboard();
            this.ccVkey = new MidiLib.ChannelControl();
            this.ccBingBong = new MidiLib.ChannelControl();
            this.pboxBingBong = new System.Windows.Forms.PictureBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxBingBong)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLogMidi,
            this.toolStripSeparator1,
            this.btnKillMidi,
            this.toolStripSeparator2,
            this.btnSettings,
            this.toolStripSeparator3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1235, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLogMidi
            // 
            this.btnLogMidi.CheckOnClick = true;
            this.btnLogMidi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLogMidi.Image = global::MidiGenerator.Properties.Resources.glyphicons_170_record;
            this.btnLogMidi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLogMidi.Name = "btnLogMidi";
            this.btnLogMidi.Size = new System.Drawing.Size(29, 24);
            this.btnLogMidi.Text = "log";
            this.btnLogMidi.ToolTipText = "Enable logging midi events";
            this.btnLogMidi.Click += new System.EventHandler(this.LogMidi_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnKillMidi
            // 
            this.btnKillMidi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnKillMidi.Image = global::MidiGenerator.Properties.Resources.glyphicons_242_flash;
            this.btnKillMidi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnKillMidi.Name = "btnKillMidi";
            this.btnKillMidi.Size = new System.Drawing.Size(29, 24);
            this.btnKillMidi.Text = "kill";
            this.btnKillMidi.ToolTipText = "Kill all midi channels";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSettings
            // 
            this.btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSettings.Image = global::MidiGenerator.Properties.Resources.glyphicons_137_cogwheel;
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(29, 24);
            this.btnSettings.Text = "settings";
            this.btnSettings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // txtViewer
            // 
            this.txtViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtViewer.Location = new System.Drawing.Point(514, 409);
            this.txtViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtViewer.MaxText = 5000;
            this.txtViewer.Name = "txtViewer";
            this.txtViewer.Size = new System.Drawing.Size(709, 357);
            this.txtViewer.TabIndex = 58;
            this.txtViewer.WordWrap = true;
            // 
            // vkey
            // 
            this.vkey.KeySize = 14;
            this.vkey.Location = new System.Drawing.Point(22, 95);
            this.vkey.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.vkey.Name = "vkey";
            this.vkey.ShowNoteNames = true;
            this.vkey.Size = new System.Drawing.Size(1200, 119);
            this.vkey.TabIndex = 93;
            this.vkey.KeyboardEvent += new System.EventHandler<MidiLib.VirtualKeyboard.KeyboardEventArgs>(this.Vkey_KeyboardEvent);
            // 
            // ccVkey
            // 
            this.ccVkey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ccVkey.ChannelNumber = 1;
            this.ccVkey.ControlColor = System.Drawing.Color.MediumOrchid;
            this.ccVkey.Location = new System.Drawing.Point(22, 43);
            this.ccVkey.Name = "ccVkey";
            this.ccVkey.Patch = 0;
            this.ccVkey.Size = new System.Drawing.Size(292, 44);
            this.ccVkey.TabIndex = 94;
            this.ccVkey.Volume = 0.8D;
            // 
            // ccBingBong
            // 
            this.ccBingBong.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ccBingBong.ChannelNumber = 1;
            this.ccBingBong.ControlColor = System.Drawing.Color.MediumOrchid;
            this.ccBingBong.Location = new System.Drawing.Point(33, 250);
            this.ccBingBong.Name = "ccBingBong";
            this.ccBingBong.Patch = 0;
            this.ccBingBong.Size = new System.Drawing.Size(292, 44);
            this.ccBingBong.TabIndex = 101;
            this.ccBingBong.Volume = 0.8D;
            // 
            // pboxBingBong
            // 
            this.pboxBingBong.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pboxBingBong.Location = new System.Drawing.Point(33, 315);
            this.pboxBingBong.Name = "pboxBingBong";
            this.pboxBingBong.Size = new System.Drawing.Size(256, 256);
            this.pboxBingBong.TabIndex = 102;
            this.pboxBingBong.TabStop = false;
            this.pboxBingBong.Click += new System.EventHandler(this.BingBong_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 779);
            this.Controls.Add(this.pboxBingBong);
            this.Controls.Add(this.ccBingBong);
            this.Controls.Add(this.ccVkey);
            this.Controls.Add(this.vkey);
            this.Controls.Add(this.txtViewer);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(300, 50);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Midi Generator";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxBingBong)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private NBagOfUis.TextViewer txtViewer;
        private System.Windows.Forms.ToolStripButton btnLogMidi;
        private System.Windows.Forms.ToolStripButton btnKillMidi;
        private VirtualKeyboard vkey;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private ChannelControl ccVkey;
        private ChannelControl ccBingBong;
        private System.Windows.Forms.PictureBox pboxBingBong;
    }
}

