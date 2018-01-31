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

        private const int Margin = 25;
        private const int SectionPadding = 30;
        private const string DateFormat = "yyyy-MM-dd";
        private const string FontFamily = "Calibri";
        private const PageSize PageSize = PdfSharp.PageSize.A4;
        private static readonly XSolidBrush TextBrush = XBrushes.Black;
        private static readonly double PageWidth = PageSizeConverter.ToSize(PageSize).Width;
        private static readonly double PageWidthLandscape = PageSizeConverter.ToSize(PageSize).Height;
        private static readonly double Width = PageWidth - Margin * 2;
        private static readonly XImage ABECELogo = XImage.FromFile(System.Environment.CurrentDirectory + "\\logo.png");
        private static XRect LogoRect = new XRect(PageWidth / 2 - ABECELogo.PointWidth / 2, Margin, ABECELogo.PointWidth, ABECELogo.PointHeight);
        private static readonly XFont Font = new XFont(FontFamily, 11, XFontStyle.Regular);
        private static readonly double BodyRectHeight = Font.GetHeight() * 7 + SectionPadding;
        private static XImage customerLogoImage;
        private string Header = "Header";
        public Image rowChart, pieChart;

        public ReportGenerator(string HeaderText = "Alarm analysis")
        {
            this.Header = HeaderText;
        }

        /// <summary>
        /// Generate a new PDF report
        /// </summary>
        /// <returns>Filepath to new PDF</returns>
        public string Generate(bool useCustomerLogo)
        {
            var document = new PdfDocument();

            //Define report
            var firstPage = document.AddPage();
            firstPage.Size = PageSize;
            var firstPage_gfx = XGraphics.FromPdfPage(firstPage);
            GenerateFirstPage(firstPage_gfx, useCustomerLogo);

            if (rowChart != null)
            {
                var chartPage = document.AddPage();
                chartPage.Size = PageSize;
                chartPage.Orientation = PageOrientation.Landscape;
                var chartPage_gfx = XGraphics.FromPdfPage(chartPage);
                GenerateChartPage(chartPage_gfx, rowChart);
            }

            if (pieChart != null)
            {
                var piePage = document.AddPage();
                piePage.Size = PageSize;
                piePage.Orientation = PageOrientation.Landscape;
                var piePage_gfx = XGraphics.FromPdfPage(piePage);
                GenerateChartPage(piePage_gfx, pieChart);
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
            
            var font = new XFont("Calibri", 42.0, XFontStyle.Bold);
            //Get stringsize width
            XSize stringSize = gfx.MeasureString(Header, font);

            //Get rectangle height dependning on how long the string is. Otherwise the text wont wrap.
            double rectHeight;
            if (stringSize.Width > Width)
                rectHeight = font.GetHeight() + font.GetHeight();
            else rectHeight = font.GetHeight();

            var rect = new XRect(Margin, LogoRect.Bottom + 150, Width, rectHeight);
            
            CreateTextFormatter(gfx, XParagraphAlignment.Center).DrawString(Header, font, TextBrush, rect, XStringFormats.TopLeft);

        }

        private void GenerateChartPage(XGraphics gfx, Image chart)
        {
            //LogoRect = new XRect(PageWidth / 2 - ABECELogo.PointWidth / 2, Margin, ABECELogo.PointWidth, ABECELogo.PointHeight);
                        
            XImage ximg = XImage.FromGdiPlusImage(chart);
            gfx.DrawImage(ximg, PageWidthLandscape / 2 - ximg.PointWidth / 2, PageWidth / 2 - ximg.PointHeight / 2);
        }

        private static XTextFormatter CreateTextFormatter(XGraphics gfx, XParagraphAlignment alignment = XParagraphAlignment.Left)
        {
            return new XTextFormatter(gfx) { Alignment = alignment };
        }
    }
}
