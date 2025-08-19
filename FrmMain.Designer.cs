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
            splitterOptions = new Splitter();
            splitterObjectives = new Splitter();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnDownloadUmaData);
            groupBox1.Controls.Add(btnCaptureEvent);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(12, 3, 12, 3);
            groupBox1.Size = new Size(384, 81);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Capture options";
            // 
            // btnDownloadUmaData
            // 
            btnDownloadUmaData.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            btnCaptureEvent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnCaptureEvent.Location = new Point(6, 22);
            btnCaptureEvent.Name = "btnCaptureEvent";
            btnCaptureEvent.Size = new Size(368, 23);
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
            groupBox2.Location = new Point(0, 87);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(12, 3, 12, 3);
            groupBox2.Size = new Size(384, 238);
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
            cboCharacterName.Size = new Size(282, 23);
            cboCharacterName.TabIndex = 7;
            cboCharacterName.SelectedIndexChanged += cboCharacterName_SelectedIndexChanged;
            // 
            // rtbOptions
            // 
            rtbOptions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbOptions.Location = new Point(6, 70);
            rtbOptions.Margin = new Padding(3, 2, 3, 2);
            rtbOptions.Name = "rtbOptions";
            rtbOptions.Size = new Size(364, 163);
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
            lblEventName.Size = new Size(272, 25);
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
            // groupBox3
            // 
            groupBox3.Controls.Add(rtbObjectives);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(0, 331);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(12, 3, 12, 3);
            groupBox3.Size = new Size(384, 460);
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
            rtbObjectives.Size = new Size(364, 436);
            rtbObjectives.TabIndex = 0;
            rtbObjectives.Text = "";
            // 
            // splitterOptions
            // 
            splitterOptions.Dock = DockStyle.Top;
            splitterOptions.Location = new Point(0, 325);
            splitterOptions.Name = "splitterOptions";
            splitterOptions.Size = new Size(384, 6);
            splitterOptions.TabIndex = 3;
            splitterOptions.TabStop = false;
            // 
            // splitterObjectives
            // 
            splitterObjectives.Dock = DockStyle.Top;
            splitterObjectives.Location = new Point(0, 81);
            splitterObjectives.Name = "splitterObjectives";
            splitterObjectives.Size = new Size(384, 6);
            splitterObjectives.TabIndex = 4;
            splitterObjectives.TabStop = false;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 791);
            Controls.Add(groupBox3);
            Controls.Add(splitterOptions);
            Controls.Add(groupBox2);
            Controls.Add(splitterObjectives);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
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
        private Button btnCaptureEvent;
        private GroupBox groupBox2;
        private Label label1;
        private Label lblEventName;
        private Label label4;
        private GroupBox groupBox3;
        private RichTextBox rtbOptions;
        private RichTextBox rtbObjectives;
        private Button btnDownloadUmaData;
        private Splitter splitterOptions;
        private Splitter splitterObjectives;
        private ComboBox cboCharacterName;
    }
}