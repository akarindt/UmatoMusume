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
            splitter1 = new Splitter();
            groupBox2 = new GroupBox();
            cboCharacterName = new ComboBox();
            rtbOptions = new RichTextBox();
            label4 = new Label();
            lblEventName = new Label();
            label1 = new Label();
            splitter2 = new Splitter();
            groupBox4 = new GroupBox();
            pDistantTypeCheckboxes = new FlowLayoutPanel();
            pTerrainCheckboxes = new FlowLayoutPanel();
            pGradeCheckboxes = new FlowLayoutPanel();
            lblDate = new Label();
            label3 = new Label();
            rtbRaces = new RichTextBox();
            splitter3 = new Splitter();
            groupBox3 = new GroupBox();
            rtbObjectives = new RichTextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnCaptureDateTime);
            groupBox1.Controls.Add(btnDownloadUmaData);
            groupBox1.Controls.Add(btnCaptureEvent);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(12, 3, 12, 3);
            groupBox1.Size = new Size(382, 81);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Capture options";
            // 
            // btnCaptureDateTime
            // 
            btnCaptureDateTime.Location = new Point(194, 22);
            btnCaptureDateTime.Margin = new Padding(3, 2, 3, 2);
            btnCaptureDateTime.Name = "btnCaptureDateTime";
            btnCaptureDateTime.Size = new Size(180, 24);
            btnCaptureDateTime.TabIndex = 3;
            btnCaptureDateTime.Text = "Capture date/time";
            btnCaptureDateTime.UseVisualStyleBackColor = true;
            btnCaptureDateTime.Click += btnCaptureDateTime_Click;
            // 
            // btnDownloadUmaData
            // 
            btnDownloadUmaData.Location = new Point(6, 50);
            btnDownloadUmaData.Margin = new Padding(3, 2, 3, 2);
            btnDownloadUmaData.Name = "btnDownloadUmaData";
            btnDownloadUmaData.Size = new Size(368, 22);
            btnDownloadUmaData.TabIndex = 2;
            btnDownloadUmaData.Text = "Download data";
            btnDownloadUmaData.UseVisualStyleBackColor = true;
            btnDownloadUmaData.Click += btnDownloadUmaData_Click;
            // 
            // btnCaptureEvent
            // 
            btnCaptureEvent.Location = new Point(6, 22);
            btnCaptureEvent.Name = "btnCaptureEvent";
            btnCaptureEvent.Size = new Size(183, 23);
            btnCaptureEvent.TabIndex = 0;
            btnCaptureEvent.Text = "Capture event";
            btnCaptureEvent.UseVisualStyleBackColor = true;
            btnCaptureEvent.Click += btnCaptureEvent_Click;
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Top;
            splitter1.Location = new Point(0, 81);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(382, 3);
            splitter1.TabIndex = 6;
            splitter1.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cboCharacterName);
            groupBox2.Controls.Add(rtbOptions);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(lblEventName);
            groupBox2.Controls.Add(label1);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.Location = new Point(0, 84);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(12, 3, 12, 3);
            groupBox2.Size = new Size(382, 238);
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
            cboCharacterName.Location = new Point(87, 43);
            cboCharacterName.Name = "cboCharacterName";
            cboCharacterName.Size = new Size(280, 23);
            cboCharacterName.TabIndex = 7;
            cboCharacterName.SelectedIndexChanged += cboCharacterName_SelectedIndexChanged;
            // 
            // rtbOptions
            // 
            rtbOptions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbOptions.Location = new Point(6, 70);
            rtbOptions.Margin = new Padding(3, 2, 3, 2);
            rtbOptions.Name = "rtbOptions";
            rtbOptions.Size = new Size(362, 163);
            rtbOptions.TabIndex = 6;
            rtbOptions.Text = "";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 43);
            label4.Name = "label4";
            label4.Padding = new Padding(0, 5, 0, 5);
            label4.Size = new Size(59, 25);
            label4.TabIndex = 4;
            label4.Text = "Char info:";
            // 
            // lblEventName
            // 
            lblEventName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblEventName.AutoEllipsis = true;
            lblEventName.Location = new Point(87, 18);
            lblEventName.Name = "lblEventName";
            lblEventName.Padding = new Padding(0, 5, 0, 5);
            lblEventName.Size = new Size(270, 25);
            lblEventName.TabIndex = 3;
            lblEventName.Text = "Capturing...";
            lblEventName.TextChanged += lblEventName_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 18);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 5, 0, 5);
            label1.Size = new Size(75, 25);
            label1.TabIndex = 0;
            label1.Text = "Event name: ";
            // 
            // splitter2
            // 
            splitter2.Dock = DockStyle.Top;
            splitter2.Location = new Point(0, 322);
            splitter2.Name = "splitter2";
            splitter2.Size = new Size(382, 3);
            splitter2.TabIndex = 7;
            splitter2.TabStop = false;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(pDistantTypeCheckboxes);
            groupBox4.Controls.Add(pTerrainCheckboxes);
            groupBox4.Controls.Add(pGradeCheckboxes);
            groupBox4.Controls.Add(lblDate);
            groupBox4.Controls.Add(label3);
            groupBox4.Controls.Add(rtbRaces);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(0, 325);
            groupBox4.Margin = new Padding(3, 2, 3, 2);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(3, 2, 3, 2);
            groupBox4.Size = new Size(382, 298);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Races";
            // 
            // pDistantTypeCheckboxes
            // 
            pDistantTypeCheckboxes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pDistantTypeCheckboxes.AutoScroll = true;
            pDistantTypeCheckboxes.Location = new Point(87, 128);
            pDistantTypeCheckboxes.Name = "pDistantTypeCheckboxes";
            pDistantTypeCheckboxes.Size = new Size(281, 31);
            pDistantTypeCheckboxes.TabIndex = 8;
            pDistantTypeCheckboxes.WrapContents = false;
            // 
            // pTerrainCheckboxes
            // 
            pTerrainCheckboxes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pTerrainCheckboxes.AutoScroll = true;
            pTerrainCheckboxes.Location = new Point(87, 94);
            pTerrainCheckboxes.Name = "pTerrainCheckboxes";
            pTerrainCheckboxes.Size = new Size(281, 28);
            pTerrainCheckboxes.TabIndex = 7;
            pTerrainCheckboxes.WrapContents = false;
            // 
            // pGradeCheckboxes
            // 
            pGradeCheckboxes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pGradeCheckboxes.AutoScroll = true;
            pGradeCheckboxes.Location = new Point(87, 46);
            pGradeCheckboxes.Name = "pGradeCheckboxes";
            pGradeCheckboxes.Size = new Size(280, 42);
            pGradeCheckboxes.TabIndex = 6;
            pGradeCheckboxes.WrapContents = false;
            // 
            // lblDate
            // 
            lblDate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDate.AutoEllipsis = true;
            lblDate.Location = new Point(87, 18);
            lblDate.Name = "lblDate";
            lblDate.Padding = new Padding(0, 5, 0, 5);
            lblDate.Size = new Size(270, 25);
            lblDate.TabIndex = 5;
            lblDate.Text = "Capturing...";
            lblDate.TextChanged += lblDate_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 18);
            label3.Name = "label3";
            label3.Padding = new Padding(0, 5, 0, 5);
            label3.Size = new Size(37, 25);
            label3.TabIndex = 4;
            label3.Text = "Date: ";
            // 
            // rtbRaces
            // 
            rtbRaces.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbRaces.Location = new Point(6, 164);
            rtbRaces.Margin = new Padding(3, 2, 3, 2);
            rtbRaces.Name = "rtbRaces";
            rtbRaces.Size = new Size(362, 128);
            rtbRaces.TabIndex = 0;
            rtbRaces.Text = "";
            // 
            // splitter3
            // 
            splitter3.Dock = DockStyle.Bottom;
            splitter3.Location = new Point(0, 623);
            splitter3.Name = "splitter3";
            splitter3.Size = new Size(382, 3);
            splitter3.TabIndex = 8;
            splitter3.TabStop = false;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(rtbObjectives);
            groupBox3.Dock = DockStyle.Bottom;
            groupBox3.Location = new Point(0, 626);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(12, 3, 12, 3);
            groupBox3.Size = new Size(382, 165);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Objectives";
            // 
            // rtbObjectives
            // 
            rtbObjectives.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbObjectives.Location = new Point(6, 20);
            rtbObjectives.Margin = new Padding(3, 2, 3, 2);
            rtbObjectives.Name = "rtbObjectives";
            rtbObjectives.Size = new Size(362, 142);
            rtbObjectives.TabIndex = 0;
            rtbObjectives.Text = "";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(382, 791);
            Controls.Add(groupBox4);
            Controls.Add(splitter3);
            Controls.Add(splitter2);
            Controls.Add(groupBox2);
            Controls.Add(splitter1);
            Controls.Add(groupBox1);
            Controls.Add(groupBox3);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmMain";
            Text = "FrmMain";
            FormClosing += FrmMain_FormClosing;
            Load += FrmMain_Load;
            ResizeEnd += FrmMain_ResizeEnd;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
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
        private ComboBox cboCharacterName;
        private Button btnCaptureDateTime;
        private GroupBox groupBox4;
        private RichTextBox rtbRaces;
        private Splitter splitter1;
        private Splitter splitter2;
        private Splitter splitter3;
        private Label lblDate;
        private Label label3;
        private FlowLayoutPanel pGradeCheckboxes;
        private FlowLayoutPanel pDistantTypeCheckboxes;
        private FlowLayoutPanel pTerrainCheckboxes;
    }
}