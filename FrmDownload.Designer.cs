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
            btnCrawlSupport = new Button();
            btnCrawlUma = new Button();
            btnDownloadSupport = new Button();
            btnDownloadUma = new Button();
            pbDownload = new ProgressBar();
            btnDownloadCareer = new Button();
            btnCrawlCareer = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
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
            groupBox1.Size = new Size(504, 117);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Downloader options";
            // 
            // btnCrawlSupport
            // 
            btnCrawlSupport.Location = new Point(171, 46);
            btnCrawlSupport.Margin = new Padding(3, 2, 3, 2);
            btnCrawlSupport.Name = "btnCrawlSupport";
            btnCrawlSupport.Size = new Size(166, 22);
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
            btnCrawlUma.Size = new Size(160, 22);
            btnCrawlUma.TabIndex = 3;
            btnCrawlUma.Text = "Crawl uma data";
            btnCrawlUma.UseVisualStyleBackColor = true;
            btnCrawlUma.Click += btnCrawlUma_Click;
            // 
            // btnDownloadSupport
            // 
            btnDownloadSupport.Location = new Point(171, 20);
            btnDownloadSupport.Margin = new Padding(3, 2, 3, 2);
            btnDownloadSupport.Name = "btnDownloadSupport";
            btnDownloadSupport.Size = new Size(166, 22);
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
            btnDownloadUma.Size = new Size(160, 22);
            btnDownloadUma.TabIndex = 1;
            btnDownloadUma.Text = "Download uma data";
            btnDownloadUma.UseVisualStyleBackColor = true;
            btnDownloadUma.Click += btnDownloadUma_Click;
            // 
            // pbDownload
            // 
            pbDownload.Location = new Point(5, 91);
            pbDownload.Margin = new Padding(3, 2, 3, 2);
            pbDownload.Name = "pbDownload";
            pbDownload.Size = new Size(494, 22);
            pbDownload.TabIndex = 0;
            // 
            // btnDownloadCareer
            // 
            btnDownloadCareer.Location = new Point(343, 20);
            btnDownloadCareer.Name = "btnDownloadCareer";
            btnDownloadCareer.Size = new Size(155, 23);
            btnDownloadCareer.TabIndex = 5;
            btnDownloadCareer.Text = "Download career data";
            btnDownloadCareer.UseVisualStyleBackColor = true;
            btnDownloadCareer.Click += btnDownloadCareer_Click;
            // 
            // btnCrawlCareer
            // 
            btnCrawlCareer.Location = new Point(343, 45);
            btnCrawlCareer.Name = "btnCrawlCareer";
            btnCrawlCareer.Size = new Size(155, 23);
            btnCrawlCareer.TabIndex = 5;
            btnCrawlCareer.Text = "Crawl career data";
            btnCrawlCareer.UseVisualStyleBackColor = true;
            btnCrawlCareer.Click += btnCrawlCareer_Click;
            // 
            // FrmDownload
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(525, 132);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "FrmDownload";
            Text = "Umamusume Data Downloader";
            groupBox1.ResumeLayout(false);
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
    }
}