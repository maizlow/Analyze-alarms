﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Analyze_alarms.Classes
{

    public class ReportData
    {
        private ReportFormData rpData;
        private UC_NewLog myUC;
        private TabPage tp_Report;
        public string saveReportFilePath;
        public SaveFileDialog saveDialog;
        public OpenFileDialog openDialog;
        public Forms.PaintCharts paintChartsForm;

        public ReportData(UC_NewLog userControl, ReportFormData RFD)
        {
            this.myUC = userControl;
            rpData = RFD;
        }

        /// <summary> 
        /// Generates all tabpage controls
        /// </summary>
        /// <returns>The tabpage</returns>
        public TabPage CreateTabPage()
        {
            Size controlSize = new Size(244, 20);
            Font fontStyleBold = new Font(Label.DefaultFont, FontStyle.Bold);

            //Tab page
            tp_Report = new TabPage();
            tp_Report.Text = "Report";

            //Tooltips
            var tooltip = new ToolTip();
            tooltip.ReshowDelay = 1000;

            //Header label
            var lbl_Header = new Label();
            lbl_Header.Text = "Report header";
            lbl_Header.Font = fontStyleBold;
            lbl_Header.AutoSize = false;
            lbl_Header.Size = controlSize;
            lbl_Header.Location = new Point(10, 10);
            tp_Report.Controls.Add(lbl_Header);

            //Header textbox
            var tb_ReportHeader = new TextBox();
            tb_ReportHeader.Enter += new EventHandler(tb_ReportHeader_Enter);
            tb_ReportHeader.Leave += new EventHandler(tb_ReportHeader_Leave);
            tb_ReportHeader.Text = "Report header...";
            tb_ReportHeader.MaxLength = 35;
            tb_ReportHeader.Size = controlSize;
            tb_ReportHeader.Location = new Point(lbl_Header.Location.X, lbl_Header.Location.Y + lbl_Header.Height);
            tooltip.SetToolTip(tb_ReportHeader, "The header of the reports first page.");
            tp_Report.Controls.Add(tb_ReportHeader);

            //Date label
            var lbl_Date = new Label();
            lbl_Date.Text = "Report date";
            lbl_Date.Font = fontStyleBold;
            lbl_Date.AutoSize = false;
            lbl_Date.Size = controlSize;
            lbl_Date.Location = new Point(tb_ReportHeader.Location.X, tb_ReportHeader.Location.Y + tb_ReportHeader.Height + 15);
            tp_Report.Controls.Add(lbl_Date);

            //Date picker
            var dtp_Report = new DateTimePicker();
            dtp_Report.ValueChanged += new EventHandler(dtp_Report_ValueChanged);
            dtp_Report.Format = DateTimePickerFormat.Short;
            dtp_Report.Value = DateTime.Now;
            dtp_Report.Size = controlSize;
            dtp_Report.Location = new Point(lbl_Date.Location.X, lbl_Date.Location.Y + lbl_Date.Height);
            tp_Report.Controls.Add(dtp_Report);

            //Report from plant/line label
            var lbl_ReportFrom = new Label();
            lbl_ReportFrom.Text = "Report from";
            lbl_ReportFrom.Font = fontStyleBold;
            lbl_ReportFrom.AutoSize = false;
            lbl_ReportFrom.Size = controlSize;
            lbl_ReportFrom.Location = new Point(dtp_Report.Location.X, dtp_Report.Location.Y + dtp_Report.Height + 15);
            tp_Report.Controls.Add(lbl_ReportFrom);

            //Report from plant/line textbox
            var tb_ReportFrom = new TextBox();
            tb_ReportFrom.Enter += new EventHandler(tb_ReportFrom_Enter);
            tb_ReportFrom.Leave += new EventHandler(tb_ReportFrom_Leave);
            tb_ReportFrom.Text = "Report from...";
            tb_ReportFrom.MaxLength = 35;
            tb_ReportFrom.Size = controlSize;
            tb_ReportFrom.Location = new Point(lbl_ReportFrom.Location.X, lbl_ReportFrom.Location.Y + lbl_ReportFrom.Height);
            tp_Report.Controls.Add(tb_ReportFrom);

            //ReportBy label
            var lbl_ReportBy = new Label();
            lbl_ReportBy.Text = "Report done by";
            lbl_ReportBy.Font = fontStyleBold;
            lbl_ReportBy.AutoSize = false;
            lbl_ReportBy.Size = controlSize;
            lbl_ReportBy.Location = new Point(tb_ReportFrom.Location.X, tb_ReportFrom.Location.Y + tb_ReportFrom.Height + 15);
            tp_Report.Controls.Add(lbl_ReportBy);

            //ReportBy textbox
            var tb_ReportBy = new TextBox();
            tb_ReportBy.Enter += new EventHandler(tb_ReportBy_Enter);
            tb_ReportBy.Leave += new EventHandler(tb_ReportBy_Leave);
            tb_ReportBy.Text = "Report done by...";
            tb_ReportBy.Size = controlSize;
            tb_ReportBy.Location = new Point(lbl_ReportBy.Location.X, lbl_ReportBy.Location.Y + lbl_ReportBy.Height);
            tp_Report.Controls.Add(tb_ReportBy);

            //Row chart label
            var lbl_RowChart = new Label();
            lbl_RowChart.Text = "Row chart";
            lbl_RowChart.Font = fontStyleBold;
            lbl_RowChart.AutoSize = true;
            lbl_RowChart.Location = new Point(lbl_Header.Location.X + lbl_Header.Width + 30, lbl_Header.Location.Y);
            tp_Report.Controls.Add(lbl_RowChart);

            //Charts checkbox row chart
            var chk_RowChart = new CheckBox();
            chk_RowChart.CheckStateChanged += new EventHandler(chk_RowChart_CheckStateChanged);
            chk_RowChart.Text = "Add Row Chart";
            chk_RowChart.Checked = true;
            chk_RowChart.AutoSize = true;
            chk_RowChart.Location = new Point(lbl_RowChart.Location.X, lbl_RowChart.Location.Y + lbl_RowChart.Height + 8);
            tp_Report.Controls.Add(chk_RowChart);

            //Pie chart label
            var lbl_PieChart = new Label();
            lbl_PieChart.Text = "Pie chart";
            lbl_PieChart.Font = fontStyleBold;
            lbl_PieChart.AutoSize = true;
            lbl_PieChart.Location = new Point(chk_RowChart.Location.X + chk_RowChart.Width + 5, lbl_RowChart.Location.Y);
            tp_Report.Controls.Add(lbl_PieChart);

            //Charts checkbox pie chart
            var chk_PieChart = new CheckBox();
            chk_PieChart.CheckStateChanged += new EventHandler(chk_PieChart_CheckStateChanged);
            chk_PieChart.Text = "Add Pie Chart";
            chk_PieChart.Checked = true;
            chk_PieChart.AutoSize = true;
            chk_PieChart.Location = new Point(chk_RowChart.Location.X + chk_RowChart.Width + 5, chk_RowChart.Location.Y);
            tp_Report.Controls.Add(chk_PieChart);

            //Summary label
            var lbl_Summary = new Label();
            lbl_Summary.Text = "Summary";
            lbl_Summary.Font = fontStyleBold;
            lbl_Summary.AutoSize = true;
            lbl_Summary.Location = new Point(chk_PieChart.Location.X + chk_PieChart.Width + 5, lbl_RowChart.Location.Y);
            tp_Report.Controls.Add(lbl_Summary);

            //Summary checkbox
            var chk_Summary = new CheckBox();
            chk_Summary.CheckStateChanged += new EventHandler(chk_Summary_CheckStateChanged);
            chk_Summary.Text = "Add summary";
            chk_Summary.Checked = true;
            chk_Summary.AutoSize = true;
            chk_Summary.Location = new Point(lbl_Summary.Location.X, chk_PieChart.Location.Y);
            tp_Report.Controls.Add(chk_Summary);

            //Freetext label
            var lbl_FreeText = new Label();
            lbl_FreeText.Text = "Comments (E.g. description of attachments)";
            lbl_FreeText.Font = fontStyleBold;
            lbl_FreeText.AutoSize = true;
            lbl_FreeText.Location = new Point(chk_RowChart.Location.X, lbl_Date.Location.Y);
            tp_Report.Controls.Add(lbl_FreeText);

            //Freetext textbox
            var tb_Freetext = new TextBox();
            tb_Freetext.Enter += new EventHandler(tb_Freetext_Enter);
            tb_Freetext.Leave += new EventHandler(tb_Freetext_Leave);
            tb_Freetext.Text = "";
            tb_Freetext.Multiline = true;
            tb_Freetext.Location = new Point(lbl_FreeText.Location.X, dtp_Report.Location.Y);
            tb_Freetext.Size = new Size(chk_Summary.Location.X + chk_Summary.Width - chk_RowChart.Location.X, tb_ReportBy.Location.Y + tb_ReportBy.Height - tb_Freetext.Location.Y);
            tooltip.SetToolTip(tb_Freetext, "Write anything you want to add, leave blank to not include.");
            tp_Report.Controls.Add(tb_Freetext);

            //Generate attachment button
            var btn_AddAttachments = new Button();
            btn_AddAttachments.Click += new EventHandler(btn_AddAttachments_Click);
            btn_AddAttachments.Name = "btn_AddAttachments";
            btn_AddAttachments.Text = "Add attachments";
            btn_AddAttachments.Size = new Size(100, 30);
            btn_AddAttachments.Location = new Point(tb_ReportBy.Location.X, tb_ReportBy.Location.Y + tb_ReportBy.Height + 15);
            tooltip.SetToolTip(btn_AddAttachments, "Add attachments, to add multiple just select all in the Open file dialog.");
            tp_Report.Controls.Add(btn_AddAttachments);


            //Attachment label
            var lbl_Attachments = new Label();
            lbl_Attachments.Name = "lbl_Attachments";
            lbl_Attachments.Text = "No attachments.";
            lbl_Attachments.ForeColor = Color.Red;
            lbl_Attachments.AutoSize = true;
            lbl_Attachments.Location = new Point(btn_AddAttachments.Location.X + btn_AddAttachments.Width + 10, btn_AddAttachments.Location.Y + 8);
            tp_Report.Controls.Add(lbl_Attachments);

            //Generate custom logo button
            var btn_AddCustomLogo = new Button();
            btn_AddCustomLogo.Click += new EventHandler(btn_AddCustomLogo_Click);
            btn_AddCustomLogo.Name = "btn_AddCustomLogo";
            btn_AddCustomLogo.Text = "Add custom logo";
            btn_AddCustomLogo.Size = new Size(100, 30);
            btn_AddCustomLogo.Location = new Point(tb_Freetext.Location.X, tb_Freetext.Location.Y + tb_Freetext.Height + 15);
            tooltip.SetToolTip(btn_AddCustomLogo, "Add a custom logo for first page.");
            tp_Report.Controls.Add(btn_AddCustomLogo);

            //Custom logo pic box
            var pb_CustomLogo = new PictureBox();
            pb_CustomLogo.Name = "pb_CustomLogo";
            pb_CustomLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_CustomLogo.BorderStyle = BorderStyle.FixedSingle;
            pb_CustomLogo.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\logo.png");
            pb_CustomLogo.Size = new Size(50, 30);
            pb_CustomLogo.Location = new Point(btn_AddCustomLogo.Location.X + btn_AddCustomLogo.Width + 10, btn_AddCustomLogo.Location.Y);
            tp_Report.Controls.Add(pb_CustomLogo);

            //Generate report button
            var btn_GenerateReport = new Button();
            btn_GenerateReport.Click += new EventHandler(btn_GenerateReport_Click);
            btn_GenerateReport.Text = "Generate report";
            btn_GenerateReport.Font = new Font(btn_GenerateReport.Font.FontFamily, 18.0f);
            btn_GenerateReport.Size = new Size(chk_Summary.Location.X + chk_Summary.Width - tb_ReportBy.Location.X, 100);
            btn_GenerateReport.Location = new Point(tb_ReportBy.Location.X, btn_AddAttachments.Location.Y + btn_AddAttachments.Height + 15);
            tp_Report.Controls.Add(btn_GenerateReport);

            //Generate save dialog    
            saveDialog = new SaveFileDialog();
            saveDialog.FileOk += new CancelEventHandler(saveDialog_FileOk);
            saveDialog.DefaultExt = ".pdf";
            saveDialog.Filter = "Portable Document Format (.pdf)|*.pdf";
            saveDialog.RestoreDirectory = true;

            //Generate open dialog    
            openDialog = new OpenFileDialog();
            openDialog.Filter = "Images | *.png; *.jpg; *.jpeg; *.bmp; *.gif";
            openDialog.RestoreDirectory = true;
            openDialog.Multiselect = true;

            return tp_Report;
        }

        public void AddAttachmentsToReport(ref Classes.ReportGenerator generator)
        {
            if (rpData.attachmentsFilePaths != null)
            {
                generator.attachments = new List<Classes.AttachmentImages>();
                int new_width;
                double new_height;
                double aspect_ratio;

                foreach (string path in rpData.attachmentsFilePaths)
                {
                    Classes.AttachmentImages img = new Classes.AttachmentImages();
                    Bitmap bmp = new Bitmap(path);
                    Image image;

                    Classes.ImageHelper imgOrient = new Classes.ImageHelper();
                    image = imgOrient.RotateImageByExifOrientationData((Image)bmp);

                    if (image.Width >= image.Height)
                    {
                        img.orientation = false;
                        new_width = 1024;
                        aspect_ratio = (double)bmp.Height / (double)bmp.Width;
                        new_height = aspect_ratio * (double)new_width;
                    }
                    else
                    {
                        img.orientation = true;
                        new_width = 480;
                        aspect_ratio = (double)bmp.Height / (double)bmp.Width;
                        new_height = aspect_ratio * (double)new_width;
                    }

                    //Only resize too big images
                    if ((img.orientation == false && image.Width > 1024) | (img.orientation && image.Height > 480))
                        image = ImageHandling.ResizeImage(image, new_width, (int)new_height);

                    img.img = image;

                    generator.attachments.Add(img);
                }
            }
        }

        private void saveDialog_FileOk(object sender, CancelEventArgs e)
        {
            SaveFileDialog s = (SaveFileDialog)sender;
            saveReportFilePath = s.FileName;            
        }
        
        private void btn_AddCustomLogo_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            PictureBox pb = (PictureBox)tp_Report.Controls[tp_Report.Controls.IndexOfKey("pb_CustomLogo")];

            if (btn.Text == "Add custom logo")
            {
                openDialog.Multiselect = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    pb.Image = Image.FromFile(openDialog.FileName);
                    rpData.customLogoPath = openDialog.FileName;
                }
                btn.Text = "Default logo";
            }
            else
            {
                pb.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\logo.png");
                rpData.customLogoPath = "";
                btn.Text = "Add custom logo";
            }

        }

        private void btn_AddAttachments_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Label lbl = (Label)tp_Report.Controls[tp_Report.Controls.IndexOfKey("lbl_Attachments")];
            if (btn.Text == "Add attachments")
            {
                openDialog.Multiselect = true;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    rpData.attachmentsFilePaths = openDialog.FileNames;
                    lbl.ForeColor = Color.Green;
                    lbl.Text = "Attachments loaded.";
                }
                btn.Text = "Clear attachments";
            }
            else
            {
                rpData.attachmentsFilePaths = null;
                lbl.ForeColor = Color.Red;
                lbl.Text = "No attachments.";
                btn.Text = "Add attachments";
            }

        }

        private void tb_ReportFrom_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!rpData.tb_ReportFrom_Edited)
                thisTB.Text = "";
        }

        private void tb_ReportFrom_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            rpData.tb_ReportFrom_Text = thisTB.Text;
            rpData.tb_ReportFrom_Edited = true;
        }

        private void tb_ReportHeader_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!rpData.tb_Header_Edited)
                thisTB.Text = "";
        }

        private void tb_ReportHeader_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            rpData.tb_Header_Text = thisTB.Text;
            rpData.tb_Header_Edited = true;
        }

        private void dtp_Report_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;
            rpData.dtp_ReportDate = dtp.Value;
        }

        private void tb_ReportBy_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!rpData.tb_ReportBy_Edited)
                thisTB.Text = "";
        }

        private void tb_ReportBy_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            rpData.tb_ReportBy_Text = thisTB.Text;
            rpData.tb_ReportBy_Edited = true;
        }

        private void chk_PieChart_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox thisChk = (CheckBox)sender;
            if (thisChk.Checked)
                rpData.chk_PieChart_Checked = true;
            else
                rpData.chk_PieChart_Checked = false;
        }

        private void chk_RowChart_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox thisChk = (CheckBox)sender;
            if (thisChk.Checked)
                rpData.chk_RowChart_Checked = true;
            else
                rpData.chk_RowChart_Checked = false;
        }

        private void chk_Summary_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox thisChk = (CheckBox)sender;
            if (thisChk.Checked)
                rpData.chk_Summary_Checked = true;
            else
                rpData.chk_Summary_Checked = false;
        }

        private void tb_Freetext_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!rpData.tb_FreeText_Edited)
                thisTB.Text = "";
        }

        private void tb_Freetext_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            rpData.tb_FreeText_Text = thisTB.Text;
            rpData.tb_FreeText_Edited = true;
        }

        private void btn_GenerateReport_Click(object sender, EventArgs e)
        {
            saveDialog.ShowDialog();
            myUC.StartCreatePDFReport(this);            
        }
    }
}