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
			chkUseGame8Scraping = new CheckBox();
			chkUseDefaultData = new CheckBox();
			chkUseRapid = new CheckBox();
			chkUsePaddle = new CheckBox();
			chkFullScreen = new CheckBox();
			chkRightMenu = new CheckBox();
			chkCheckForUpdates = new CheckBox();
			groupBox1.SuspendLayout();
			SuspendLayout();
			int settingTabIndex = 0;

			// 
			// chkCheckForUpdates
			// 
			chkCheckForUpdates.AutoSize = true;
			chkCheckForUpdates.Location = new Point(6, 22 + (25 * settingTabIndex));
			chkCheckForUpdates.Name = "chkCheckForUpdates";
			chkCheckForUpdates.Size = new Size(175, 19);
			chkCheckForUpdates.TabIndex = settingTabIndex++;
			chkCheckForUpdates.Text = "Check for updates at startup";
			chkCheckForUpdates.UseVisualStyleBackColor = true;
			chkCheckForUpdates.CheckedChanged += chkCheckForUpdates_CheckedChanged;
			// 
			// chkRightMenu
			// 
			chkRightMenu.AutoSize = true;
			chkRightMenu.Location = new Point(6, 22 + (25 * settingTabIndex));
			chkRightMenu.Name = "chkRightMenu";
			chkRightMenu.Size = new Size(88, 19);
			chkRightMenu.TabIndex = settingTabIndex++;
			chkRightMenu.Text = "Right menu";
			chkRightMenu.UseVisualStyleBackColor = true;
			chkRightMenu.CheckedChanged += chkRightMenu_CheckedChanged;
			// 
			// chkFullScreen
			// 
			chkFullScreen.AutoSize = true;
			chkFullScreen.Location = new Point(5, 22 + (25 * settingTabIndex));
			chkFullScreen.Name = "chkFullScreen";
			chkFullScreen.Size = new Size(113, 19);
			chkFullScreen.TabIndex = settingTabIndex++;
			chkFullScreen.Text = "Fullscreen mode";
			chkFullScreen.UseVisualStyleBackColor = true;
			chkFullScreen.CheckedChanged += chkFullScreen_CheckedChanged;
			// 
			// chkUsePaddle
			// 
			chkUsePaddle.AutoSize = true;
			chkUsePaddle.Location = new Point(5, 22 + (25 * settingTabIndex));
			chkUsePaddle.Margin = new Padding(3, 2, 3, 2);
			chkUsePaddle.Name = "chkUsePaddle";
			chkUsePaddle.Size = new Size(239, 19);
			chkUsePaddle.TabIndex = settingTabIndex++;
			chkUsePaddle.Text = "Use PaddleOCR (applies on next startup)";
			chkUsePaddle.UseVisualStyleBackColor = true;
			chkUsePaddle.CheckedChanged += chkUsePaddle_CheckedChanged;
			// 
			// chkUseRapid
			// 
			chkUseRapid.AutoSize = true;
			chkUseRapid.Location = new Point(5, 22 + (25 * settingTabIndex));
			chkUseRapid.Margin = new Padding(3, 2, 3, 2);
			chkUseRapid.Name = "chkUseRapid";
			chkUseRapid.Size = new Size(233, 19);
			chkUseRapid.TabIndex = settingTabIndex++;
			chkUseRapid.Text = "Use RapidOCR (applies on next startup)";
			chkUseRapid.UseVisualStyleBackColor = true;
			chkUseRapid.CheckedChanged += chkUseRapid_CheckedChanged;
			// 
			// chkUseGameTora
			// 
			chkUseDefaultData.AutoSize = true;
			chkUseDefaultData.Location = new Point(5, 22 + (25 * settingTabIndex));
			chkUseDefaultData.Margin = new Padding(3, 2, 3, 2);
			chkUseDefaultData.Name = "chkUseDefaultData";
			chkUseDefaultData.Size = new Size(239, 19);
			chkUseDefaultData.TabIndex = settingTabIndex++;
			chkUseDefaultData.Text = "Use Default Data Method (Restart might needed)";
			chkUseDefaultData.UseVisualStyleBackColor = true;
			chkUseDefaultData.CheckedChanged += chkUseDefaultData_CheckedChanged;
			// 
			// chkUseGame8
			// 
			chkUseGame8Scraping.AutoSize = true;
			chkUseGame8Scraping.Location = new Point(5, 22 + (25 * settingTabIndex));
			chkUseGame8Scraping.Margin = new Padding(3, 2, 3, 2);
			chkUseGame8Scraping.Name = "chkUseGame8ScrapingData";
			chkUseGame8Scraping.Size = new Size(239, 19);
			chkUseGame8Scraping.TabIndex = settingTabIndex++;
			chkUseGame8Scraping.Text = "Use Game8 Scraping Method (Restart might needed)";
			chkUseGame8Scraping.UseVisualStyleBackColor = true;
			chkUseGame8Scraping.CheckedChanged += chkUseGame8_CheckedChanged;

			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(chkUseGame8Scraping);
			groupBox1.Controls.Add(chkUseDefaultData);
			groupBox1.Controls.Add(chkUseRapid);
			groupBox1.Controls.Add(chkUsePaddle);
			groupBox1.Controls.Add(chkFullScreen);
			groupBox1.Controls.Add(chkRightMenu);
			groupBox1.Controls.Add(chkCheckForUpdates);
			groupBox1.Location = new Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(350, 22 + (25 * settingTabIndex));
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Setting";

			// 
			// FrmSetting
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(374, 214);
			Controls.Add(groupBox1);
			Icon = (Icon)resources.GetObject("$this.Icon");
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
        private CheckBox chkUseGame8Scraping;
        private CheckBox chkUseDefaultData;
        private CheckBox chkUsePaddle;
        private CheckBox chkUseRapid;
    }
}
