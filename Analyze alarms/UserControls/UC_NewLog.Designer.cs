namespace Analyze_alarms
{
    partial class UC_NewLog
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dTP_From = new System.Windows.Forms.DateTimePicker();
            this.gb_Modify = new System.Windows.Forms.GroupBox();
            this.btn_Analyze = new System.Windows.Forms.Button();
            this.btn_Filter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dTP_To = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.gb_Data = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tp_Data = new System.Windows.Forms.TabPage();
            this.tp_Summary = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.tp_Diagram = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.gb_Modify.SuspendLayout();
            this.gb_Data.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tp_Data.SuspendLayout();
            this.tp_Summary.SuspendLayout();
            this.tp_Diagram.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 19);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(569, 354);
            this.dataGridView1.TabIndex = 1;
            // 
            // dTP_From
            // 
            this.dTP_From.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dTP_From.Location = new System.Drawing.Point(58, 19);
            this.dTP_From.Name = "dTP_From";
            this.dTP_From.Size = new System.Drawing.Size(167, 20);
            this.dTP_From.TabIndex = 2;
            this.dTP_From.ValueChanged += new System.EventHandler(this.dTP_From_ValueChanged);
            // 
            // gb_Modify
            // 
            this.gb_Modify.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_Modify.Controls.Add(this.btn_Analyze);
            this.gb_Modify.Controls.Add(this.btn_Filter);
            this.gb_Modify.Controls.Add(this.label1);
            this.gb_Modify.Controls.Add(this.dTP_To);
            this.gb_Modify.Controls.Add(this.label2);
            this.gb_Modify.Controls.Add(this.dTP_From);
            this.gb_Modify.Location = new System.Drawing.Point(6, 6);
            this.gb_Modify.Name = "gb_Modify";
            this.gb_Modify.Size = new System.Drawing.Size(581, 77);
            this.gb_Modify.TabIndex = 4;
            this.gb_Modify.TabStop = false;
            this.gb_Modify.Text = "Modify";
            // 
            // btn_Analyze
            // 
            this.btn_Analyze.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Analyze.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Analyze.Location = new System.Drawing.Point(473, 19);
            this.btn_Analyze.Name = "btn_Analyze";
            this.btn_Analyze.Size = new System.Drawing.Size(102, 46);
            this.btn_Analyze.TabIndex = 8;
            this.btn_Analyze.Text = "Analyze";
            this.btn_Analyze.UseVisualStyleBackColor = true;
            this.btn_Analyze.Click += new System.EventHandler(this.btn_Analyze_Click);
            // 
            // btn_Filter
            // 
            this.btn_Filter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Filter.Location = new System.Drawing.Point(231, 19);
            this.btn_Filter.Name = "btn_Filter";
            this.btn_Filter.Size = new System.Drawing.Size(62, 46);
            this.btn_Filter.TabIndex = 7;
            this.btn_Filter.Text = "Filter";
            this.btn_Filter.UseVisualStyleBackColor = true;
            this.btn_Filter.Click += new System.EventHandler(this.btn_Filter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(4, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "To:";
            // 
            // dTP_To
            // 
            this.dTP_To.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dTP_To.Location = new System.Drawing.Point(58, 45);
            this.dTP_To.Name = "dTP_To";
            this.dTP_To.Size = new System.Drawing.Size(167, 20);
            this.dTP_To.TabIndex = 5;
            this.dTP_To.ValueChanged += new System.EventHandler(this.dTP_To_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label2.Location = new System.Drawing.Point(4, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "From:";
            // 
            // gb_Data
            // 
            this.gb_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_Data.Controls.Add(this.dataGridView1);
            this.gb_Data.Location = new System.Drawing.Point(6, 89);
            this.gb_Data.Name = "gb_Data";
            this.gb_Data.Size = new System.Drawing.Size(581, 379);
            this.gb_Data.TabIndex = 5;
            this.gb_Data.TabStop = false;
            this.gb_Data.Text = "Data";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tp_Data);
            this.tabControl1.Controls.Add(this.tp_Summary);
            this.tabControl1.Controls.Add(this.tp_Diagram);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(601, 500);
            this.tabControl1.TabIndex = 6;
            // 
            // tp_Data
            // 
            this.tp_Data.Controls.Add(this.gb_Modify);
            this.tp_Data.Controls.Add(this.gb_Data);
            this.tp_Data.Location = new System.Drawing.Point(4, 22);
            this.tp_Data.Name = "tp_Data";
            this.tp_Data.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Data.Size = new System.Drawing.Size(593, 474);
            this.tp_Data.TabIndex = 0;
            this.tp_Data.Text = "Data";
            this.tp_Data.UseVisualStyleBackColor = true;
            // 
            // tp_Summary
            // 
            this.tp_Summary.Controls.Add(this.label3);
            this.tp_Summary.Location = new System.Drawing.Point(4, 22);
            this.tp_Summary.Name = "tp_Summary";
            this.tp_Summary.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Summary.Size = new System.Drawing.Size(593, 474);
            this.tp_Summary.TabIndex = 1;
            this.tp_Summary.Text = "Summary";
            this.tp_Summary.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(251, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "No analysis done.";
            // 
            // tp_Diagram
            // 
            this.tp_Diagram.Controls.Add(this.label4);
            this.tp_Diagram.Location = new System.Drawing.Point(4, 22);
            this.tp_Diagram.Name = "tp_Diagram";
            this.tp_Diagram.Size = new System.Drawing.Size(593, 474);
            this.tp_Diagram.TabIndex = 2;
            this.tp_Diagram.Text = "Diagram";
            this.tp_Diagram.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(251, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "No analysis done.";
            // 
            // UC_NewLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tabControl1);
            this.Name = "UC_NewLog";
            this.Size = new System.Drawing.Size(601, 500);
            this.Load += new System.EventHandler(this.UC_NewLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.gb_Modify.ResumeLayout(false);
            this.gb_Modify.PerformLayout();
            this.gb_Data.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tp_Data.ResumeLayout(false);
            this.tp_Summary.ResumeLayout(false);
            this.tp_Summary.PerformLayout();
            this.tp_Diagram.ResumeLayout(false);
            this.tp_Diagram.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DateTimePicker dTP_From;
        private System.Windows.Forms.GroupBox gb_Modify;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dTP_To;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox gb_Data;
        private System.Windows.Forms.Button btn_Filter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tp_Data;
        private System.Windows.Forms.TabPage tp_Summary;
        private System.Windows.Forms.TabPage tp_Diagram;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Analyze;
    }
}
