namespace UmatoMusume
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            groupBox1 = new GroupBox();
            btnCaptureDateTime = new Button();
            btnDownloadUmaData = new Button();
            btnCaptureEvent = new Button();
            groupBox2 = new GroupBox();
            cboCharacterName = new ComboBox();
            rtbOptions = new RichTextBox();
            label4 = new Label();
            lblEventName = new Label();
            label1 = new Label();
            groupBox3 = new GroupBox();
            rtbObjectives = new RichTextBox();
            splitterObjectives = new Splitter();
            groupBox4 = new GroupBox();
            richTextBox1 = new RichTextBox();
            splitter1 = new Splitter();
            splitter2 = new Splitter();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnCaptureDateTime);
            groupBox1.Controls.Add(btnDownloadUmaData);
            groupBox1.Controls.Add(btnCaptureEvent);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(14, 4, 14, 4);
            groupBox1.Size = new Size(437, 108);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Capture options";
            // 
            // btnCaptureDateTime
            // 
            btnCaptureDateTime.Location = new Point(222, 29);
            btnCaptureDateTime.Name = "btnCaptureDateTime";
            btnCaptureDateTime.Size = new Size(206, 32);
            btnCaptureDateTime.TabIndex = 3;
            btnCaptureDateTime.Text = "Capture date/time";
            btnCaptureDateTime.UseVisualStyleBackColor = true;
            // 
            // btnDownloadUmaData
            // 
            btnDownloadUmaData.Location = new Point(7, 67);
            btnDownloadUmaData.Name = "btnDownloadUmaData";
            btnDownloadUmaData.Size = new Size(421, 29);
            btnDownloadUmaData.TabIndex = 2;
            btnDownloadUmaData.Text = "Download data";
            btnDownloadUmaData.UseVisualStyleBackColor = true;
            btnDownloadUmaData.Click += btnDownloadUmaData_Click;
            // 
            // btnCaptureEvent
            // 
            btnCaptureEvent.Location = new Point(7, 29);
            btnCaptureEvent.Margin = new Padding(3, 4, 3, 4);
            btnCaptureEvent.Name = "btnCaptureEvent";
            btnCaptureEvent.Size = new Size(209, 31);
            btnCaptureEvent.TabIndex = 0;
            btnCaptureEvent.Text = "Capture event";
            btnCaptureEvent.UseVisualStyleBackColor = true;
            btnCaptureEvent.Click += btnCaptureEvent_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cboCharacterName);
            groupBox2.Controls.Add(rtbOptions);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(lblEventName);
            groupBox2.Controls.Add(label1);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.Location = new Point(0, 116);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(14, 4, 14, 4);
            groupBox2.Size = new Size(437, 317);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Event selector";
            // 
            // cboCharacterName
            // 
            cboCharacterName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cboCharacterName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCharacterName.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboCharacterName.FormattingEnabled = true;
            cboCharacterName.Location = new Point(99, 57);
            cboCharacterName.Margin = new Padding(3, 4, 3, 4);
            cboCharacterName.Name = "cboCharacterName";
            cboCharacterName.Size = new Size(320, 28);
            cboCharacterName.TabIndex = 7;
            cboCharacterName.SelectedIndexChanged += cboCharacterName_SelectedIndexChanged;
            // 
            // rtbOptions
            // 
            rtbOptions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbOptions.Location = new Point(7, 93);
            rtbOptions.Name = "rtbOptions";
            rtbOptions.Size = new Size(413, 216);
            rtbOptions.TabIndex = 6;
            rtbOptions.Text = "";
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
            lblEventName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblEventName.AutoEllipsis = true;
            lblEventName.Location = new Point(99, 24);
            lblEventName.Name = "lblEventName";
            lblEventName.Padding = new Padding(0, 7, 0, 7);
            lblEventName.Size = new Size(309, 33);
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
            groupBox3.Controls.Add(rtbObjectives);
            groupBox3.Dock = DockStyle.Bottom;
            groupBox3.Location = new Point(0, 705);
            groupBox3.Margin = new Padding(3, 4, 3, 4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(14, 4, 14, 4);
            groupBox3.Size = new Size(437, 350);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Objectives";
            // 
            // rtbObjectives
            // 
            rtbObjectives.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbObjectives.Location = new Point(7, 27);
            rtbObjectives.Name = "rtbObjectives";
            rtbObjectives.Size = new Size(413, 317);
            rtbObjectives.TabIndex = 0;
            rtbObjectives.Text = "";
            // 
            // splitterObjectives
            // 
            splitterObjectives.Dock = DockStyle.Top;
            splitterObjectives.Location = new Point(0, 108);
            splitterObjectives.Margin = new Padding(3, 4, 3, 4);
            splitterObjectives.Name = "splitterObjectives";
            splitterObjectives.Size = new Size(437, 8);
            splitterObjectives.TabIndex = 4;
            splitterObjectives.TabStop = false;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(richTextBox1);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(0, 433);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(437, 272);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Races";
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Location = new Point(7, 26);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(413, 236);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Top;
            splitter1.Location = new Point(0, 433);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(437, 4);
            splitter1.TabIndex = 6;
            splitter1.TabStop = false;
            // 
            // splitter2
            // 
            splitter2.Dock = DockStyle.Bottom;
            splitter2.Location = new Point(0, 701);
            splitter2.Name = "splitter2";
            splitter2.Size = new Size(437, 4);
            splitter2.TabIndex = 7;
            splitter2.TabStop = false;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(437, 1055);
            Controls.Add(splitter2);
            Controls.Add(splitter1);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(splitterObjectives);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmMain";
            Text = "FrmMain";
            FormClosing += FrmMain_FormClosing;
            Load += FrmMain_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnCaptureEvent;
        private GroupBox groupBox2;
        private Label label1;
        private Label lblEventName;
        private Label label4;
        private GroupBox groupBox3;
        private RichTextBox rtbOptions;
        private RichTextBox rtbObjectives;
        private Button btnDownloadUmaData;
        private Splitter splitterObjectives;
        private ComboBox cboCharacterName;
        private Button btnCaptureDateTime;
        private GroupBox groupBox4;
        private Splitter splitter1;
        private Splitter splitter2;
        private RichTextBox richTextBox1;
    }
}