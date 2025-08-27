namespace UmatoMusume
{
    partial class FrmUpdater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdater));
            btnReDown = new Button();
            btnCheckUpdate = new Button();
            lblUpdate = new Label();
            pUpdater = new ProgressBar();
            SuspendLayout();
            // 
            // btnReDown
            // 
            btnReDown.Location = new Point(149, 65);
            btnReDown.Name = "btnReDown";
            btnReDown.Size = new Size(105, 33);
            btnReDown.TabIndex = 7;
            btnReDown.Text = "Re-Download";
            btnReDown.UseVisualStyleBackColor = true;
            btnReDown.Click += btnReDown_Click;
            // 
            // btnCheckUpdate
            // 
            btnCheckUpdate.Location = new Point(12, 65);
            btnCheckUpdate.Name = "btnCheckUpdate";
            btnCheckUpdate.Size = new Size(131, 33);
            btnCheckUpdate.TabIndex = 6;
            btnCheckUpdate.Text = "Check for updates";
            btnCheckUpdate.UseVisualStyleBackColor = true;
            btnCheckUpdate.Click += btnCheckUpdate_Click;
            // 
            // lblUpdate
            // 
            lblUpdate.AutoSize = true;
            lblUpdate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUpdate.Location = new Point(12, 12);
            lblUpdate.Margin = new Padding(3);
            lblUpdate.Name = "lblUpdate";
            lblUpdate.Size = new Size(44, 17);
            lblUpdate.TabIndex = 5;
            lblUpdate.Text = "Ready";
            // 
            // pUpdater
            // 
            pUpdater.Location = new Point(12, 35);
            pUpdater.Name = "pUpdater";
            pUpdater.Size = new Size(635, 24);
            pUpdater.TabIndex = 4;
            // 
            // FrmUpdater
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(659, 114);
            Controls.Add(btnReDown);
            Controls.Add(btnCheckUpdate);
            Controls.Add(lblUpdate);
            Controls.Add(pUpdater);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmUpdater";
            Text = "Umatomusume Auto Updater";
            Load += FrmUpdater_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnReDown;
        private Button btnCheckUpdate;
        private Label lblUpdate;
        private ProgressBar pUpdater;
    }
}