﻿namespace UmatoMusume
{
    partial class FrmMain
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
            groupBox1 = new GroupBox();
            btnDownloadUmaData = new Button();
            btnCaptureCharInfo = new Button();
            btnCaptureEvent = new Button();
            groupBox2 = new GroupBox();
            rtbOptions = new RichTextBox();
            lblCharacterInfo = new Label();
            label4 = new Label();
            lblEventName = new Label();
            label1 = new Label();
            groupBox3 = new GroupBox();
            rtbObjectives = new RichTextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnDownloadUmaData);
            groupBox1.Controls.Add(btnCaptureCharInfo);
            groupBox1.Controls.Add(btnCaptureEvent);
            groupBox1.Location = new Point(14, 16);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(416, 108);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Capture options";
            // 
            // btnDownloadUmaData
            // 
            btnDownloadUmaData.Location = new Point(7, 67);
            btnDownloadUmaData.Name = "btnDownloadUmaData";
            btnDownloadUmaData.Size = new Size(403, 29);
            btnDownloadUmaData.TabIndex = 2;
            btnDownloadUmaData.Text = "Download data";
            btnDownloadUmaData.UseVisualStyleBackColor = true;
            btnDownloadUmaData.Click += btnDownloadUmaData_Click;
            // 
            // btnCaptureCharInfo
            // 
            btnCaptureCharInfo.Location = new Point(215, 29);
            btnCaptureCharInfo.Margin = new Padding(3, 4, 3, 4);
            btnCaptureCharInfo.Name = "btnCaptureCharInfo";
            btnCaptureCharInfo.Size = new Size(195, 31);
            btnCaptureCharInfo.TabIndex = 1;
            btnCaptureCharInfo.Text = "Capture character info";
            btnCaptureCharInfo.UseVisualStyleBackColor = true;
            btnCaptureCharInfo.Click += btnCaptureCharInfo_Click;
            // 
            // btnCaptureEvent
            // 
            btnCaptureEvent.Location = new Point(7, 29);
            btnCaptureEvent.Margin = new Padding(3, 4, 3, 4);
            btnCaptureEvent.Name = "btnCaptureEvent";
            btnCaptureEvent.Size = new Size(202, 31);
            btnCaptureEvent.TabIndex = 0;
            btnCaptureEvent.Text = "Capture event";
            btnCaptureEvent.UseVisualStyleBackColor = true;
            btnCaptureEvent.Click += btnCaptureEvent_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rtbOptions);
            groupBox2.Controls.Add(lblCharacterInfo);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(lblEventName);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new Point(14, 132);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(416, 317);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Event selector";
            // 
            // rtbOptions
            // 
            rtbOptions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbOptions.Location = new Point(7, 94);
            rtbOptions.Name = "rtbOptions";
            rtbOptions.Size = new Size(403, 216);
            rtbOptions.TabIndex = 6;
            rtbOptions.Text = "";
            // 
            // lblCharacterInfo
            // 
            lblCharacterInfo.AutoSize = true;
            lblCharacterInfo.Location = new Point(99, 57);
            lblCharacterInfo.Name = "lblCharacterInfo";
            lblCharacterInfo.Padding = new Padding(0, 7, 0, 7);
            lblCharacterInfo.Size = new Size(83, 34);
            lblCharacterInfo.TabIndex = 5;
            lblCharacterInfo.Text = "Capturing...";
            lblCharacterInfo.TextChanged += lblCharacterInfo_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 57);
            label4.Name = "label4";
            label4.Padding = new Padding(0, 7, 0, 7);
            label4.Size = new Size(72, 34);
            label4.TabIndex = 4;
            label4.Text = "Char info:";
            // 
            // lblEventName
            // 
            lblEventName.AutoSize = true;
            lblEventName.Location = new Point(99, 24);
            lblEventName.Name = "lblEventName";
            lblEventName.Padding = new Padding(0, 7, 0, 7);
            lblEventName.Size = new Size(83, 34);
            lblEventName.TabIndex = 3;
            lblEventName.Text = "Capturing...";
            lblEventName.TextChanged += lblEventName_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 24);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 7, 0, 7);
            label1.Size = new Size(93, 34);
            label1.TabIndex = 0;
            label1.Text = "Event name: ";
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(rtbObjectives);
            groupBox3.Location = new Point(14, 457);
            groupBox3.Margin = new Padding(3, 4, 3, 4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 4, 3, 4);
            groupBox3.Size = new Size(416, 585);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Objectives";
            // 
            // rtbObjectives
            // 
            rtbObjectives.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbObjectives.Location = new Point(7, 27);
            rtbObjectives.Name = "rtbObjectives";
            rtbObjectives.Size = new Size(403, 551);
            rtbObjectives.TabIndex = 0;
            rtbObjectives.Text = "";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(439, 1055);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmMain";
            Text = "FrmMain";
            FormClosing += FrmMain_FormClosing;
            Load += FrmMain_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnCaptureCharInfo;
        private Button btnCaptureEvent;
        private GroupBox groupBox2;
        private Label label1;
        private Label lblEventName;
        private Label lblCharacterInfo;
        private Label label4;
        private GroupBox groupBox3;
        private RichTextBox rtbOptions;
        private RichTextBox rtbObjectives;
        private Button btnDownloadUmaData;
    }
}