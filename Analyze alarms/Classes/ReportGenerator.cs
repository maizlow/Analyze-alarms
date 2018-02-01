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
        private static readonly double UsableWidth = PageWidth - Margin * 2;
        private static readonly XImage ABECELogo = XImage.FromFile(System.Environment.CurrentDirectory + "\\logo.png");
        private static XRect LogoRect = new XRect(PageWidth / 2 - ABECELogo.PointWidth / 2, Margin, ABECELogo.PointWidth, ABECELogo.PointHeight);
        private static readonly XFont Font = new XFont(FontFamily, 11, XFontStyle.Regular);
        private static XImage customerLogoImage;
        public Image rowChart, pieChart;
        private UC_NewLog parent;

        public ReportGenerator(UC_NewLog parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Generate a new PDF report
        /// </summary>
        /// <returns>Filepath to new PDF</returns>
        public string Generate(bool useCustomerLogo)
        {
            var document = new PdfDocument();

            //======================================================================================================================================//
            //Add page: Front page
            var firstPage = document.AddPage();
            firstPage.Size = PageSize;
            var firstPage_gfx = XGraphics.FromPdfPage(firstPage);
            GenerateFirstPage(firstPage_gfx, useCustomerLogo);
            GenerateFooter(firstPage_gfx, PageOrientation.Portrait);

            //======================================================================================================================================//
            //Add page: Summary
            var summaryPage = document.AddPage();
            summaryPage.Size = PageSize;
            var summaryPage_gfx = XGraphics.FromPdfPage(summaryPage);
            GenerateSummaryPage(summaryPage_gfx);
            GenerateFooter(firstPage_gfx, PageOrientation.Portrait);

            //======================================================================================================================================//
            //Add page: Row chart
            if (rowChart != null && parent.chk_RowChart_Checked)
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
            if (pieChart != null && parent.chk_PieChart_Checked)
            {
                var pieChartPage = document.AddPage();
                pieChartPage.Size = PageSize;
                pieChartPage.Orientation = PageOrientation.Landscape;
                var pieChartPage_gfx = XGraphics.FromPdfPage(pieChartPage);
                GenerateChartPage(pieChartPage_gfx, pieChart);
                GenerateFooter(pieChartPage_gfx, PageOrientation.Landscape);
            }


            var filename = @"Analyzed log report " + DateTime.Today.ToShortDateString() + ".pdf";
            document.Save(filename);
            return filename;
        }



        /// <summary>
        /// Creates a new logo image from the filePath. If not created the default ABECE logo will show.
        /// </summary>
        /// <param name="filePath">File path to customer logo.</param>
        public void NewCustomerLogo(string filePath)
        {
            customerLogoImage = XImage.FromFile(filePath);            
            LogoRect = new XRect(PageWidth / 2 - customerLogoImage.PointWidth / 2, Margin, customerLogoImage.PointWidth, customerLogoImage.PointHeight);
        }
        
        /// <summary>
        /// Generate a first page
        /// </summary>
        /// <param name="gfx">First page graphics component</param>
        /// <param name="useCustomerLogo">If using a customerlogo. Needs to be created first!</param>
        private void GenerateFirstPage(XGraphics gfx, bool useCustomerLogo)
        {
            if (useCustomerLogo)
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
            XSize stringSize = gfx.MeasureString(parent.tb_Header_Text, font);

            //Get rectangle height dependning on how long the string is. Otherwise the text wont wrap.
            double rectHeight;
            if (stringSize.Width > UsableWidth)
                rectHeight = font.GetHeight() + font.GetHeight() + font.GetHeight();
            else rectHeight = font.GetHeight();

            var rect = new XRect(Margin, LogoRect.Bottom + 150, UsableWidth, rectHeight);
            
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(parent.tb_Header_Text, font, TextBrush, rect, XStringFormats.TopLeft);
            
            //======================================================================================================================================//
            //Add Plant: label
            font = new XFont("Calibri", 24.0, XFontStyle.Bold | XFontStyle.Underline);
            stringSize = gfx.MeasureString("Plant: ", font);
            rect = new XRect(Margin, PageHeight - 180, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString("Plant: ", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add where text
            font = new XFont("Calibri", 22.0, XFontStyle.Bold);
            stringSize = gfx.MeasureString(parent.tb_ReportFrom_Text, font);
            rect = new XRect(Margin + rect.Width + 8, rect.Location.Y + 2, stringSize.Width, rectHeight);
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.tb_ReportFrom_Text, font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add By: label
            font = new XFont("Calibri", 24.0, XFontStyle.Bold | XFontStyle.Underline);
            stringSize = gfx.MeasureString("By: ", font);
            rect = new XRect(Margin, rect.Location.Y + rect.Height, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString("By: ", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add By: text
            font = new XFont("Calibri", 22.0, XFontStyle.Bold);
            stringSize = gfx.MeasureString(parent.tb_ReportBy_Text, font);
            rect = new XRect(Margin + rect.Width + 8, rect.Location.Y + 2, stringSize.Width, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(parent.tb_ReportBy_Text, font, TextBrush, rect, XStringFormats.TopLeft);

        }

        private void GenerateSummaryPage(XGraphics gfx)
        {
            //======================================================================================================================================//
            //Add Plant: label
            var font = new XFont("Calibri", 30.0, XFontStyle.Bold);
            var stringSize = gfx.MeasureString("Summary", font);
            var rect = new XRect(Margin, Margin, UsableWidth, font.GetHeight());
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Summary", font, TextBrush, rect, XStringFormats.TopLeft);

            //======================================================================================================================================//
            //Add: Header boxes
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
            //Add Plant: label
            font = new XFont("Calibri", 15.0, XFontStyle.Bold);
            var msgTextRect = new XRect(rects[0].Location.X + 1, rects[0].Location.Y + 3, rects[0].Width - 2, rects[0].Height - 2);
            var amountTextRect = new XRect(rects[1].Location.X + 1, rects[1].Location.Y + 3, rects[1].Width - 2, rects[1].Height - 2);
            var durationTextRect = new XRect(rects[2].Location.X + 1, rects[2].Location.Y + 3, rects[2].Width - 2, rects[2].Height - 2);

            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Alarm text", font, TextBrush, msgTextRect, XStringFormats.TopLeft);
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Amount", font, TextBrush, amountTextRect, XStringFormats.TopLeft);
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString("Total duration", font, TextBrush, durationTextRect, XStringFormats.TopLeft);

            font = new XFont("Calibri", 11.0, XFontStyle.Bold);
            XRect loopRect;
            int i = 1;

            foreach (Summary s in parent.mySummary)
            {
                loopRect = new XRect(startX, startY + (rectHeight * i), bigBoxW, rectHeight);
                gfx.DrawRectangle(pen, loopRect);

                loopRect = new XRect(startX + 2, startY + (rectHeight * i) + 5, rects[0].Width - 2, rects[0].Height - 2);
                CreateTextFormatter(gfx, XParagraphAlignment.Left).DrawString(s.MsgText, font, TextBrush, loopRect, XStringFormats.TopLeft);

                loopRect = new XRect(startX + bigBoxW, startY + (rectHeight * i), smallBoxW, rectHeight);
                gfx.DrawRectangle(pen, loopRect);

                loopRect = new XRect(startX + bigBoxW + 1, startY + (rectHeight * i) + 5, rects[1].Width - 2, rects[1].Height - 2);
                CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(s.Amount.ToString(), font, TextBrush, loopRect, XStringFormats.TopLeft);

                loopRect = new XRect(startX + bigBoxW + smallBoxW, startY + (rectHeight * i), smallBoxW, rectHeight);
                gfx.DrawRectangle(pen, loopRect);

                loopRect = new XRect(startX + bigBoxW + smallBoxW + 1, startY + (rectHeight * i) + 5, rects[2].Width - 2, rects[2].Height - 2);
                CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(s.stopDuration.ToString(@"hh\:mm\:ss"), font, TextBrush, loopRect, XStringFormats.TopLeft);

                i++;
            }




        }

        private void GenerateChartPage(XGraphics gfx, Image chart)
        {        
            XImage ximg = XImage.FromGdiPlusImage(chart);
            gfx.DrawImage(ximg, PageWidthLandscape / 2 - ximg.PointWidth / 2, PageWidth / 2 - ximg.PointHeight / 2);
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
