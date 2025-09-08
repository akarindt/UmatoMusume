namespace UmatoMusume
{
    partial class FrmSetting
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSetting));
            groupBox1 = new GroupBox();
            chkUseRapid = new CheckBox();
            chkUsePaddle = new CheckBox();
            chkFullScreen = new CheckBox();
            chkRightMenu = new CheckBox();
            chkCheckForUpdates = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(chkUseRapid);
            groupBox1.Controls.Add(chkUsePaddle);
            groupBox1.Controls.Add(chkFullScreen);
            groupBox1.Controls.Add(chkRightMenu);
            groupBox1.Controls.Add(chkCheckForUpdates);
            groupBox1.Location = new Point(14, 16);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(400, 253);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Setting";
            // 
            // chkUseRapid
            // 
            chkUseRapid.AutoSize = true;
            chkUseRapid.Location = new Point(6, 156);
            chkUseRapid.Name = "chkUseRapid";
            chkUseRapid.Size = new Size(292, 24);
            chkUseRapid.TabIndex = 4;
            chkUseRapid.Text = "Use RapidOCR (applies on next startup)";
            chkUseRapid.UseVisualStyleBackColor = true;
            chkUseRapid.CheckedChanged += chkUseRapid_CheckedChanged;
            // 
            // chkUsePaddle
            // 
            chkUsePaddle.AutoSize = true;
            chkUsePaddle.Location = new Point(6, 126);
            chkUsePaddle.Name = "chkUsePaddle";
            chkUsePaddle.Size = new Size(298, 24);
            chkUsePaddle.TabIndex = 3;
            chkUsePaddle.Text = "Use PaddleOCR (applies on next startup)";
            chkUsePaddle.UseVisualStyleBackColor = true;
            chkUsePaddle.CheckedChanged += chkUsePaddle_CheckedChanged;
            // 
            // chkFullScreen
            // 
            chkFullScreen.AutoSize = true;
            chkFullScreen.Location = new Point(6, 95);
            chkFullScreen.Margin = new Padding(3, 4, 3, 4);
            chkFullScreen.Name = "chkFullScreen";
            chkFullScreen.Size = new Size(304, 24);
            chkFullScreen.TabIndex = 2;
            chkFullScreen.Text = "Fullscreen mode (applies on next startup)";
            chkFullScreen.UseVisualStyleBackColor = true;
            chkFullScreen.CheckedChanged += chkFullScreen_CheckedChanged;
            // 
            // chkRightMenu
            // 
            chkRightMenu.AutoSize = true;
            chkRightMenu.Location = new Point(7, 63);
            chkRightMenu.Margin = new Padding(3, 4, 3, 4);
            chkRightMenu.Name = "chkRightMenu";
            chkRightMenu.Size = new Size(107, 24);
            chkRightMenu.TabIndex = 1;
            chkRightMenu.Text = "Right menu";
            chkRightMenu.UseVisualStyleBackColor = true;
            chkRightMenu.CheckedChanged += chkRightMenu_CheckedChanged;
            // 
            // chkCheckForUpdates
            // 
            chkCheckForUpdates.AutoSize = true;
            chkCheckForUpdates.Location = new Point(7, 29);
            chkCheckForUpdates.Margin = new Padding(3, 4, 3, 4);
            chkCheckForUpdates.Name = "chkCheckForUpdates";
            chkCheckForUpdates.Size = new Size(217, 24);
            chkCheckForUpdates.TabIndex = 0;
            chkCheckForUpdates.Text = "Check for updates at startup";
            chkCheckForUpdates.UseVisualStyleBackColor = true;
            chkCheckForUpdates.CheckedChanged += chkCheckForUpdates_CheckedChanged;
            // 
            // FrmSetting
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(427, 285);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmSetting";
            Text = "Setting";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private CheckBox chkRightMenu;
        private CheckBox chkCheckForUpdates;
        private CheckBox chkFullScreen;
        private CheckBox chkUsePaddle;
        private CheckBox chkUseRapid;
    }
}