using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Analyze_alarms
{
    public partial class Settings_Form : Form
    {
        /*
        Class types are as following:
        1 Logging               {req. messageNr}
        2 Direct
        3 Indirect              {req. messageNr, subClassMember}
        4 Indirect subclass
        */

        private List<LogSettings> localListOfSettings; //My local list to pass on DialogResult = OK
        private LogSettings selectedSetting; //Placeholder for select function
        private LogSettings updatedSetting; //To edit existing
        private LogSettings newSetting; //To add
        private bool isEditing; //Is editing an entry
        private bool fillingInfoForEdit; //Skip event code
        private int isEditingIndexInLv = -1; //Active item in list of LogSettings that we are editing


        public Settings_Form()
        {
            InitializeComponent();
        }

        private void Settings_Form_Load(object sender, EventArgs e)
        {
            this.Icon = new Icon(System.Environment.CurrentDirectory + "\\logo.ico");
            localListOfSettings = MainForm.logSettings;
            InitListView();
            InitComboBoxes();
        }

        #region BUTTONS
        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            //Save settings. Write to XML is done in MainForm.cs
            MainForm.logSettings = localListOfSettings;

            this.Close();
        }

        private void btn_EditSelected_Click(object sender, EventArgs e)
        {
            if ( selectedSetting != null )
            {
                isEditing = true;
                btn_EditSelected.Enabled = false;
                btn_AddNew.Enabled = false;
                btn_Discard.Enabled = true;
                fillingInfoForEdit = true;
                ExistingClassesLV.Enabled = false;

                updatedSetting = selectedSetting;

                tb_ClassName.Text = selectedSetting.className;
                tb_ClassNr.Text = selectedSetting.classNr.ToString();
                cb_ClassType.SelectedIndex = selectedSetting.classType - 1;
                tb_MsgNr.Text = selectedSetting.messageNr.ToString();

                foreach(LogSettings ls in localListOfSettings)
                {
                    if (ls.classNr == selectedSetting.subClassMember )
                    {
                        foreach (var item in cb_SubClassMember.Items)
                        {
                            if (item.ToString() == ls.className)
                                cb_SubClassMember.SelectedItem = item;
                        }
                    }
                }
                
                if (selectedSetting.isProdActiveLogBit) radioButton1.Checked = true;
                else if (selectedSetting.isShiftActiveLogBit) radioButton2.Checked = true;
                else
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;
                }
                

                if (cb_ClassType.SelectedIndex == 0)
                {
                    MsgNrVisible(true);
                    p_LogBitSelect.Visible = true;
                }
                else if (cb_ClassType.SelectedIndex == 2)
                {
                    MsgNrVisible(true);
                    SubClassMemberVisible(true);
                    p_LogBitSelect.Visible = false;
                }
                else
                {
                    MsgNrVisible(false);
                    SubClassMemberVisible(false);
                    p_LogBitSelect.Visible = false;
                }
                gb_EditAdd.Visible = true;


                //Prevent some event from firering
                fillingInfoForEdit = false;
            }
        }

        private void btn_DeleteSelected_Click(object sender, EventArgs e)
        {
            if (ExistingClassesLV.SelectedIndices != null)
            {
                localListOfSettings.Remove(selectedSetting);
                btn_DeleteSelected.Enabled = false;
                InitListView();
            }

        }

        private void btn_AddNew_Click(object sender, EventArgs e)
        {
            fillingInfoForEdit = true;

            ExistingClassesLV.SelectedIndices.Clear();
            ExistingClassesLV.Enabled = false;

            newSetting = new LogSettings();

            isEditing = false;
            btn_EditSelected.Enabled = false;
            btn_AddNew.Enabled = false;
            btn_Discard.Enabled = true;
            tb_MsgNr.Visible = false;
            cb_SubClassMember.Visible = false;

            tb_ClassName.Text = "";
            tb_ClassNr.Text = "0";
            cb_ClassType.SelectedIndex = -1;
            tb_MsgNr.Text = "0";
            cb_SubClassMember.SelectedIndex = -1;
            radioButton1.Checked = false;
            radioButton2.Checked = false;

            updatedSetting = null;
            selectedSetting = null;
            

            gb_EditAdd.Visible = true;

            //Prevent some event from firering
            fillingInfoForEdit = false;
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            //Fill List<LogSettings> index with the new settings.

            //Update existing
            if (isEditing)
            {
                localListOfSettings[isEditingIndexInLv] = updatedSetting;
                InitListView();
                InitComboBoxes();                
            }
            //Add new List item
            else
            {
                if (!DuplicatesInListView())
                {
                    localListOfSettings.Add(newSetting);
                    InitListView();
                    InitComboBoxes();
                    btn_AddNew.Enabled = true;
                    ExistingClassesLV.Enabled = true;
                }
                else
                {
                    return;
                }
            }

            isEditingIndexInLv = -1;
            isEditing = false;
            selectedSetting = null;
            newSetting = null;
            updatedSetting = null;
            ExistingClassesLV.Enabled = true;
            gb_EditAdd.Visible = false;
            p_LogBitSelect.Visible = false;
            MsgNrVisible(false);
            SubClassMemberVisible(false);
        }

        private void btn_Discard_Click(object sender, EventArgs e)
        {
            isEditingIndexInLv = -1;
            isEditing = false;
            selectedSetting = null;
            ExistingClassesLV.Enabled = true;
            gb_EditAdd.Visible = false;
            btn_AddNew.Enabled = true;
        }

        private void brn_Help_Click(object sender, EventArgs e)
        {
            HELP_LogSettings helpWindow = new HELP_LogSettings();
            helpWindow.Show();
        }
        #endregion

        #region INPUTS
        private void ExistingClassesLV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ExistingClassesLV.SelectedIndices.Count != 0)
            {
                selectedSetting = localListOfSettings[ExistingClassesLV.SelectedIndices[0]];
                if (selectedSetting.className == "")
                {
                    selectedSetting = null;
                }
                else
                {
                    isEditingIndexInLv = ExistingClassesLV.SelectedIndices[0];
                    btn_EditSelected.Enabled = true;
                    btn_DeleteSelected.Enabled = true;
                }
            }


        }

        private void tb_ClassName_TextChanged(object sender, EventArgs e)
        {
            //Editing existing
            if (isEditing)
            {
                updatedSetting.className = tb_ClassName.Text;
            }
            //Adding new
            else
            {
                newSetting.className = tb_ClassName.Text;
            }

            CheckIfAllowApply();
        }

        private void tb_ClassNr_TextChanged(object sender, EventArgs e)
        {
            if (!IsNumeric(tb_ClassNr.Text)) 
            {
                tb_ClassNr.Text = "0";
                btn_Apply.Enabled = false;
                return;
            }

            //Editing existing
            if (isEditing)
            {
                updatedSetting.classNr = int.Parse(tb_ClassNr.Text);
            }
            //Adding new
            else
            {
                newSetting.classNr = int.Parse(tb_ClassNr.Text);
            }
            CheckIfAllowApply();
        }

        private void cb_ClassType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_ClassType.SelectedIndex == 0)
            {
                MsgNrVisible(true);
                p_LogBitSelect.Visible = true;
                cb_SubClassMember.Visible = false;
            }
            else if (cb_ClassType.SelectedIndex == 2)
            {
                MsgNrVisible(true);
                p_LogBitSelect.Visible = false;
                SubClassMemberVisible(true);
            }
            else
            {
                p_LogBitSelect.Visible = false;
                MsgNrVisible(false);
                SubClassMemberVisible(false);
            }

            //Editing existing
            if (isEditing)
            {
                updatedSetting.classType = cb_ClassType.SelectedIndex + 1;

            }
            //Adding new
            else
            {
                newSetting.classType = cb_ClassType.SelectedIndex + 1;
            }

            CheckIfAllowApply();
        }

        private void tb_MsgNr_TextChanged(object sender, EventArgs e)
        {
            if (!IsNumeric(tb_MsgNr.Text))
            {
                tb_MsgNr.Text = "0";
                btn_Apply.Enabled = false;
                return;
            }

            //Editing existing
            if (isEditing)
            {
                updatedSetting.messageNr = int.Parse(tb_MsgNr.Text);
            }
            //Adding new
            else
            {
                newSetting.messageNr = int.Parse(tb_MsgNr.Text);
            }

            CheckIfAllowApply();
        }

        private void cb_SubClassMember_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fillingInfoForEdit) return;
            //Editing existing
            if (isEditing)
            {
                foreach (LogSettings s in localListOfSettings)
                {
                    if (s.className == cb_SubClassMember.SelectedItem.ToString())
                        updatedSetting.subClassMember = s.classNr;
                }
            }
            //Adding new
            else
            {
                foreach (LogSettings s in localListOfSettings)
                {
                    if (s.className == cb_SubClassMember.SelectedItem.ToString())
                        newSetting.subClassMember = s.classNr;
                }
            }

            CheckIfAllowApply();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!fillingInfoForEdit)
            {
                if (radioButton1.Checked)
                {
                    if (isEditing)
                    {
                        updatedSetting.isProdActiveLogBit = true;
                    }
                    else
                    {
                        newSetting.isProdActiveLogBit = true;
                    }
                }
                else
                {
                    if (isEditing)
                    {
                        updatedSetting.isProdActiveLogBit = false;
                    }
                    else
                    {
                        newSetting.isProdActiveLogBit = false;
                    }
                }
                CheckIfAllowApply();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!fillingInfoForEdit)
            {
                if (radioButton2.Checked)
                {
                    if (isEditing)
                    {
                        updatedSetting.isShiftActiveLogBit = true;
                    }
                    else
                    {
                        newSetting.isShiftActiveLogBit = true;
                    }
                }
                else
                {
                    if (isEditing)
                    {
                        updatedSetting.isShiftActiveLogBit = false;
                    }
                    else
                    {
                        newSetting.isShiftActiveLogBit = false;
                    }
                }
                CheckIfAllowApply();
            }
        }
        #endregion

        #region FUNCTIONS
        private void InitComboBoxes()
        {
            cb_ClassType.Items.Clear();
            cb_ClassType.Items.Add("Logging");
            cb_ClassType.Items.Add("Direct stop");
            cb_ClassType.Items.Add("Indirect");
            cb_ClassType.Items.Add("Indirect subclass");

            cb_SubClassMember.Items.Clear();
            foreach (LogSettings s in localListOfSettings)
            {
                if (s.classType == 4)
                    cb_SubClassMember.Items.Add(s.className);
            }
        }

        private void InitListView()
        {
            ExistingClassesLV.Items.Clear();
            ExistingClassesLV.Columns.Clear();
            ExistingClassesLV.View = View.Details;
            ExistingClassesLV.FullRowSelect = true;
            ExistingClassesLV.Columns.Add("Class name", 150);
            ExistingClassesLV.Columns.Add("Class nr", 60);
            ExistingClassesLV.Columns.Add("Class type", 60);
            ExistingClassesLV.Columns.Add("Message nr", 80);
            ExistingClassesLV.Columns.Add("Sub class member", 150);

            foreach (LogSettings item in localListOfSettings)
            {
                var lvi = new ListViewItem(new[] { item.className.ToString(), item.classNr.ToString(), item.classType.ToString(), item.messageNr.ToString(), item.subClassMember.ToString() });

                ExistingClassesLV.Items.Add(lvi);
            }


        }

        private void SubClassMemberVisible(bool visible)
        {
            if (visible)
            {
                lbl_SubClassMember.Visible = true;
                cb_SubClassMember.Visible = true;
            }
            else
            {
                lbl_SubClassMember.Visible = false;
                cb_SubClassMember.Visible = false;
            }
        }

        private void MsgNrVisible(bool visible)
        {
            if (visible)
            {
                lbl_MsgNr.Visible = true;
                tb_MsgNr.Visible = true;
            }
            else
            {
                lbl_MsgNr.Visible = false;
                tb_MsgNr.Visible = false;
            }
        }

        private void SettingWasEdited(bool yes)
        {
            if ( yes )
            {
                btn_Discard.Enabled = true;
            }
        }

        private void CheckIfAllowApply()
        {
            if (!fillingInfoForEdit)
            {
                if (tb_ClassName.Text != "")
                {
                    if (IsNumeric(tb_ClassNr.Text))
                    {
                        if (int.Parse(tb_ClassNr.Text) > 0)
                        {
                            if (cb_ClassType.SelectedIndex > -1)
                            {
                                if (cb_ClassType.SelectedIndex == 0)
                                {
                                    if (radioButton1.Checked || radioButton2.Checked)
                                    { 
                                        if (IsNumeric(tb_MsgNr.Text))
                                        {
                                            if (int.Parse(tb_MsgNr.Text) > 0)
                                            {
                                                //SUCCESS
                                                btn_Apply.Enabled = true;
                                                return;
                                            }
                                        }
                                    }
                                }
                                else if (cb_ClassType.SelectedIndex == 2)
                                {
                                    if (IsNumeric(tb_MsgNr.Text))
                                    {
                                        if (int.Parse(tb_MsgNr.Text) > 0)
                                        {
                                            if (cb_SubClassMember.SelectedIndex > -1)
                                            {
                                                //SUCCESS
                                                btn_Apply.Enabled = true;
                                                return;
                                            }
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    //SUCCESS
                                    btn_Apply.Enabled = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            //FAIL
            btn_Apply.Enabled = false;
        }

        private bool IsNumeric(string text)
        {
            int parsedValue;
            if (!int.TryParse(text, out parsedValue))
            {
                MessageBox.Show("This is a number only field!");
                return false;
            }
            return true;
        }

        private bool DuplicatesInListView()
        {            
            foreach (ListViewItem item in ExistingClassesLV.Items)
            {
                if ( cb_ClassType.SelectedIndex == 0 && String.Compare(item.SubItems[1].Text.ToLower(), tb_ClassNr.Text.ToLower()) == 0 )
                {
                    if (String.Compare(item.SubItems[3].Text.ToLower(), tb_MsgNr.Text.ToLower()) == 0)
                    {
                        MessageBox.Show("Message number allready exists!");
                        return true;
                    }                        
                }

                else if (cb_ClassType.SelectedIndex == 2)
                {
                    if (String.Compare(item.SubItems[3].Text.ToLower(), tb_MsgNr.Text.ToLower()) == 0)
                    {
                        MessageBox.Show("Message number allready exists!");
                        return true;
                    }
                }

                else
                {
                    if (String.Compare(item.SubItems[0].Text.ToLower(), tb_ClassName.Text.ToLower()) == 0 ||
                        String.Compare(item.SubItems[1].Text.ToLower(), tb_ClassNr.Text.ToLower()) == 0)
                    {
                        MessageBox.Show("Class name or Class number allready exists!");
                        return true;
                    }
                }

            }
            return false;
        }

        #endregion


    }
}
