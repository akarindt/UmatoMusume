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
            chkFullScreen = new CheckBox();
            chkRightMenu = new CheckBox();
            chkCheckForUpdates = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(chkFullScreen);
            groupBox1.Controls.Add(chkRightMenu);
            groupBox1.Controls.Add(chkCheckForUpdates);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(350, 190);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Setting";
            // 
            // chkFullScreen
            // 
            chkFullScreen.AutoSize = true;
            chkFullScreen.Location = new Point(6, 72);
            chkFullScreen.Name = "chkFullScreen";
            chkFullScreen.Size = new Size(244, 19);
            chkFullScreen.TabIndex = 2;
            chkFullScreen.Text = "Fullscreen mode (applies on next startup)";
            chkFullScreen.UseVisualStyleBackColor = true;
            chkFullScreen.CheckedChanged += chkFullScreen_CheckedChanged;
            // 
            // chkRightMenu
            // 
            chkRightMenu.AutoSize = true;
            chkRightMenu.Location = new Point(6, 47);
            chkRightMenu.Name = "chkRightMenu";
            chkRightMenu.Size = new Size(88, 19);
            chkRightMenu.TabIndex = 1;
            chkRightMenu.Text = "Right menu";
            chkRightMenu.UseVisualStyleBackColor = true;
            chkRightMenu.CheckedChanged += chkRightMenu_CheckedChanged;
            // 
            // chkCheckForUpdates
            // 
            chkCheckForUpdates.AutoSize = true;
            chkCheckForUpdates.Location = new Point(6, 22);
            chkCheckForUpdates.Name = "chkCheckForUpdates";
            chkCheckForUpdates.Size = new Size(175, 19);
            chkCheckForUpdates.TabIndex = 0;
            chkCheckForUpdates.Text = "Check for updates at startup";
            chkCheckForUpdates.UseVisualStyleBackColor = true;
            chkCheckForUpdates.CheckedChanged += chkCheckForUpdates_CheckedChanged;
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
    }
}