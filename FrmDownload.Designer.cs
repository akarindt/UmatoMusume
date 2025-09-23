namespace UmatoMusume
{
    partial class FrmDownload
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDownload));
			groupBox1 = new GroupBox();
			btnCrawlRaces = new Button();
			btnDownloadRaces = new Button();
			lblProgress = new Label();
			btnCrawlCareer = new Button();
			btnDownloadCareer = new Button();
			btnCrawlSupport = new Button();
			btnCrawlUma = new Button();
			btnDownloadSupport = new Button();
			btnDownloadUma = new Button();
			pbDownload = new ProgressBar();
			groupBox1.SuspendLayout();
			SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(btnCrawlRaces);
			groupBox1.Controls.Add(btnDownloadRaces);
			groupBox1.Controls.Add(lblProgress);
			groupBox1.Controls.Add(btnCrawlCareer);
			groupBox1.Controls.Add(btnDownloadCareer);
			groupBox1.Controls.Add(btnCrawlSupport);
			groupBox1.Controls.Add(btnCrawlUma);
			groupBox1.Controls.Add(btnDownloadSupport);
			groupBox1.Controls.Add(btnDownloadUma);
			groupBox1.Controls.Add(pbDownload);
			groupBox1.Location = new Point(10, 9);
			groupBox1.Margin = new Padding(3, 2, 3, 2);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(3, 2, 3, 2);
			groupBox1.Size = new Size(583, 134);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Downloader options";
			// 
			// btnCrawlRaces
			// 
			btnCrawlRaces.Location = new Point(434, 45);
			btnCrawlRaces.Name = "btnCrawlRaces";
			btnCrawlRaces.Size = new Size(143, 23);
			btnCrawlRaces.TabIndex = 7;
			btnCrawlRaces.Text = "Crawl races data";
			btnCrawlRaces.UseVisualStyleBackColor = true;
			btnCrawlRaces.Click += btnCrawlRaces_Click;
			// 
			// btnDownloadRaces
			// 
			btnDownloadRaces.Location = new Point(434, 21);
			btnDownloadRaces.Name = "btnDownloadRaces";
			btnDownloadRaces.Size = new Size(143, 23);
			btnDownloadRaces.TabIndex = 8;
			btnDownloadRaces.Text = "Download races data";
			btnDownloadRaces.UseVisualStyleBackColor = true;
			btnDownloadRaces.Click += btnDownloadRaces_Click;
			// 
			// lblProgress
			// 
			lblProgress.AutoSize = true;
			lblProgress.Location = new Point(6, 87);
			lblProgress.Name = "lblProgress";
			lblProgress.Padding = new Padding(0, 2, 0, 2);
			lblProgress.Size = new Size(39, 19);
			lblProgress.TabIndex = 6;
			lblProgress.Text = "Ready";
			// 
			// btnCrawlCareer
			// 
			btnCrawlCareer.Location = new Point(292, 45);
			btnCrawlCareer.Name = "btnCrawlCareer";
			btnCrawlCareer.Size = new Size(136, 23);
			btnCrawlCareer.TabIndex = 5;
			btnCrawlCareer.Text = "Crawl career data";
			btnCrawlCareer.UseVisualStyleBackColor = true;
			btnCrawlCareer.Click += btnCrawlCareer_Click;
			// 
			// btnDownloadCareer
			// 
			btnDownloadCareer.Location = new Point(292, 21);
			btnDownloadCareer.Name = "btnDownloadCareer";
			btnDownloadCareer.Size = new Size(136, 23);
			btnDownloadCareer.TabIndex = 5;
			btnDownloadCareer.Text = "Download career data";
			btnDownloadCareer.UseVisualStyleBackColor = true;
			btnDownloadCareer.Click += btnDownloadCareer_Click;
			// 
			// btnCrawlSupport
			// 
			btnCrawlSupport.Location = new Point(141, 46);
			btnCrawlSupport.Margin = new Padding(3, 2, 3, 2);
			btnCrawlSupport.Name = "btnCrawlSupport";
			btnCrawlSupport.Size = new Size(145, 22);
			btnCrawlSupport.TabIndex = 4;
			btnCrawlSupport.Text = "Crawl support data";
			btnCrawlSupport.UseVisualStyleBackColor = true;
			btnCrawlSupport.Click += btnCrawlSupport_Click;
			// 
			// btnCrawlUma
			// 
			btnCrawlUma.Location = new Point(5, 46);
			btnCrawlUma.Margin = new Padding(3, 2, 3, 2);
			btnCrawlUma.Name = "btnCrawlUma";
			btnCrawlUma.Size = new Size(130, 22);
			btnCrawlUma.TabIndex = 3;
			btnCrawlUma.Text = "Crawl uma data";
			btnCrawlUma.UseVisualStyleBackColor = true;
			btnCrawlUma.Click += btnCrawlUma_Click;
			// 
			// btnDownloadSupport
			// 
			btnDownloadSupport.Location = new Point(141, 20);
			btnDownloadSupport.Margin = new Padding(3, 2, 3, 2);
			btnDownloadSupport.Name = "btnDownloadSupport";
			btnDownloadSupport.Size = new Size(145, 22);
			btnDownloadSupport.TabIndex = 2;
			btnDownloadSupport.Text = "Download support data";
			btnDownloadSupport.UseVisualStyleBackColor = true;
			btnDownloadSupport.Click += btnDownloadSupport_Click;
			// 
			// btnDownloadUma
			// 
			btnDownloadUma.Location = new Point(5, 20);
			btnDownloadUma.Margin = new Padding(3, 2, 3, 2);
			btnDownloadUma.Name = "btnDownloadUma";
			btnDownloadUma.Size = new Size(130, 22);
			btnDownloadUma.TabIndex = 1;
			btnDownloadUma.Text = "Download uma data";
			btnDownloadUma.UseVisualStyleBackColor = true;
			btnDownloadUma.Click += btnDownloadUma_Click;
			// 
			// pbDownload
			// 
			pbDownload.Location = new Point(4, 108);
			pbDownload.Margin = new Padding(3, 2, 3, 2);
			pbDownload.Name = "pbDownload";
			pbDownload.Size = new Size(573, 22);
			pbDownload.TabIndex = 0;
			// 
			// FrmDownload
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(600, 154);
			Controls.Add(groupBox1);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(3, 2, 3, 2);
			Name = "FrmDownload";
			Text = "Umamusume Data Downloader";
			FormClosed += FrmDownload_FormClosed;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private GroupBox groupBox1;
        private Button btnCrawlSupport;
        private Button btnCrawlUma;
        private Button btnDownloadSupport;
        private Button btnDownloadUma;
        private ProgressBar pbDownload;
        private Button btnCrawlCareer;
        private Button btnDownloadCareer;
        private Label lblProgress;
        private Button btnCrawlRaces;
        private Button btnDownloadRaces;
    }
}
