
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
            MidiLib.Channel channel1 = new MidiLib.Channel();
            MidiLib.Channel channel2 = new MidiLib.Channel();
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
            this.bb = new MidiLib.BingBong();
            this.toolStrip1.SuspendLayout();
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
            this.txtViewer.Location = new System.Drawing.Point(354, 234);
            this.txtViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtViewer.MaxText = 5000;
            this.txtViewer.Name = "txtViewer";
            this.txtViewer.Size = new System.Drawing.Size(859, 306);
            this.txtViewer.TabIndex = 58;
            this.txtViewer.WordWrap = true;
            // 
            // vkey
            // 
            this.vkey.CaptureEnable = false;
            this.vkey.DeviceName = "VirtualKeyboard";
            this.vkey.KeySize = 14;
            this.vkey.Location = new System.Drawing.Point(22, 95);
            this.vkey.LogEnable = false;
            this.vkey.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.vkey.Name = "vkey";
            this.vkey.ShowNoteNames = true;
            this.vkey.Size = new System.Drawing.Size(1200, 119);
            this.vkey.TabIndex = 93;
            // 
            // ccVkey
            // 
            this.ccVkey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            channel1.AddNoteOff = false;
            channel1.ChannelName = "";
            channel1.ChannelNumber = -1;
            channel1.Device = null;
            channel1.DeviceId = "";
            channel1.IsDrums = false;
            channel1.Patch = 0;
            channel1.Selected = false;
            channel1.State = MidiLib.ChannelState.Normal;
            channel1.Volume = 0.8D;
            this.ccVkey.BoundChannel = channel1;
            this.ccVkey.IsDrums = false;
            this.ccVkey.Location = new System.Drawing.Point(22, 43);
            this.ccVkey.Name = "ccVkey";
            this.ccVkey.Patch = 0;
            this.ccVkey.Selected = false;
            this.ccVkey.SelectedColor = System.Drawing.Color.Aquamarine;
            this.ccVkey.Size = new System.Drawing.Size(292, 44);
            this.ccVkey.State = MidiLib.ChannelState.Normal;
            this.ccVkey.TabIndex = 94;
            this.ccVkey.UnselectedColor = System.Drawing.SystemColors.Control;
            this.ccVkey.Volume = 0.8D;
            // 
            // ccBingBong
            // 
            this.ccBingBong.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            channel2.AddNoteOff = false;
            channel2.ChannelName = "";
            channel2.ChannelNumber = -1;
            channel2.Device = null;
            channel2.DeviceId = "";
            channel2.IsDrums = false;
            channel2.Patch = 0;
            channel2.Selected = false;
            channel2.State = MidiLib.ChannelState.Normal;
            channel2.Volume = 0.8D;
            this.ccBingBong.BoundChannel = channel2;
            this.ccBingBong.IsDrums = false;
            this.ccBingBong.Location = new System.Drawing.Point(22, 234);
            this.ccBingBong.Name = "ccBingBong";
            this.ccBingBong.Patch = 0;
            this.ccBingBong.Selected = false;
            this.ccBingBong.SelectedColor = System.Drawing.Color.Aquamarine;
            this.ccBingBong.Size = new System.Drawing.Size(292, 44);
            this.ccBingBong.State = MidiLib.ChannelState.Normal;
            this.ccBingBong.TabIndex = 101;
            this.ccBingBong.UnselectedColor = System.Drawing.SystemColors.Control;
            this.ccBingBong.Volume = 0.8D;
            // 
            // bb
            // 
            this.bb.CaptureEnable = false;
            this.bb.DeviceName = "BingBong";
            this.bb.DrawNoteGrid = true;
            this.bb.Location = new System.Drawing.Point(22, 284);
            this.bb.LogEnable = false;
            this.bb.MaxControl = 127;
            this.bb.MaxNote = 95;
            this.bb.MinControl = 0;
            this.bb.MinNote = 24;
            this.bb.Name = "bb";
            this.bb.Size = new System.Drawing.Size(256, 256);
            this.bb.TabIndex = 102;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 560);
            this.Controls.Add(this.bb);
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
        private BingBong bb;
    }
}

