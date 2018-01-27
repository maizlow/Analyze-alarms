namespace Analyze_alarms
{
    partial class Settings_Form
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
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Apply = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.brn_Help = new System.Windows.Forms.Button();
            this.ExistingClassesLV = new System.Windows.Forms.ListView();
            this.gb_ExistingClasses = new System.Windows.Forms.GroupBox();
            this.btn_AddNew = new System.Windows.Forms.Button();
            this.btn_EditSelected = new System.Windows.Forms.Button();
            this.gb_EditAdd = new System.Windows.Forms.GroupBox();
            this.p_LogBitSelect = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.btn_Discard = new System.Windows.Forms.Button();
            this.cb_SubClassMember = new System.Windows.Forms.ComboBox();
            this.lbl_SubClassMember = new System.Windows.Forms.Label();
            this.lbl_MsgNr = new System.Windows.Forms.Label();
            this.tb_MsgNr = new System.Windows.Forms.TextBox();
            this.cb_ClassType = new System.Windows.Forms.ComboBox();
            this.lbl_ClassType = new System.Windows.Forms.Label();
            this.tb_ClassNr = new System.Windows.Forms.TextBox();
            this.lbl_ClassNumber = new System.Windows.Forms.Label();
            this.tb_ClassName = new System.Windows.Forms.TextBox();
            this.lbl_ClassName = new System.Windows.Forms.Label();
            this.btn_DeleteSelected = new System.Windows.Forms.Button();
            this.gb_ExistingClasses.SuspendLayout();
            this.gb_EditAdd.SuspendLayout();
            this.p_LogBitSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(473, 389);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 1;
            this.btn_OK.Text = "Ok";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Apply
            // 
            this.btn_Apply.Enabled = false;
            this.btn_Apply.Location = new System.Drawing.Point(309, 89);
            this.btn_Apply.Name = "btn_Apply";
            this.btn_Apply.Size = new System.Drawing.Size(108, 21);
            this.btn_Apply.TabIndex = 2;
            this.btn_Apply.Text = "Apply";
            this.btn_Apply.UseVisualStyleBackColor = true;
            this.btn_Apply.Click += new System.EventHandler(this.btn_Apply_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(12, 389);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // brn_Help
            // 
            this.brn_Help.Location = new System.Drawing.Point(93, 389);
            this.brn_Help.Name = "brn_Help";
            this.brn_Help.Size = new System.Drawing.Size(75, 23);
            this.brn_Help.TabIndex = 4;
            this.brn_Help.Text = "Help";
            this.brn_Help.UseVisualStyleBackColor = true;
            this.brn_Help.Click += new System.EventHandler(this.brn_Help_Click);
            // 
            // ExistingClassesLV
            // 
            this.ExistingClassesLV.Location = new System.Drawing.Point(6, 19);
            this.ExistingClassesLV.Name = "ExistingClassesLV";
            this.ExistingClassesLV.Size = new System.Drawing.Size(524, 185);
            this.ExistingClassesLV.TabIndex = 0;
            this.ExistingClassesLV.UseCompatibleStateImageBehavior = false;
            this.ExistingClassesLV.View = System.Windows.Forms.View.Details;
            this.ExistingClassesLV.SelectedIndexChanged += new System.EventHandler(this.ExistingClassesLV_SelectedIndexChanged);
            // 
            // gb_ExistingClasses
            // 
            this.gb_ExistingClasses.Controls.Add(this.btn_DeleteSelected);
            this.gb_ExistingClasses.Controls.Add(this.btn_AddNew);
            this.gb_ExistingClasses.Controls.Add(this.btn_EditSelected);
            this.gb_ExistingClasses.Controls.Add(this.ExistingClassesLV);
            this.gb_ExistingClasses.Location = new System.Drawing.Point(12, 12);
            this.gb_ExistingClasses.Name = "gb_ExistingClasses";
            this.gb_ExistingClasses.Size = new System.Drawing.Size(536, 235);
            this.gb_ExistingClasses.TabIndex = 5;
            this.gb_ExistingClasses.TabStop = false;
            this.gb_ExistingClasses.Text = "Existing classes";
            // 
            // btn_AddNew
            // 
            this.btn_AddNew.Location = new System.Drawing.Point(455, 206);
            this.btn_AddNew.Name = "btn_AddNew";
            this.btn_AddNew.Size = new System.Drawing.Size(75, 23);
            this.btn_AddNew.TabIndex = 7;
            this.btn_AddNew.Text = "Add...";
            this.btn_AddNew.UseVisualStyleBackColor = true;
            this.btn_AddNew.Click += new System.EventHandler(this.btn_AddNew_Click);
            // 
            // btn_EditSelected
            // 
            this.btn_EditSelected.Enabled = false;
            this.btn_EditSelected.Location = new System.Drawing.Point(346, 206);
            this.btn_EditSelected.Name = "btn_EditSelected";
            this.btn_EditSelected.Size = new System.Drawing.Size(103, 23);
            this.btn_EditSelected.TabIndex = 6;
            this.btn_EditSelected.Text = "Edit selected";
            this.btn_EditSelected.UseVisualStyleBackColor = true;
            this.btn_EditSelected.Click += new System.EventHandler(this.btn_EditSelected_Click);
            // 
            // gb_EditAdd
            // 
            this.gb_EditAdd.Controls.Add(this.p_LogBitSelect);
            this.gb_EditAdd.Controls.Add(this.btn_Discard);
            this.gb_EditAdd.Controls.Add(this.cb_SubClassMember);
            this.gb_EditAdd.Controls.Add(this.lbl_SubClassMember);
            this.gb_EditAdd.Controls.Add(this.lbl_MsgNr);
            this.gb_EditAdd.Controls.Add(this.tb_MsgNr);
            this.gb_EditAdd.Controls.Add(this.btn_Apply);
            this.gb_EditAdd.Controls.Add(this.cb_ClassType);
            this.gb_EditAdd.Controls.Add(this.lbl_ClassType);
            this.gb_EditAdd.Controls.Add(this.tb_ClassNr);
            this.gb_EditAdd.Controls.Add(this.lbl_ClassNumber);
            this.gb_EditAdd.Controls.Add(this.tb_ClassName);
            this.gb_EditAdd.Controls.Add(this.lbl_ClassName);
            this.gb_EditAdd.Location = new System.Drawing.Point(12, 254);
            this.gb_EditAdd.Name = "gb_EditAdd";
            this.gb_EditAdd.Size = new System.Drawing.Size(536, 129);
            this.gb_EditAdd.TabIndex = 6;
            this.gb_EditAdd.TabStop = false;
            this.gb_EditAdd.Text = "Edit/Add";
            this.gb_EditAdd.Visible = false;
            // 
            // p_LogBitSelect
            // 
            this.p_LogBitSelect.Controls.Add(this.label1);
            this.p_LogBitSelect.Controls.Add(this.radioButton2);
            this.p_LogBitSelect.Controls.Add(this.radioButton1);
            this.p_LogBitSelect.Location = new System.Drawing.Point(6, 63);
            this.p_LogBitSelect.Name = "p_LogBitSelect";
            this.p_LogBitSelect.Size = new System.Drawing.Size(128, 60);
            this.p_LogBitSelect.TabIndex = 11;
            this.p_LogBitSelect.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Logbit type";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(4, 42);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(78, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Shift active";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(4, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(114, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Production running";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // btn_Discard
            // 
            this.btn_Discard.Enabled = false;
            this.btn_Discard.Location = new System.Drawing.Point(423, 89);
            this.btn_Discard.Name = "btn_Discard";
            this.btn_Discard.Size = new System.Drawing.Size(107, 21);
            this.btn_Discard.TabIndex = 10;
            this.btn_Discard.Text = "Discard";
            this.btn_Discard.UseVisualStyleBackColor = true;
            this.btn_Discard.Click += new System.EventHandler(this.btn_Discard_Click);
            // 
            // cb_SubClassMember
            // 
            this.cb_SubClassMember.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SubClassMember.FormattingEnabled = true;
            this.cb_SubClassMember.Location = new System.Drawing.Point(6, 89);
            this.cb_SubClassMember.Name = "cb_SubClassMember";
            this.cb_SubClassMember.Size = new System.Drawing.Size(221, 21);
            this.cb_SubClassMember.TabIndex = 9;
            this.cb_SubClassMember.Visible = false;
            this.cb_SubClassMember.SelectedIndexChanged += new System.EventHandler(this.cb_SubClassMember_SelectedIndexChanged);
            // 
            // lbl_SubClassMember
            // 
            this.lbl_SubClassMember.AutoSize = true;
            this.lbl_SubClassMember.Location = new System.Drawing.Point(3, 73);
            this.lbl_SubClassMember.Name = "lbl_SubClassMember";
            this.lbl_SubClassMember.Size = new System.Drawing.Size(93, 13);
            this.lbl_SubClassMember.TabIndex = 8;
            this.lbl_SubClassMember.Text = "Sub class member";
            this.lbl_SubClassMember.Visible = false;
            // 
            // lbl_MsgNr
            // 
            this.lbl_MsgNr.AutoSize = true;
            this.lbl_MsgNr.Location = new System.Drawing.Point(442, 20);
            this.lbl_MsgNr.Name = "lbl_MsgNr";
            this.lbl_MsgNr.Size = new System.Drawing.Size(88, 13);
            this.lbl_MsgNr.TabIndex = 7;
            this.lbl_MsgNr.Text = "Message number";
            this.lbl_MsgNr.Visible = false;
            // 
            // tb_MsgNr
            // 
            this.tb_MsgNr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.tb_MsgNr.Location = new System.Drawing.Point(442, 36);
            this.tb_MsgNr.MaxLength = 5;
            this.tb_MsgNr.Name = "tb_MsgNr";
            this.tb_MsgNr.Size = new System.Drawing.Size(88, 21);
            this.tb_MsgNr.TabIndex = 6;
            this.tb_MsgNr.Visible = false;
            this.tb_MsgNr.TextChanged += new System.EventHandler(this.tb_MsgNr_TextChanged);
            // 
            // cb_ClassType
            // 
            this.cb_ClassType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ClassType.FormattingEnabled = true;
            this.cb_ClassType.Location = new System.Drawing.Point(309, 36);
            this.cb_ClassType.Name = "cb_ClassType";
            this.cb_ClassType.Size = new System.Drawing.Size(127, 21);
            this.cb_ClassType.TabIndex = 5;
            this.cb_ClassType.SelectedIndexChanged += new System.EventHandler(this.cb_ClassType_SelectedIndexChanged);
            // 
            // lbl_ClassType
            // 
            this.lbl_ClassType.AutoSize = true;
            this.lbl_ClassType.Location = new System.Drawing.Point(306, 20);
            this.lbl_ClassType.Name = "lbl_ClassType";
            this.lbl_ClassType.Size = new System.Drawing.Size(55, 13);
            this.lbl_ClassType.TabIndex = 4;
            this.lbl_ClassType.Text = "Class type";
            // 
            // tb_ClassNr
            // 
            this.tb_ClassNr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.tb_ClassNr.Location = new System.Drawing.Point(233, 36);
            this.tb_ClassNr.MaxLength = 3;
            this.tb_ClassNr.Name = "tb_ClassNr";
            this.tb_ClassNr.Size = new System.Drawing.Size(70, 21);
            this.tb_ClassNr.TabIndex = 3;
            this.tb_ClassNr.TextChanged += new System.EventHandler(this.tb_ClassNr_TextChanged);
            // 
            // lbl_ClassNumber
            // 
            this.lbl_ClassNumber.AutoSize = true;
            this.lbl_ClassNumber.Location = new System.Drawing.Point(230, 20);
            this.lbl_ClassNumber.Name = "lbl_ClassNumber";
            this.lbl_ClassNumber.Size = new System.Drawing.Size(70, 13);
            this.lbl_ClassNumber.TabIndex = 2;
            this.lbl_ClassNumber.Text = "Class number";
            // 
            // tb_ClassName
            // 
            this.tb_ClassName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.tb_ClassName.Location = new System.Drawing.Point(6, 36);
            this.tb_ClassName.Name = "tb_ClassName";
            this.tb_ClassName.Size = new System.Drawing.Size(221, 21);
            this.tb_ClassName.TabIndex = 1;
            this.tb_ClassName.TextChanged += new System.EventHandler(this.tb_ClassName_TextChanged);
            // 
            // lbl_ClassName
            // 
            this.lbl_ClassName.AutoSize = true;
            this.lbl_ClassName.Location = new System.Drawing.Point(6, 20);
            this.lbl_ClassName.Name = "lbl_ClassName";
            this.lbl_ClassName.Size = new System.Drawing.Size(61, 13);
            this.lbl_ClassName.TabIndex = 0;
            this.lbl_ClassName.Text = "Class name";
            // 
            // btn_DeleteSelected
            // 
            this.btn_DeleteSelected.Enabled = false;
            this.btn_DeleteSelected.Location = new System.Drawing.Point(242, 206);
            this.btn_DeleteSelected.Name = "btn_DeleteSelected";
            this.btn_DeleteSelected.Size = new System.Drawing.Size(98, 23);
            this.btn_DeleteSelected.TabIndex = 8;
            this.btn_DeleteSelected.Text = "Delete selected";
            this.btn_DeleteSelected.UseVisualStyleBackColor = true;
            this.btn_DeleteSelected.Click += new System.EventHandler(this.btn_DeleteSelected_Click);
            // 
            // Settings_Form
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(560, 420);
            this.Controls.Add(this.gb_EditAdd);
            this.Controls.Add(this.gb_ExistingClasses);
            this.Controls.Add(this.brn_Help);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings_Form";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Log settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Settings_Form_Load);
            this.gb_ExistingClasses.ResumeLayout(false);
            this.gb_EditAdd.ResumeLayout(false);
            this.gb_EditAdd.PerformLayout();
            this.p_LogBitSelect.ResumeLayout(false);
            this.p_LogBitSelect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Apply;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button brn_Help;
        private System.Windows.Forms.ListView ExistingClassesLV;
        private System.Windows.Forms.GroupBox gb_ExistingClasses;
        private System.Windows.Forms.Button btn_AddNew;
        private System.Windows.Forms.Button btn_EditSelected;
        private System.Windows.Forms.GroupBox gb_EditAdd;
        private System.Windows.Forms.TextBox tb_ClassNr;
        private System.Windows.Forms.Label lbl_ClassNumber;
        private System.Windows.Forms.TextBox tb_ClassName;
        private System.Windows.Forms.Label lbl_ClassName;
        private System.Windows.Forms.ComboBox cb_ClassType;
        private System.Windows.Forms.Label lbl_ClassType;
        private System.Windows.Forms.ComboBox cb_SubClassMember;
        private System.Windows.Forms.Label lbl_SubClassMember;
        private System.Windows.Forms.Label lbl_MsgNr;
        private System.Windows.Forms.TextBox tb_MsgNr;
        private System.Windows.Forms.Button btn_Discard;
        private System.Windows.Forms.Panel p_LogBitSelect;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_DeleteSelected;
    }
}