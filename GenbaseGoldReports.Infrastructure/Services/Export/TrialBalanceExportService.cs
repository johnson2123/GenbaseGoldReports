using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance;
using MigraDoc.Rendering;
using System.Globalization;
using System.Text;

using MigraDocDoc = MigraDoc.DocumentObjectModel.Document;
using MigraDocColor = MigraDoc.DocumentObjectModel.Color;
using MigraDocColors = MigraDoc.DocumentObjectModel.Colors;
using MigraDocOrientation = MigraDoc.DocumentObjectModel.Orientation;
using MigraDocUnit = MigraDoc.DocumentObjectModel.Unit;
using MigraDocPageFormat = MigraDoc.DocumentObjectModel.PageFormat;
using MigraDocParagraphAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment;
using MigraDocUnderline = MigraDoc.DocumentObjectModel.Underline;

using Wordprocessing = DocumentFormat.OpenXml.Wordprocessing;

namespace GenbaseGoldReports.Infrastructure.Services.Export
{
    public class TrialBalanceExportService : IReportExportService<TrialBalanceReportDto>
    {
        private const string PrimaryGoldHex = "C59747";
        private const string DarkBannerHex = "1E293B";
        private const string AmberTotalHex = "FEF3C7";
        private const string SubTotalBlueHex = "EFF6FF";
        private const string SubTotalBorderHex = "BFDBFE";
        private const string NegativeRedHex = "DC2626";

        static TrialBalanceExportService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // -------------------------------------------------------------
        // 1. EXCEL EXPORT (ClosedXML)
        // -------------------------------------------------------------
        public byte[] GenerateExcel(TrialBalanceReportDto data, string tenantName)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Trial Balance");
            ws.ShowGridLines = true;

            // Corporate Letterhead
            ws.Cell(1, 1).Value = tenantName.ToUpperInvariant();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 13;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml($"#{PrimaryGoldHex}");

            ws.Cell(1, 5).Value = $"Doc Ref: TB-{data.AsOfDate:yyyyMMdd}";
            ws.Cell(1, 5).Style.Font.FontSize = 8;
            ws.Cell(1, 5).Style.Font.FontColor = XLColor.DimGray;
            ws.Range(1, 5, 1, 6).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell(2, 1).Value = "Hierarchical Chart-of-Accounts Valuation Ledger";
            ws.Cell(2, 1).Style.Font.FontSize = 8.5;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.DimGray;

            ws.Cell(2, 5).Value = $"Run Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm} UTC";
            ws.Cell(2, 5).Style.Font.FontSize = 8;
            ws.Cell(2, 5).Style.Font.FontColor = XLColor.DimGray;
            ws.Range(2, 5, 2, 6).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Ribbon Banner
            var ribbon = ws.Range(4, 1, 4, 6);
            ribbon.Merge();
            ribbon.Value = "HIERARCHICAL FINANCIAL & BULLION TRIAL BALANCE";
            ribbon.Style.Font.Bold = true;
            ribbon.Style.Font.FontSize = 10;
            ribbon.Style.Font.FontColor = XLColor.White;
            ribbon.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{DarkBannerHex}");
            ribbon.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ribbon.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Row(4).Height = 22;

            // Operational Valuation Scope
            var metaScope = ws.Range(5, 1, 5, 6);
            metaScope.Merge();
            metaScope.Value = $"As Of: {data.AsOfDate:dd-MM-yyyy}   |   Gold Benchmark: {data.GoldRate:N2} /g   |   Silver Benchmark: {data.SilverRate:N2} /g";
            metaScope.Style.Font.Italic = true;
            metaScope.Style.Font.FontSize = 8.5;
            metaScope.Style.Font.FontColor = XLColor.FromHtml("#475569");
            metaScope.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Table Headers
            var headers = new[]
            {
                "Account Classification / Title", "Code", "AED Amount",
                "Gold Gms", "Silver Gms", "Net Amount"
            };

            int startRow = 7;
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(startRow, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 8.5;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = col >= 2 ? XLAlignmentHorizontalValues.Right : (col == 1 ? XLAlignmentHorizontalValues.Center : XLAlignmentHorizontalValues.Left);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#A17C34");
            }
            ws.Row(startRow).Height = 20;

            // Data Rows
            int currentRow = startRow + 1;
            foreach (var item in data.Items)
            {
                var titleCell = ws.Cell(currentRow, 1);
                var codeCell = ws.Cell(currentRow, 2);
                var lcCell = ws.Cell(currentRow, 3);
                var goldCell = ws.Cell(currentRow, 4);
                var silverCell = ws.Cell(currentRow, 5);
                var netCell = ws.Cell(currentRow, 6);

                string indent = new string(' ', item.LevelId * 3);
                titleCell.Value = $"{indent}{item.AccountName}";

                if (item.IsAccount && !string.IsNullOrWhiteSpace(item.AccountCode) && item.AccountCode != "0")
                {
                    codeCell.SetValue(item.AccountCode).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                if (item.IsSubTotal)
                {
                    var subRow = ws.Range(currentRow, 1, currentRow, 6);
                    subRow.Style.Font.Bold = true;
                    subRow.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{SubTotalBlueHex}");
                    subRow.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    subRow.Style.Border.TopBorderColor = XLColor.FromHtml($"#{SubTotalBorderHex}");
                    subRow.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    subRow.Style.Border.BottomBorderColor = XLColor.FromHtml($"#{SubTotalBorderHex}");

                    titleCell.Style.Font.FontColor = XLColor.FromHtml("#1D4ED8");
                }
                else if (!item.IsAccount)
                {
                    // Section Headers
                    titleCell.Style.Font.Bold = true;
                    switch (item.LevelId)
                    {
                        case 0:
                            titleCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");
                            titleCell.Style.Font.FontSize = 10;
                            break;
                        case 1:
                            titleCell.Style.Font.FontColor = XLColor.FromHtml("#1D4ED8");
                            break;
                        case 2:
                            titleCell.Style.Font.FontColor = XLColor.FromHtml("#2563EB");
                            break;
                        default:
                            titleCell.Style.Font.FontColor = XLColor.FromHtml("#3B82F6");
                            break;
                    }
                }
                else
                {
                    // Leaf Accounts
                    for (int c = 1; c <= 6; c++)
                    {
                        ws.Cell(currentRow, c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(currentRow, c).Style.Border.BottomBorderColor = XLColor.FromHtml("#F8FAFC");
                    }
                }

                // Numerical Bindings
                if (item.LcAmount.HasValue)
                {
                    lcCell.SetValue(item.LcAmount.Value).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
                    if (item.LcAmount.Value < 0) lcCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");
                }

                if (item.GoldGrams.HasValue)
                {
                    goldCell.SetValue(item.GoldGrams.Value).Style.NumberFormat.Format = "#,##0.000;[Red]-#,##0.000";
                    if (item.GoldGrams.Value < 0) goldCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");
                }

                if (item.SilverGrams.HasValue)
                {
                    silverCell.SetValue(item.SilverGrams.Value).Style.NumberFormat.Format = "#,##0.000;[Red]-#,##0.000";
                    if (item.SilverGrams.Value < 0) silverCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");
                }

                if (item.NetAmount.HasValue)
                {
                    netCell.SetValue(item.NetAmount.Value).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
                    netCell.Style.Font.Bold = true;
                    if (item.NetAmount.Value < 0) netCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");
                }

                currentRow++;
            }

            // Report Grand Total Row
            var totalRange = ws.Range(currentRow, 1, currentRow, 6);
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{AmberTotalHex}");
            ws.Row(currentRow).Height = 22;

            ws.Cell(currentRow, 1).Value = "REPORT GRAND TOTAL";
            ws.Range(currentRow, 1, currentRow, 2).Merge();

            var totLc = ws.Cell(currentRow, 3);
            totLc.SetValue(data.TotalLcAmount).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
            if (data.TotalLcAmount < 0) totLc.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

            var totGold = ws.Cell(currentRow, 4);
            totGold.SetValue(data.TotalGoldGrams).Style.NumberFormat.Format = "#,##0.000;[Red]-#,##0.000";
            if (data.TotalGoldGrams < 0) totGold.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

            var totSilver = ws.Cell(currentRow, 5);
            totSilver.SetValue(data.TotalSilverGrams).Style.NumberFormat.Format = "#,##0.000;[Red]-#,##0.000";
            if (data.TotalSilverGrams < 0) totSilver.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

            var totNet = ws.Cell(currentRow, 6);
            totNet.SetValue(data.TotalNetAmount).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
            if (data.TotalNetAmount < 0) totNet.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

            for (int c = 1; c <= 6; c++)
            {
                var cell = ws.Cell(currentRow, c);
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.TopBorderColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                cell.Style.Border.BottomBorderColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
            }

            // Audit Sign-off
            int signRow = currentRow + 3;
            ws.Cell(signRow, 1).Value = "Prepared By: System Automated";
            ws.Cell(signRow, 1).Style.Font.Italic = true;
            ws.Cell(signRow, 1).Style.Font.FontSize = 8.5;

            ws.Cell(signRow, 3).Value = "Approved By: ___________________";
            ws.Cell(signRow, 3).Style.Font.Italic = true;
            ws.Cell(signRow, 3).Style.Font.FontSize = 8.5;

            ws.Cell(signRow, 6).Value = "Sign-off Date: _______________";
            ws.Cell(signRow, 6).Style.Font.Italic = true;
            ws.Cell(signRow, 6).Style.Font.FontSize = 8.5;
            ws.Cell(signRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Column(1).Width = 38;
            ws.Column(2).Width = 12;
            ws.Column(3).Width = 18;
            ws.Column(4).Width = 16;
            ws.Column(5).Width = 16;
            ws.Column(6).Width = 20;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // -------------------------------------------------------------
        // 2. PDF EXPORT (PDFsharp-MigraDoc) - Landscape A4 (800 pt)
        // -------------------------------------------------------------
        public byte[] GeneratePdf(TrialBalanceReportDto data, string tenantName)
        {
            var document = new MigraDocDoc();
            var section = document.AddSection();

            section.PageSetup.Orientation = MigraDocOrientation.Landscape;
            section.PageSetup.PageFormat = MigraDocPageFormat.A4;
            section.PageSetup.LeftMargin = MigraDocUnit.FromPoint(21);
            section.PageSetup.RightMargin = MigraDocUnit.FromPoint(21);
            section.PageSetup.TopMargin = MigraDocUnit.FromPoint(24);
            section.PageSetup.BottomMargin = MigraDocUnit.FromPoint(28);

            // Footers
            var footerPara = section.Footers.Primary.AddParagraph();
            footerPara.Format.Font.Size = 7;
            footerPara.Format.Font.Color = MigraDocColors.Gray;
            footerPara.Format.Alignment = MigraDocParagraphAlignment.Center;
            footerPara.AddText("CONFIDENTIAL  |  Genbase Gold Enterprise System  |  Page ");
            footerPara.AddPageField();
            footerPara.AddText(" of ");
            footerPara.AddNumPagesField();

            // Letterhead: 520 pt + 280 pt = 800 pt
            var headTable = section.AddTable();
            headTable.AddColumn(MigraDocUnit.FromPoint(520));
            headTable.AddColumn(MigraDocUnit.FromPoint(280));

            var headRow = headTable.AddRow();
            var leftCell = headRow.Cells[0];
            var pTenant = leftCell.AddParagraph(tenantName.ToUpperInvariant());
            pTenant.Format.Font.Size = 13;
            pTenant.Format.Font.Bold = true;
            pTenant.Format.Font.Color = new MigraDocColor(197, 151, 71);

            var pSub = leftCell.AddParagraph("Hierarchical Chart-of-Accounts Valuation Ledger");
            pSub.Format.Font.Size = 8;
            pSub.Format.Font.Color = MigraDocColors.DarkSlateGray;

            var rightCell = headRow.Cells[1];
            rightCell.Format.Alignment = MigraDocParagraphAlignment.Right;
            var pRef = rightCell.AddParagraph($"Doc Ref: TB-{data.AsOfDate:yyyyMMdd}");
            pRef.Format.Font.Size = 7.5;
            pRef.Format.Font.Color = MigraDocColors.Gray;
            var pRun = rightCell.AddParagraph($"Run Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm} UTC");
            pRun.Format.Font.Size = 7.5;
            pRun.Format.Font.Color = MigraDocColors.Gray;

            section.AddParagraph().Format.SpaceAfter = MigraDocUnit.FromPoint(4);

            // Banner Ribbon
            var banner = section.AddParagraph("HIERARCHICAL FINANCIAL & BULLION TRIAL BALANCE");
            banner.Format.Font.Size = 9.5;
            banner.Format.Font.Bold = true;
            banner.Format.Font.Color = MigraDocColors.White;
            banner.Format.Alignment = MigraDocParagraphAlignment.Center;
            banner.Format.Shading.Color = new MigraDocColor(30, 41, 59);
            banner.Format.SpaceBefore = MigraDocUnit.FromPoint(2);
            banner.Format.SpaceAfter = MigraDocUnit.FromPoint(3);

            var meta = section.AddParagraph($"As Of: {data.AsOfDate:dd-MM-yyyy}   |   Gold Rate: {data.GoldRate:N2} /g   |   Silver Rate: {data.SilverRate:N2} /g");
            meta.Format.Font.Size = 7.5;
            meta.Format.Font.Italic = true;
            meta.Format.Font.Color = MigraDocColors.SlateGray;
            meta.Format.Alignment = MigraDocParagraphAlignment.Center;
            meta.Format.SpaceAfter = MigraDocUnit.FromPoint(8);

            // Main Table: exactly 800 pt (280 + 50 + 115 + 115 + 110 + 130)
            var table = section.AddTable();
            table.Borders.Width = 0.3;
            table.Borders.Color = new MigraDocColor(226, 232, 240);

            double[] colWidths = { 280, 50, 115, 115, 110, 130 };
            foreach (var w in colWidths) table.AddColumn(MigraDocUnit.FromPoint(w));

            var th = table.AddRow();
            th.Shading.Color = new MigraDocColor(197, 151, 71);
            th.HeadingFormat = true;
            th.Format.Font.Size = 7.5;
            th.Format.Font.Bold = true;
            th.Format.Font.Color = MigraDocColors.White;

            string[] titles = { "Account Classification / Title", "Code", "AED Amount", "Gold Gms", "Silver Gms", "Net Amount" };
            for (int i = 0; i < titles.Length; i++)
            {
                var cell = th.Cells[i];
                cell.AddParagraph(titles[i]);
                cell.Format.Alignment = i >= 2 ? MigraDocParagraphAlignment.Right : (i == 1 ? MigraDocParagraphAlignment.Center : MigraDocParagraphAlignment.Left);
            }

            foreach (var item in data.Items)
            {
                var row = table.AddRow();
                row.Format.Font.Size = 7.5;

                var titleP = row.Cells[0].AddParagraph();
                double leftIndent = Math.Min(item.LevelId * 10.0, 40.0);
                titleP.Format.LeftIndent = MigraDocUnit.FromPoint(leftIndent);
                titleP.AddText(item.AccountName);

                if (item.IsSubTotal)
                {
                    row.Shading.Color = new MigraDocColor(239, 246, 255);
                    row.Format.Font.Bold = true;
                    titleP.Format.Font.Color = new MigraDocColor(29, 78, 216);
                }
                else if (!item.IsAccount)
                {
                    titleP.Format.Font.Bold = true;
                    switch (item.LevelId)
                    {
                        case 0:
                            titleP.Format.Font.Color = new MigraDocColor(220, 38, 38);
                            titleP.Format.Font.Size = 8;
                            break;
                        case 1:
                            titleP.Format.Font.Color = new MigraDocColor(29, 78, 216);
                            break;
                        case 2:
                            titleP.Format.Font.Color = new MigraDocColor(37, 99, 235);
                            break;
                        default:
                            titleP.Format.Font.Color = new MigraDocColor(59, 130, 246);
                            break;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item.AccountCode) && item.AccountCode != "0")
                    {
                        row.Cells[1].AddParagraph(item.AccountCode).Format.Alignment = MigraDocParagraphAlignment.Center;
                    }
                }

                if (item.LcAmount.HasValue)
                {
                    var p = row.Cells[2].AddParagraph(item.LcAmount.Value.ToString("N2", CultureInfo.InvariantCulture));
                    p.Format.Alignment = MigraDocParagraphAlignment.Right;
                    if (item.LcAmount.Value < 0) p.Format.Font.Color = new MigraDocColor(220, 38, 38);
                }

                if (item.GoldGrams.HasValue)
                {
                    var p = row.Cells[3].AddParagraph(item.GoldGrams.Value.ToString("N3", CultureInfo.InvariantCulture));
                    p.Format.Alignment = MigraDocParagraphAlignment.Right;
                    if (item.GoldGrams.Value < 0) p.Format.Font.Color = new MigraDocColor(220, 38, 38);
                }

                if (item.SilverGrams.HasValue)
                {
                    var p = row.Cells[4].AddParagraph(item.SilverGrams.Value.ToString("N3", CultureInfo.InvariantCulture));
                    p.Format.Alignment = MigraDocParagraphAlignment.Right;
                    if (item.SilverGrams.Value < 0) p.Format.Font.Color = new MigraDocColor(220, 38, 38);
                }

                if (item.NetAmount.HasValue)
                {
                    var p = row.Cells[5].AddParagraph(item.NetAmount.Value.ToString("N2", CultureInfo.InvariantCulture));
                    p.Format.Alignment = MigraDocParagraphAlignment.Right;
                    p.Format.Font.Bold = true;
                    if (item.NetAmount.Value < 0) p.Format.Font.Color = new MigraDocColor(220, 38, 38);
                }
            }

            // Summary Grand Total
            var tf = table.AddRow();
            tf.Shading.Color = new MigraDocColor(254, 243, 199);
            tf.Format.Font.Bold = true;
            tf.Format.Font.Size = 7.5;

            tf.Cells[0].AddParagraph("REPORT GRAND TOTAL");
            tf.Cells[0].MergeRight = 1;

            var pTotLc = tf.Cells[2].AddParagraph(data.TotalLcAmount.ToString("N2", CultureInfo.InvariantCulture));
            pTotLc.Format.Alignment = MigraDocParagraphAlignment.Right;
            if (data.TotalLcAmount < 0) pTotLc.Format.Font.Color = new MigraDocColor(220, 38, 38);

            var pTotGold = tf.Cells[3].AddParagraph(data.TotalGoldGrams.ToString("N3", CultureInfo.InvariantCulture));
            pTotGold.Format.Alignment = MigraDocParagraphAlignment.Right;
            if (data.TotalGoldGrams < 0) pTotGold.Format.Font.Color = new MigraDocColor(220, 38, 38);

            var pTotSilver = tf.Cells[4].AddParagraph(data.TotalSilverGrams.ToString("N3", CultureInfo.InvariantCulture));
            pTotSilver.Format.Alignment = MigraDocParagraphAlignment.Right;
            if (data.TotalSilverGrams < 0) pTotSilver.Format.Font.Color = new MigraDocColor(220, 38, 38);

            var pTotNet = tf.Cells[5].AddParagraph(data.TotalNetAmount.ToString("N2", CultureInfo.InvariantCulture));
            pTotNet.Format.Alignment = MigraDocParagraphAlignment.Right;
            pTotNet.Format.Font.Underline = MigraDocUnderline.Single;
            if (data.TotalNetAmount < 0) pTotNet.Format.Font.Color = new MigraDocColor(220, 38, 38);

            section.AddParagraph().Format.SpaceAfter = MigraDocUnit.FromPoint(16);

            // Audit Sign-off: 266 + 266 + 268 = 800 pt
            var signTable = section.AddTable();
            signTable.AddColumn(MigraDocUnit.FromPoint(266));
            signTable.AddColumn(MigraDocUnit.FromPoint(266));
            signTable.AddColumn(MigraDocUnit.FromPoint(268));

            var signRow = signTable.AddRow();
            signRow.Format.Font.Size = 7.5;
            signRow.Format.Font.Italic = true;
            signRow.Format.Font.Color = MigraDocColors.DarkSlateGray;

            signRow.Cells[0].AddParagraph("Prepared By: System Automated");
            signRow.Cells[1].AddParagraph("Approved By: ___________________________").Format.Alignment = MigraDocParagraphAlignment.Center;
            signRow.Cells[2].AddParagraph("Sign-off Date: ___________________").Format.Alignment = MigraDocParagraphAlignment.Right;

            var renderer = new PdfDocumentRenderer(unicode: true) { Document = document };
            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, closeStream: false);
            return stream.ToArray();
        }

        // -------------------------------------------------------------
        // 3. WORD EXPORT (DocumentFormat.OpenXml) - Landscape A4 (15,398 dxa)
        // -------------------------------------------------------------
        public byte[] GenerateWord(TrialBalanceReportDto data, string tenantName)
        {
            using var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new Wordprocessing.Body());

                // Landscape A4 Page Setup
                var secPr = new Wordprocessing.SectionProperties(
                    new Wordprocessing.PageSize { Width = 16838U, Height = 11906U, Orient = Wordprocessing.PageOrientationValues.Landscape },
                    new Wordprocessing.PageMargin { Top = 720, Bottom = 720, Left = 720, Right = 720 }
                );
                body.AppendChild(secPr);

                // Corporate Letterhead
                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text(tenantName.ToUpperInvariant()))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Bold(), new Wordprocessing.FontSize { Val = "26" }, new Wordprocessing.Color { Val = PrimaryGoldHex })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "40" }) });

                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text("HIERARCHICAL FINANCIAL & BULLION TRIAL BALANCE"))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Bold(), new Wordprocessing.FontSize { Val = "20" })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "40" }) });

                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text($"As Of: {data.AsOfDate:dd-MM-yyyy}  |  Gold Benchmark: {data.GoldRate:N2} /g  |  Silver Benchmark: {data.SilverRate:N2} /g"))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Italic(), new Wordprocessing.FontSize { Val = "16" }, new Wordprocessing.Color { Val = "64748B" })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "160" }) });

                // Table Layout: 15,398 dxa (5398 + 1000 + 2200 + 2200 + 2200 + 2400)
                var table = new Wordprocessing.Table();
                var colWidths = new uint[] { 5398, 1000, 2200, 2200, 2200, 2400 };

                var tblPr = new Wordprocessing.TableProperties(
                    new Wordprocessing.TableWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = "15398" },
                    new Wordprocessing.TableLayout { Type = Wordprocessing.TableLayoutValues.Fixed },
                    new Wordprocessing.TableCellMarginDefault(
                        new Wordprocessing.TopMargin { Width = "80", Type = Wordprocessing.TableWidthUnitValues.Dxa },
                        new Wordprocessing.BottomMargin { Width = "80", Type = Wordprocessing.TableWidthUnitValues.Dxa },
                        new Wordprocessing.TableCellLeftMargin { Width = 100, Type = Wordprocessing.TableWidthValues.Dxa },
                        new Wordprocessing.TableCellRightMargin { Width = 100, Type = Wordprocessing.TableWidthValues.Dxa }
                    ),
                    new Wordprocessing.TableBorders(
                        new Wordprocessing.TopBorder { Val = Wordprocessing.BorderValues.Single, Size = 4, Color = "CBD5E1" },
                        new Wordprocessing.BottomBorder { Val = Wordprocessing.BorderValues.Single, Size = 4, Color = "CBD5E1" },
                        new Wordprocessing.InsideHorizontalBorder { Val = Wordprocessing.BorderValues.Single, Size = 2, Color = "F1F5F9" },
                        new Wordprocessing.InsideVerticalBorder { Val = Wordprocessing.BorderValues.None }
                    )
                );
                table.AppendChild(tblPr);

                var tblGrid = new Wordprocessing.TableGrid();
                foreach (var w in colWidths) tblGrid.AppendChild(new Wordprocessing.GridColumn { Width = w.ToString() });
                table.AppendChild(tblGrid);

                Wordprocessing.TableCell CreateWordCell(
                    string text,
                    uint width,
                    bool isBold = false,
                    bool isRight = false,
                    bool isCenter = false,
                    string? bgHex = null,
                    string? textHex = null,
                    int indentLevel = 0)
                {
                    var tcPr = new Wordprocessing.TableCellProperties(
                        new Wordprocessing.TableCellWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = width.ToString() },
                        new Wordprocessing.NoWrap()
                    );
                    if (!string.IsNullOrEmpty(bgHex)) tcPr.AppendChild(new Wordprocessing.Shading { Fill = bgHex });

                    var pPr = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { Before = "30", After = "30" });
                    if (isRight) pPr.AppendChild(new Wordprocessing.Justification { Val = Wordprocessing.JustificationValues.Right });
                    else if (isCenter) pPr.AppendChild(new Wordprocessing.Justification { Val = Wordprocessing.JustificationValues.Center });

                    if (indentLevel > 0)
                    {
                        pPr.AppendChild(new Wordprocessing.Indentation { Left = (indentLevel * 180).ToString() });
                    }

                    var runPr = new Wordprocessing.RunProperties(new Wordprocessing.FontSize { Val = "15" });
                    if (isBold) runPr.AppendChild(new Wordprocessing.Bold());
                    if (!string.IsNullOrEmpty(textHex)) runPr.AppendChild(new Wordprocessing.Color { Val = textHex });
                    else if (bgHex == PrimaryGoldHex) runPr.AppendChild(new Wordprocessing.Color { Val = "FFFFFF" });

                    return new Wordprocessing.TableCell(tcPr, new Wordprocessing.Paragraph(pPr, new Wordprocessing.Run(runPr, new Wordprocessing.Text(text))));
                }

                // Table Header
                var trHeader = new Wordprocessing.TableRow();
                trHeader.AppendChild(new Wordprocessing.TableRowProperties(new Wordprocessing.TableHeader()));
                string[] headers = { "Account Classification / Title", "Code", "AED Amount", "Gold Gms", "Silver Gms", "Net Amount" };
                for (int i = 0; i < headers.Length; i++)
                {
                    trHeader.AppendChild(CreateWordCell(headers[i], colWidths[i], isBold: true, isRight: i >= 2, isCenter: i == 1, bgHex: PrimaryGoldHex));
                }
                table.AppendChild(trHeader);

                // Table Body
                foreach (var item in data.Items)
                {
                    var tr = new Wordprocessing.TableRow();

                    string? rowBg = item.IsSubTotal ? SubTotalBlueHex : null;
                    string? titleColor = null;

                    if (!item.IsAccount && !item.IsSubTotal)
                    {
                        titleColor = item.LevelId switch
                        {
                            0 => NegativeRedHex,
                            1 => "1D4ED8",
                            2 => "2563EB",
                            _ => "3B82F6"
                        };
                    }
                    else if (item.IsSubTotal)
                    {
                        titleColor = "1D4ED8";
                    }

                    tr.AppendChild(CreateWordCell(
                        item.AccountName,
                        colWidths[0],
                        isBold: !item.IsAccount || item.IsSubTotal,
                        bgHex: rowBg,
                        textHex: titleColor,
                        indentLevel: item.LevelId
                    ));

                    string codeText = item.IsAccount && !string.IsNullOrWhiteSpace(item.AccountCode) && item.AccountCode != "0"
                        ? item.AccountCode
                        : string.Empty;

                    tr.AppendChild(CreateWordCell(codeText, colWidths[1], isCenter: true, bgHex: rowBg));

                    string lcText = item.LcAmount.HasValue ? item.LcAmount.Value.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;
                    tr.AppendChild(CreateWordCell(lcText, colWidths[2], isBold: item.IsSubTotal, isRight: true, bgHex: rowBg, textHex: item.LcAmount < 0 ? NegativeRedHex : null));

                    string goldText = item.GoldGrams.HasValue ? item.GoldGrams.Value.ToString("N3", CultureInfo.InvariantCulture) : string.Empty;
                    tr.AppendChild(CreateWordCell(goldText, colWidths[3], isBold: item.IsSubTotal, isRight: true, bgHex: rowBg, textHex: item.GoldGrams < 0 ? NegativeRedHex : null));

                    string silverText = item.SilverGrams.HasValue ? item.SilverGrams.Value.ToString("N3", CultureInfo.InvariantCulture) : string.Empty;
                    tr.AppendChild(CreateWordCell(silverText, colWidths[4], isBold: item.IsSubTotal, isRight: true, bgHex: rowBg, textHex: item.SilverGrams < 0 ? NegativeRedHex : null));

                    string netText = item.NetAmount.HasValue ? item.NetAmount.Value.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;
                    tr.AppendChild(CreateWordCell(netText, colWidths[5], isBold: true, isRight: true, bgHex: rowBg, textHex: item.NetAmount < 0 ? NegativeRedHex : null));

                    table.AppendChild(tr);
                }

                // Summary Total Row
                var trTotal = new Wordprocessing.TableRow();
                var totalTitle = CreateWordCell("REPORT GRAND TOTAL", colWidths[0] + colWidths[1], isBold: true, bgHex: AmberTotalHex);
                totalTitle.TableCellProperties!.AppendChild(new Wordprocessing.GridSpan { Val = 2 });
                trTotal.AppendChild(totalTitle);

                trTotal.AppendChild(CreateWordCell(data.TotalLcAmount.ToString("N2", CultureInfo.InvariantCulture), colWidths[2], isBold: true, isRight: true, bgHex: AmberTotalHex, textHex: data.TotalLcAmount < 0 ? NegativeRedHex : null));
                trTotal.AppendChild(CreateWordCell(data.TotalGoldGrams.ToString("N3", CultureInfo.InvariantCulture), colWidths[3], isBold: true, isRight: true, bgHex: AmberTotalHex, textHex: data.TotalGoldGrams < 0 ? NegativeRedHex : null));
                trTotal.AppendChild(CreateWordCell(data.TotalSilverGrams.ToString("N3", CultureInfo.InvariantCulture), colWidths[4], isBold: true, isRight: true, bgHex: AmberTotalHex, textHex: data.TotalSilverGrams < 0 ? NegativeRedHex : null));
                trTotal.AppendChild(CreateWordCell(data.TotalNetAmount.ToString("N2", CultureInfo.InvariantCulture), colWidths[5], isBold: true, isRight: true, bgHex: AmberTotalHex, textHex: data.TotalNetAmount < 0 ? NegativeRedHex : null));
                table.AppendChild(trTotal);

                body.AppendChild(table);

                // Sign-off Table: 5132 + 5132 + 5134 = 15,398 dxa
                var signTable = new Wordprocessing.Table();
                var signTblPr = new Wordprocessing.TableProperties(
                    new Wordprocessing.TableWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = "15398" },
                    new Wordprocessing.TableLayout { Type = Wordprocessing.TableLayoutValues.Fixed },
                    new Wordprocessing.TableBorders(
                        new Wordprocessing.TopBorder { Val = Wordprocessing.BorderValues.None },
                        new Wordprocessing.BottomBorder { Val = Wordprocessing.BorderValues.None },
                        new Wordprocessing.LeftBorder { Val = Wordprocessing.BorderValues.None },
                        new Wordprocessing.RightBorder { Val = Wordprocessing.BorderValues.None },
                        new Wordprocessing.InsideHorizontalBorder { Val = Wordprocessing.BorderValues.None },
                        new Wordprocessing.InsideVerticalBorder { Val = Wordprocessing.BorderValues.None }
                    )
                );
                signTable.AppendChild(signTblPr);

                var signGrid = new Wordprocessing.TableGrid();
                signGrid.AppendChild(new Wordprocessing.GridColumn { Width = "5132" });
                signGrid.AppendChild(new Wordprocessing.GridColumn { Width = "5132" });
                signGrid.AppendChild(new Wordprocessing.GridColumn { Width = "5134" });
                signTable.AppendChild(signGrid);

                var signRow = new Wordprocessing.TableRow();

                Wordprocessing.TableCell CreateSignCell(string label, string underlineContent, Wordprocessing.JustificationValues align, uint width)
                {
                    var pPr = new Wordprocessing.ParagraphProperties(
                        new Wordprocessing.SpacingBetweenLines { Before = "360", After = "0" },
                        new Wordprocessing.Justification { Val = align }
                    );

                    var p = new Wordprocessing.Paragraph(pPr);
                    p.AppendChild(new Wordprocessing.Run(
                        new Wordprocessing.RunProperties(new Wordprocessing.Italic(), new Wordprocessing.FontSize { Val = "16" }, new Wordprocessing.Color { Val = "475569" }),
                        new Wordprocessing.Text(label)
                    ));

                    if (!string.IsNullOrEmpty(underlineContent))
                    {
                        p.AppendChild(new Wordprocessing.Run(
                            new Wordprocessing.RunProperties(new Wordprocessing.Underline { Val = Wordprocessing.UnderlineValues.Single }, new Wordprocessing.FontSize { Val = "16" }, new Wordprocessing.Color { Val = "94A3B8" }),
                            new Wordprocessing.Text(underlineContent)
                        ));
                    }

                    return new Wordprocessing.TableCell(
                        new Wordprocessing.TableCellProperties(new Wordprocessing.TableCellWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = width.ToString() }, new Wordprocessing.NoWrap()),
                        p
                    );
                }

                signRow.AppendChild(CreateSignCell("Prepared By: System Automated", string.Empty, Wordprocessing.JustificationValues.Left, 5132));
                signRow.AppendChild(CreateSignCell("Approved By: ", "                              ", Wordprocessing.JustificationValues.Center, 5132));
                signRow.AppendChild(CreateSignCell("Sign-off Date: ", "                    ", Wordprocessing.JustificationValues.Right, 5134));

                signTable.AppendChild(signRow);
                body.AppendChild(signTable);

                mainPart.Document.Save();
            }

            return stream.ToArray();
        }
    }
}