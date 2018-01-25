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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dTP_To = new System.Windows.Forms.DateTimePicker();
            this.gb_Data = new System.Windows.Forms.GroupBox();
            this.btn_Filter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.gb_Modify.SuspendLayout();
            this.gb_Data.SuspendLayout();
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
            this.dataGridView1.Size = new System.Drawing.Size(640, 371);
            this.dataGridView1.TabIndex = 1;
            // 
            // dTP_From
            // 
            this.dTP_From.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dTP_From.Location = new System.Drawing.Point(58, 19);
            this.dTP_From.Name = "dTP_From";
            this.dTP_From.Size = new System.Drawing.Size(112, 20);
            this.dTP_From.TabIndex = 2;
            this.dTP_From.ValueChanged += new System.EventHandler(this.dTP_From_ValueChanged);
            // 
            // gb_Modify
            // 
            this.gb_Modify.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_Modify.Controls.Add(this.btn_Filter);
            this.gb_Modify.Controls.Add(this.label1);
            this.gb_Modify.Controls.Add(this.dTP_To);
            this.gb_Modify.Controls.Add(this.label2);
            this.gb_Modify.Controls.Add(this.dTP_From);
            this.gb_Modify.Location = new System.Drawing.Point(3, 3);
            this.gb_Modify.Name = "gb_Modify";
            this.gb_Modify.Size = new System.Drawing.Size(652, 89);
            this.gb_Modify.TabIndex = 4;
            this.gb_Modify.TabStop = false;
            this.gb_Modify.Text = "Modify";
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
            this.dTP_To.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dTP_To.Location = new System.Drawing.Point(58, 45);
            this.dTP_To.Name = "dTP_To";
            this.dTP_To.Size = new System.Drawing.Size(112, 20);
            this.dTP_To.TabIndex = 5;
            this.dTP_To.ValueChanged += new System.EventHandler(this.dTP_To_ValueChanged);
            // 
            // gb_Data
            // 
            this.gb_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_Data.Controls.Add(this.dataGridView1);
            this.gb_Data.Location = new System.Drawing.Point(3, 98);
            this.gb_Data.Name = "gb_Data";
            this.gb_Data.Size = new System.Drawing.Size(652, 396);
            this.gb_Data.TabIndex = 5;
            this.gb_Data.TabStop = false;
            this.gb_Data.Text = "Data";
            // 
            // btn_Filter
            // 
            this.btn_Filter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Filter.Location = new System.Drawing.Point(177, 19);
            this.btn_Filter.Name = "btn_Filter";
            this.btn_Filter.Size = new System.Drawing.Size(62, 46);
            this.btn_Filter.TabIndex = 7;
            this.btn_Filter.Text = "Filter";
            this.btn_Filter.UseVisualStyleBackColor = true;
            this.btn_Filter.Click += new System.EventHandler(this.btn_Filter_Click);
            // 
            // UC_NewLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.gb_Data);
            this.Controls.Add(this.gb_Modify);
            this.Name = "UC_NewLog";
            this.Size = new System.Drawing.Size(658, 497);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.gb_Modify.ResumeLayout(false);
            this.gb_Modify.PerformLayout();
            this.gb_Data.ResumeLayout(false);
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
    }
}
