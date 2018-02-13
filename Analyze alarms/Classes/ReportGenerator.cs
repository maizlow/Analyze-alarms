using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Analyze_alarms.Classes
{
    public class ReportGenerator
    {
        private const int Margin = 30;
        private const int SectionPadding = 30;
        private const string DateFormat = "yyyy-MM-dd";
        private const string FontFamily = "Calibri";
        private const PageSize PageSize = PdfSharp.PageSize.A4;
        private static readonly XSolidBrush TextBrush = XBrushes.Black;
        private static readonly double PageWidth = PageSizeConverter.ToSize(PageSize).Width;
        private static readonly double PageHeight = PageSizeConverter.ToSize(PageSize).Height;
        private static readonly double PageWidthLandscape = PageSizeConverter.ToSize(PageSize).Height;
        private static readonly double PageHeightLandscape = PageSizeConverter.ToSize(PageSize).Width;
        public double UsableWidth = PageWidth - Margin * 2;
        private static readonly XImage ABECELogo = XImage.FromFile(System.Environment.CurrentDirectory + "\\logo.png");
        private static XRect LogoRect = new XRect(PageWidth / 2 - ABECELogo.PointWidth / 2, Margin, ABECELogo.PointWidth, ABECELogo.PointHeight);
        private static readonly XFont Font = new XFont(FontFamily, 11, XFontStyle.Regular);
        private static XImage customerLogoImage = null;
        public Image rowChart, pieChart;
        public List<AttachmentImages> attachments;
        private UC_NewLog parent;
        private int maxSummaryEntrysPerPage = 25;
        public ReportTab reportData;

        public ReportGenerator(UC_NewLog parent, ReportTab reportData)
        {
            this.parent = parent;
            this.reportData = reportData;
        }

        /// <summary>
        /// Generate a new PDF report
        /// </summary>
        /// <returns>Filepath to new PDF</returns>
        public string Generate(string savePath)
        {
            if (savePath != null)
            {
                using (var document = new PdfDocument())
                {
                    //======================================================================================================================================//
                    //Add page: Front page
                    var firstPage = document.AddPage();
                    firstPage.Size = PageSize;
                    var firstPage_gfx = XGraphics.FromPdfPage(firstPage);
                    GenerateFirstPage(firstPage_gfx);
                    GenerateFooter(firstPage_gfx, PageOrientation.Portrait);

                    //======================================================================================================================================//
                    //Add page: Freetext
                    if (parent.myReportFormData.tb_FreeText_Text != "" && parent.myReportFormData.tb_FreeText_Text != null)
                    {
                        var freeTextPage = document.AddPage();
                        freeTextPage.Size = PageSize;
                        var freeTextPage_gfx = XGraphics.FromPdfPage(freeTextPage);
                        GenerateFreeTextPage(freeTextPage_gfx);
                        GenerateFooter(freeTextPage_gfx, PageOrientation.Portrait);
                    }

                    //======================================================================================================================================//
                    //Add page: Summary
                    if (parent.myReportFormData.chk_Summary_Checked)
                    {
                        var summaryPage = document.AddPage();
                        summaryPage.Size = PageSize;
                        var summaryPage_gfx = XGraphics.FromPdfPage(summaryPage);
                        GenerateSummaryPage(summaryPage_gfx, 0);
                        GenerateFooter(summaryPage_gfx, PageOrientation.Portrait);

                        PdfPage summaryPage2 = new PdfPage();
                        PdfPage summaryPage3 = new PdfPage();

                        if (parent.mySummary.Count > maxSummaryEntrysPerPage)
                        {
                            GenerateContinuesLabel(summaryPage_gfx);
                            summaryPage2 = document.AddPage();
                            summaryPage2.Size = PageSize;
                            var summaryPage2_gfx = XGraphics.FromPdfPage(summaryPage2);
                            GenerateSummaryPage(summaryPage2_gfx, maxSummaryEntrysPerPage);
                            GenerateFooter(summaryPage2_gfx, PageOrientation.Portrait);

                            if (parent.mySummary.Count > maxSummaryEntrysPerPage * 2)
                            {
                                GenerateContinuesLabel(summaryPage2_gfx);
                                summaryPage3 = document.AddPage();
                                summaryPage3.Size = PageSize;
                                var summaryPage3_gfx = XGraphics.FromPdfPage(summaryPage3);
                                GenerateSummaryPage(summaryPage3_gfx, maxSummaryEntrysPerPage * 2);
                                GenerateFooter(summaryPage3_gfx, PageOrientation.Portrait);
                            }
                        }
                    }

                    //======================================================================================================================================//
                    //Add page: Row chart
                    if (rowChart != null && parent.myReportFormData.chk_RowChart_Checked)
                    {
                        var rowChartPage = document.AddPage();
                        rowChartPage.Size = PageSize;
                        rowChartPage.Orientation = PageOrientation.Landscape;
                        var rowChartPage_gfx = XGraphics.FromPdfPage(rowChartPage);
                        GenerateChartPage(rowChartPage_gfx, rowChart);
                        GenerateFooter(rowChartPage_gfx, PageOrientation.Landscape);
                    }

                    //======================================================================================================================================//
                    //Add page: Pie chart
                    if (pieChart != null && parent.myReportFormData.chk_PieChart_Checked)
                    {
                        var pieChartPage = document.AddPage();
                        pieChartPage.Size = PageSize;
                        pieChartPage.Orientation = PageOrientation.Landscape;
                        var pieChartPage_gfx = XGraphics.FromPdfPage(pieChartPage);
                        GenerateChartPage(pieChartPage_gfx, pieChart);
                        GenerateFooter(pieChartPage_gfx, PageOrientation.Landscape);
                    }

                    //======================================================================================================================================//
                    //Add page: Attachments
                    if (attachments != null)
                    {
                        foreach (AttachmentImages i in attachments)
                        {
                            var attachtmentPage = document.AddPage();
                            attachtmentPage.Size = PageSize;

                            if (i.orientation) attachtmentPage.Orientation = PageOrientation.Portrait;
                            else attachtmentPage.Orientation = PageOrientation.Landscape;

                            var attachtmentPage_gfx = XGraphics.FromPdfPage(attachtmentPage);
                            GenerateAttachmentPage(attachtmentPage_gfx, i.img, attachments.IndexOf(i));

                            if (i.orientation) GenerateFooter(attachtmentPage_gfx, PageOrientation.Portrait);
                            else GenerateFooter(attachtmentPage_gfx, PageOrientation.Landscape);

                        }
                    }

                    document.Save(savePath);
                    return savePath;
                }
            }
            return null;
        }



        /// <summary>
        /// Creates a new logo image from the filePath. If not created the default ABECE logo will show.
        /// </summary>
        /// <param name="filePath">File path to customer logo.</param>
        public void NewCustomerLogo(string filePath)
        {
            if (filePath != "")
            {
                customerLogoImage = XImage.FromFile(filePath);
                LogoRect = new XRect(PageWidth / 2 - customerLogoImage.PointWidth / 2, Margin, customerLogoImage.PointWidth, customerLogoImage.PointHeight);
            }
            else
            {
                customerLogoImage = null;
                LogoRect = new XRect(PageWidth / 2 - ABECELogo.PointWidth / 2, Margin, ABECELogo.PointWidth, ABECELogo.PointHeight);
            }

        }
        
        /// <summary>
        /// Generate a first page
        /// </summary>
        /// <param name="gfx">First page graphics component</param>
        /// <param name="useCustomerLogo">If using a customerlogo. Needs to be created first!</param>
        private void GenerateFirstPage(XGraphics gfx)
        {
            if (customerLogoImage != null)
            {
                gfx.DrawImage(customerLogoImage, LogoRect.Left, LogoRect.Top);
            }
            else
            {
                gfx.DrawImage(ABECELogo, LogoRect.Left, LogoRect.Top);
            }

            //======================================================================================================================================//
            var font = new XFont("Calibri", 42.0, XFontStyle.Bold);
            //Get stringsize width
            XSize stringSize = gfx.MeasureString(parent.myReportFormData.tb_Header_Text, font);

            //Get rectangle height dependning on how long the string is. Otherwise the text wont wrap.
            double rectHeight;
            if (stringSize.Width > UsableWidth)
                rectHeight = font.GetHeight() + font.GetHeight() + font.GetHeight();
            else rectHeight = font.GetHeight();

            var rect = new XRect(Margin, LogoRect.Bottom + 150, UsableWidth, rectHeight);
            
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(parent.myReportFormData.tb_Header_Text, font, TextBrush, rect, XStringFormats.TopLeft);
            
            //======================================================================================================================================//
            //Add Plant: label
            font = new XFont("Calibri", 24.0, XFontStyle.Bold | XFontStyle.Underline);
            stringSize = gfx.MeasureString("Plant: ", font);
            rect = new XRect(Margin, PageHeight - 220, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString("Plant: ", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add where text
            font = new XFont("Calibri", 22.0, XFontStyle.Bold);
            stringSize = gfx.MeasureString(parent.myReportFormData.tb_ReportFrom_Text, font);
            rect = new XRect(Margin + rect.Width + 8, rect.Location.Y + 2, stringSize.Width, rectHeight);
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.myReportFormData.tb_ReportFrom_Text, font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add By: label
            font = new XFont("Calibri", 24.0, XFontStyle.Bold | XFontStyle.Underline);
            stringSize = gfx.MeasureString("By: ", font);
            rect = new XRect(Margin, rect.Location.Y + rect.Height, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString("By: ", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add By: text
            font = new XFont("Calibri", 22.0, XFontStyle.Bold);
            stringSize = gfx.MeasureString(parent.myReportFormData.tb_ReportBy_Text, font);
            rect = new XRect(Margin + rect.Width + 8, rect.Location.Y + 2, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.myReportFormData.tb_ReportBy_Text, font, TextBrush, rect, XStringFormats.TopLeft);

            //Add Date: label
            font = new XFont("Calibri", 24.0, XFontStyle.Bold | XFontStyle.Underline);
            stringSize = gfx.MeasureString("Date: ", font);
            rect = new XRect(Margin, rect.Location.Y + rect.Height + rect.Height, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString("Date: ", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add date
            font = new XFont("Calibri", 22.0, XFontStyle.Bold);
            stringSize = gfx.MeasureString(parent.myReportFormData.tb_ReportBy_Text, font);
            rect = new XRect(Margin + rect.Width + 8, rect.Location.Y + 2, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.myReportFormData.dtp_ReportDate.Date.ToShortDateString(), font, TextBrush, rect, XStringFormats.TopLeft);
        }

        private void GenerateFreeTextPage(XGraphics gfx)
        {
            //Add comment page header
            //======================================================================================================================================//
            var font = new XFont("Calibri", 30.0, XFontStyle.Bold);
            var stringSize = gfx.MeasureString("COMMENTS", font);
            var rect = new XRect(Margin, Margin, UsableWidth, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("COMMENTS", font, TextBrush, rect, XStringFormats.TopLeft);

            //Add free text
            //======================================================================================================================================//
            font = new XFont("Calibri", 11.0, XFontStyle.Bold);
            rect = new XRect(Margin, rect.Location.Y + rect.Height + 5, UsableWidth, PageHeight - rect.Location.Y + rect.Height + 5);
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.myReportFormData.tb_FreeText_Text, font, TextBrush, rect, XStringFormats.TopLeft);
        }

        private void GenerateSummaryPage(XGraphics gfx, int firstIndex)
        {
            //======================================================================================================================================//
            //Add summary header
            var font = new XFont("Calibri", 30.0, XFontStyle.Bold);
            var stringSize = gfx.MeasureString("SUMMARY", font);
            var rect = new XRect(Margin, Margin, UsableWidth, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("SUMMARY", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add: Summary header boxes
            int bigBoxW = (int)UsableWidth / 2,
                smallBoxW = (int)UsableWidth / 5,
                startX = (int)PageWidth / 2 - (bigBoxW + smallBoxW + smallBoxW) / 2,
                startY = (int)rect.Location.Y + (int)rect.Height + 10,
                rectHeight = 25;

            XPen pen = XPens.Black;
            Rectangle[] rects =
                {
                    //msgTextRect
                    new Rectangle(new Point(startX, startY), new Size(bigBoxW, rectHeight)),
                    //amountTextRect
                    new Rectangle(new Point(startX + bigBoxW, startY), new Size(smallBoxW, rectHeight)),
                    //durationTextRect
                    new Rectangle(new Point(startX + bigBoxW + smallBoxW, startY), new Size(smallBoxW, rectHeight))
                };

            gfx.DrawRectangles(pen, rects);

            //======================================================================================================================================//
            //Add summary table
            font = new XFont("Calibri", 15.0, XFontStyle.Bold);
            var msgTextRect = new XRect(rects[0].Location.X + 1, rects[0].Location.Y + 3, rects[0].Width - 2, rects[0].Height - 2);
            var amountTextRect = new XRect(rects[1].Location.X + 1, rects[1].Location.Y + 3, rects[1].Width - 2, rects[1].Height - 2);
            var durationTextRect = new XRect(rects[2].Location.X + 1, rects[2].Location.Y + 3, rects[2].Width - 2, rects[2].Height - 2);

            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Alarm text", font, TextBrush, msgTextRect, XStringFormats.TopLeft);
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Amount", font, TextBrush, amountTextRect, XStringFormats.TopLeft);
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Total duration", font, TextBrush, durationTextRect, XStringFormats.TopLeft);

            font = new XFont("Calibri", 11.0, XFontStyle.Bold);
            XRect loopRect;
            int y = 1; //Maximum 26 then new page is needed

            for(int i = firstIndex; i < parent.mySummary.Count - 1; i++)
            {
                loopRect = new XRect(startX, startY + (rectHeight * y), bigBoxW, rectHeight);
                gfx.DrawRectangle(pen, loopRect);

                loopRect = new XRect(startX + 2, startY + (rectHeight * y) + 5, rects[0].Width - 2, rects[0].Height - 2);
                CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.mySummary[i].MsgText, font, TextBrush, loopRect, XStringFormats.TopLeft);

                loopRect = new XRect(startX + bigBoxW, startY + (rectHeight * y), smallBoxW, rectHeight);
                gfx.DrawRectangle(pen, loopRect);

                loopRect = new XRect(startX + bigBoxW + 1, startY + (rectHeight * y) + 5, rects[1].Width - 2, rects[1].Height - 2);
                CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(parent.mySummary[i].Amount.ToString(), font, TextBrush, loopRect, XStringFormats.TopLeft);

                loopRect = new XRect(startX + bigBoxW + smallBoxW, startY + (rectHeight * y), smallBoxW, rectHeight);
                gfx.DrawRectangle(pen, loopRect);

                loopRect = new XRect(startX + bigBoxW + smallBoxW + 1, startY + (rectHeight * y) + 5, rects[2].Width - 2, rects[2].Height - 2);
                CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(parent.mySummary[i].StopDuration.ToString(@"hh\:mm\:ss"), font, TextBrush, loopRect, XStringFormats.TopLeft);
                y++;
            }
        }

        /// <summary>
        /// Generates a page for a chart containing a picture and a header
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="chart">Image of chart</param>
        private void GenerateChartPage(XGraphics gfx, Image chart)
        {
            //======================================================================================================================================//
            //Add chart image
            XImage ximg = XImage.FromGdiPlusImage(chart);
            gfx.DrawImage(ximg, PageWidthLandscape / 2 - ximg.PointWidth / 2, PageWidth / 2 - ximg.PointHeight / 2);
        }

        private void GenerateAttachmentPage(XGraphics gfx, Image image, int index)
        {
            index = index + 1;
            //======================================================================================================================================//
            //Add attachment header
            var font = new XFont("Calibri", 15.0, XFontStyle.Bold);
            var stringSize = gfx.MeasureString("Attachment " + index.ToString(), font);
            var rect = new XRect(Margin, Margin, PageWidthLandscape - Margin * 2, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString("Attachment " + index.ToString(), font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add attachment image
            XImage ximg = XImage.FromGdiPlusImage(image);

            //double width, height;
            if (gfx.PageSize.Width > gfx.PageSize.Height)
            {
                //Landscape
                gfx.DrawImage(ximg, PageWidthLandscape / 2 - ximg.PointWidth / 2, PageWidth / 2 - ximg.PointHeight / 2);
            }
            else
            {
                //Portrait
                gfx.DrawImage(ximg, PageWidth / 2 - ximg.PointWidth / 2, PageWidthLandscape / 2 - ximg.PointHeight / 2);
            }
            
                        
            
        }

        private void GenerateContinuesLabel(XGraphics gfx)
        {
            //======================================================================================================================================//
            //Add "Continues..."
            var font = new XFont("Calibri", 9.0, XFontStyle.Bold);
            var stringSize = gfx.MeasureString("Continues >", font);
            var rect = new XRect(PageWidth - Margin - stringSize.Width, PageHeight - stringSize.Height - 60, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Right).DrawString("Continues >", font, TextBrush, rect, XStringFormats.TopLeft);
        }

        private void GenerateFooter(XGraphics gfx, PageOrientation orientation)
        {
            //Add By: text
            var font = new XFont("Calibri", 9.0, XFontStyle.Bold);
            var stringSize = gfx.MeasureString("Generated by ABECE - A Better Coverage\u2122 | www.abece.se", font);
            XRect rect;
            if (orientation == PageOrientation.Portrait) rect = new XRect(Margin, PageHeight - Margin, UsableWidth, stringSize.Height);
            else rect = new XRect(0, PageHeightLandscape - Margin, PageWidthLandscape, stringSize.Height);
            
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Generated by ABECE - A Better Coverage\u2122 | www.abece.se", font, TextBrush, rect, XStringFormats.TopLeft);
        }

        private static XTextFormatter CreateTextFormatter(XGraphics gfx, XParagraphAlignment alignment = XParagraphAlignment.Left)
        {
            return new XTextFormatter(gfx) { Alignment = alignment };
        }
    }
}
