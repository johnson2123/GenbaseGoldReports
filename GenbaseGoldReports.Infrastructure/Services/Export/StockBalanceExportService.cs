using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance;
using MigraDoc.Rendering;
using System.Globalization;
using System.Text;

// Explicit Aliases to prevent namespace collisions
using MigraDocDoc = MigraDoc.DocumentObjectModel.Document;
using MigraDocColor = MigraDoc.DocumentObjectModel.Color;
using MigraDocColors = MigraDoc.DocumentObjectModel.Colors;
using MigraDocOrientation = MigraDoc.DocumentObjectModel.Orientation;
using MigraDocUnit = MigraDoc.DocumentObjectModel.Unit;
using MigraDocPageFormat = MigraDoc.DocumentObjectModel.PageFormat;
using MigraDocParagraphAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment;
using MigraDocTextFormat = MigraDoc.DocumentObjectModel.TextFormat;

using Wordprocessing = DocumentFormat.OpenXml.Wordprocessing;

namespace GenbaseGoldReports.Infrastructure.Services.Export
{
    public class StockBalanceExportService : IReportExportService<StockBalanceReportDto>
    {
        private const string PrimaryGoldHex = "C59747";
        private const string DarkBannerHex = "1E293B";
        private const string AmberTotalHex = "FEF3C7";

        static StockBalanceExportService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // -------------------------------------------------------------
        // 1. EXCEL EXPORT (ClosedXML - MIT)
        // -------------------------------------------------------------
        public byte[] GenerateExcel(StockBalanceReportDto data, string tenantName)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Stock Balance");
            ws.ShowGridLines = true;

            // Corporate Letterhead
            ws.Cell(1, 1).Value = tenantName.ToUpperInvariant();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml($"#{PrimaryGoldHex}");

            ws.Cell(1, 8).Value = $"Doc Ref: GB-STK-{data.AsOfDate:yyyyMMdd}";
            ws.Cell(1, 8).Style.Font.FontSize = 8;
            ws.Cell(1, 8).Style.Font.FontColor = XLColor.DimGray;
            ws.Range(1, 8, 1, 9).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell(2, 1).Value = "Jewellery Enterprise Management Suite";
            ws.Cell(2, 1).Style.Font.FontSize = 9;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.DimGray;

            ws.Cell(2, 8).Value = $"Run Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm} UTC";
            ws.Cell(2, 8).Style.Font.FontSize = 8;
            ws.Cell(2, 8).Style.Font.FontColor = XLColor.DimGray;
            ws.Range(2, 8, 2, 9).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Banner Ribbon
            var ribbon = ws.Range(4, 1, 4, 9);
            ribbon.Merge();
            ribbon.Value = "JEWELLERY STOCK BALANCE REGISTER";
            ribbon.Style.Font.Bold = true;
            ribbon.Style.Font.FontSize = 11;
            ribbon.Style.Font.FontColor = XLColor.White;
            ribbon.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{DarkBannerHex}");
            ribbon.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ribbon.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Row(4).Height = 24;

            // Operational Scope
            var metaScope = ws.Range(5, 1, 5, 9);
            metaScope.Merge();
            metaScope.Value = $"Scope: Live Vault CTE Path   |   As Of: {data.AsOfDate:dd-MM-yyyy}   |   Gold Benchmark: {data.GoldRate:N2} AED/g";
            metaScope.Style.Font.Italic = true;
            metaScope.Style.Font.FontSize = 8.5;
            metaScope.Style.Font.FontColor = XLColor.FromHtml("#475569");
            metaScope.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Column Headers
            var headers = new[]
            {
                "S.No", "Item Details", "Pieces", "Gross Wt (g)", "Purity",
                "Pure Wt (g)", "Making Cost", "Making Total", "Metal Value (AED)"
            };

            int startRow = 7;
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(startRow, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 9;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = col >= 2 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#A17C34");
            }
            ws.Row(startRow).Height = 20;

            // Data Rows
            int currentRow = startRow + 1;
            foreach (var item in data.Items)
            {
                ws.Cell(currentRow, 1).SetValue(item.SlNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(currentRow, 2).SetValue($"{item.ItemCode} - {item.ItemDescription}");

                ws.Cell(currentRow, 3).SetValue(item.Pieces).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 4).SetValue(item.GrossWeight).Style.NumberFormat.Format = "#,##0.000";
                ws.Cell(currentRow, 5).SetValue(item.Purity).Style.NumberFormat.Format = "0.0000";

                var pureCell = ws.Cell(currentRow, 6);
                pureCell.SetValue(item.PureWeight).Style.NumberFormat.Format = "#,##0.00";
                pureCell.Style.Font.Bold = true;

                ws.Cell(currentRow, 7).SetValue(item.MakingCost).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 8).SetValue(item.MakingAmount).Style.NumberFormat.Format = "#,##0.00";

                var valCell = ws.Cell(currentRow, 9);
                valCell.SetValue(item.MetalValue).Style.NumberFormat.Format = "#,##0.00";
                valCell.Style.Font.Bold = true;

                for (int c = 1; c <= 9; c++)
                {
                    var cell = ws.Cell(currentRow, c);
                    cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.BottomBorderColor = XLColor.FromHtml("#F1F5F9");
                    if (currentRow % 2 == 0)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FAFAF9");
                    }
                }

                currentRow++;
            }

            // Summary Totals Row (Confined to Columns 1 through 9)
            var totalRange = ws.Range(currentRow, 1, currentRow, 9);
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{AmberTotalHex}");
            ws.Row(currentRow).Height = 22;

            ws.Cell(currentRow, 1).Value = "SUMMARY TOTAL";
            ws.Range(currentRow, 1, currentRow, 2).Merge();

            ws.Cell(currentRow, 3).SetValue(data.TotalPieces).Style.NumberFormat.Format = "#,##0";
            ws.Cell(currentRow, 4).SetValue(data.TotalGrossWeight).Style.NumberFormat.Format = "#,##0.000";
            ws.Cell(currentRow, 6).SetValue(data.TotalPureWeight).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(currentRow, 8).SetValue(data.TotalMakingAmount).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(currentRow, 9).SetValue(data.TotalMetalValue).Style.NumberFormat.Format = "#,##0.00";

            for (int c = 1; c <= 9; c++)
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

            ws.Cell(signRow, 5).Value = "Approved By: ___________________";
            ws.Cell(signRow, 5).Style.Font.Italic = true;
            ws.Cell(signRow, 5).Style.Font.FontSize = 8.5;

            ws.Cell(signRow, 8).Value = "Sign-off Date: _______________";
            ws.Cell(signRow, 8).Style.Font.Italic = true;
            ws.Cell(signRow, 8).Style.Font.FontSize = 8.5;

            ws.Columns().AdjustToContents();
            ws.Column(1).Width = 8;
            ws.Column(2).Width = 32;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // -------------------------------------------------------------
        // 2. PDF EXPORT (PDFsharp-MigraDoc 1.50.5147 - Pure MIT)
        // -------------------------------------------------------------
        public byte[] GeneratePdf(StockBalanceReportDto data, string tenantName)
        {
            var document = new MigraDocDoc();
            var section = document.AddSection();

            // Total A4 Landscape Width: 842 pt. Margins: 21 pt each -> Printable: exactly 800 pt.
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
            footerPara.AddText("CONFIDENTIAL - FOR INTERNAL AUDIT ONLY  |  Genbase Gold Enterprise System  |  Page ");
            footerPara.AddPageField();
            footerPara.AddText(" of ");
            footerPara.AddNumPagesField();

            // Letterhead (Exactly 800 pt: 520 + 280)
            var headTable = section.AddTable();
            headTable.AddColumn(MigraDocUnit.FromPoint(520));
            headTable.AddColumn(MigraDocUnit.FromPoint(280));

            var headRow = headTable.AddRow();
            var leftCell = headRow.Cells[0];
            var pTenant = leftCell.AddParagraph(tenantName.ToUpperInvariant());
            pTenant.Format.Font.Size = 13;
            pTenant.Format.Font.Bold = true;
            pTenant.Format.Font.Color = new MigraDocColor(197, 151, 71);

            var pSub = leftCell.AddParagraph("Jewellery Enterprise Management Suite");
            pSub.Format.Font.Size = 8;
            pSub.Format.Font.Color = MigraDocColors.DarkSlateGray;

            var rightCell = headRow.Cells[1];
            rightCell.Format.Alignment = MigraDocParagraphAlignment.Right;
            var pRef = rightCell.AddParagraph($"Doc Ref: GB-STK-{data.AsOfDate:yyyyMMdd}");
            pRef.Format.Font.Size = 7.5;
            pRef.Format.Font.Color = MigraDocColors.Gray;
            var pRun = rightCell.AddParagraph($"Run Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm} UTC");
            pRun.Format.Font.Size = 7.5;
            pRun.Format.Font.Color = MigraDocColors.Gray;

            section.AddParagraph().Format.SpaceAfter = MigraDocUnit.FromPoint(4);

            // Banner Ribbon (Spans the 800 pt page width)
            var banner = section.AddParagraph("JEWELLERY STOCK BALANCE REGISTER");
            banner.Format.Font.Size = 9.5;
            banner.Format.Font.Bold = true;
            banner.Format.Font.Color = MigraDocColors.White;
            banner.Format.Alignment = MigraDocParagraphAlignment.Center;
            banner.Format.Shading.Color = new MigraDocColor(30, 41, 59);
            banner.Format.SpaceBefore = MigraDocUnit.FromPoint(2);
            banner.Format.SpaceAfter = MigraDocUnit.FromPoint(3);

            // Meta Scope Line
            var meta = section.AddParagraph($"Scope: Active Vault CTE Path   |   As Of: {data.AsOfDate:dd-MM-yyyy}   |   Gold Benchmark: {data.GoldRate:N2} AED/g");
            meta.Format.Font.Size = 7.5;
            meta.Format.Font.Italic = true;
            meta.Format.Font.Color = MigraDocColors.SlateGray;
            meta.Format.Alignment = MigraDocParagraphAlignment.Center;
            meta.Format.SpaceAfter = MigraDocUnit.FromPoint(8);

            // Main Table (Sum = exactly 800 pt: 25 + 205 + 45 + 85 + 65 + 85 + 75 + 85 + 130)
            var table = section.AddTable();
            table.Borders.Width = 0.3;
            table.Borders.Color = new MigraDocColor(226, 232, 240);

            double[] colWidths = { 25, 205, 45, 85, 65, 85, 75, 85, 130 };
            foreach (var w in colWidths)
            {
                table.AddColumn(MigraDocUnit.FromPoint(w));
            }

            var th = table.AddRow();
            th.Shading.Color = new MigraDocColor(197, 151, 71);
            th.HeadingFormat = true;
            th.Format.Font.Size = 7.5;
            th.Format.Font.Bold = true;
            th.Format.Font.Color = MigraDocColors.White;

            string[] titles = { "S.No", "Item Details", "Pieces", "Gross Wt (g)", "Purity", "Pure Wt (g)", "Making Cost", "Making Total", "Metal Value (AED)" };
            for (int i = 0; i < titles.Length; i++)
            {
                var cell = th.Cells[i];
                cell.AddParagraph(titles[i]);
                cell.Format.Alignment = i >= 2 ? MigraDocParagraphAlignment.Right : MigraDocParagraphAlignment.Left;
                if (i == 0) cell.Format.Alignment = MigraDocParagraphAlignment.Center;
            }

            bool alt = false;
            foreach (var item in data.Items)
            {
                var row = table.AddRow();
                row.Format.Font.Size = 7.5;
                if (alt) row.Shading.Color = new MigraDocColor(250, 250, 249);

                row.Cells[0].AddParagraph(item.SlNo.ToString(CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Center;

                var cellDesc = row.Cells[1].AddParagraph();
                var rCode = cellDesc.AddFormattedText(item.ItemCode, MigraDocTextFormat.Bold);
                rCode.Font.Size = 7.5;
                cellDesc.AddText($"\n{item.ItemDescription}");
                cellDesc.Format.Font.Size = 6.5;
                cellDesc.Format.Font.Color = MigraDocColors.DarkSlateGray;

                row.Cells[2].AddParagraph(item.Pieces.ToString("N0", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
                row.Cells[3].AddParagraph(item.GrossWeight.ToString("N3", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
                row.Cells[4].AddParagraph(item.Purity.ToString("0.0000", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;

                var pureP = row.Cells[5].AddParagraph(item.PureWeight.ToString("N2", CultureInfo.InvariantCulture));
                pureP.Format.Font.Bold = true;
                pureP.Format.Alignment = MigraDocParagraphAlignment.Right;

                row.Cells[6].AddParagraph(item.MakingCost.ToString("N2", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
                row.Cells[7].AddParagraph(item.MakingAmount.ToString("N2", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;

                var valP = row.Cells[8].AddParagraph(item.MetalValue.ToString("N2", CultureInfo.InvariantCulture));
                valP.Format.Font.Bold = true;
                valP.Format.Alignment = MigraDocParagraphAlignment.Right;

                alt = !alt;
            }

            var tf = table.AddRow();
            tf.Shading.Color = new MigraDocColor(254, 243, 199);
            tf.Format.Font.Bold = true;
            tf.Format.Font.Size = 7.5;

            tf.Cells[0].AddParagraph("SUMMARY TOTAL");
            tf.Cells[0].MergeRight = 1;

            tf.Cells[2].AddParagraph(data.TotalPieces.ToString("N0", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
            tf.Cells[3].AddParagraph(data.TotalGrossWeight.ToString("N3", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
            tf.Cells[5].AddParagraph(data.TotalPureWeight.ToString("N2", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
            tf.Cells[7].AddParagraph(data.TotalMakingAmount.ToString("N2", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;
            tf.Cells[8].AddParagraph(data.TotalMetalValue.ToString("N2", CultureInfo.InvariantCulture)).Format.Alignment = MigraDocParagraphAlignment.Right;

            section.AddParagraph().Format.SpaceAfter = MigraDocUnit.FromPoint(16);

            // Audit Sign-off (Exactly 800 pt: 266 + 266 + 268)
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

            var renderer = new PdfDocumentRenderer(unicode: true)
            {
                Document = document
            };
            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, closeStream: false);
            return stream.ToArray();
        }

        // -------------------------------------------------------------
        // 3. WORD EXPORT (DocumentFormat.OpenXml - MIT)
        // -------------------------------------------------------------
        public byte[] GenerateWord(StockBalanceReportDto data, string tenantName)
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
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "60" }) });

                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text("JEWELLERY STOCK BALANCE REGISTER"))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Bold(), new Wordprocessing.FontSize { Val = "20" })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "60" }) });

                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text($"Scope: Active CTE Path  |  As Of: {data.AsOfDate:dd-MM-yyyy}  |  Gold Benchmark: {data.GoldRate:N2} AED/g"))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Italic(), new Wordprocessing.FontSize { Val = "16" }, new Wordprocessing.Color { Val = "64748B" })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "180" }) });

                // Main Word Table
                var table = new Wordprocessing.Table();
                var colWidths = new uint[] { 650, 3448, 900, 1800, 1500, 1800, 1400, 1600, 2300 };

                var tblPr = new Wordprocessing.TableProperties(
                    new Wordprocessing.TableWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = "15398" },
                    new Wordprocessing.TableLayout { Type = Wordprocessing.TableLayoutValues.Fixed },
                    new Wordprocessing.TableCellMarginDefault(
                        new Wordprocessing.TopMargin { Width = "100", Type = Wordprocessing.TableWidthUnitValues.Dxa },
                        new Wordprocessing.BottomMargin { Width = "100", Type = Wordprocessing.TableWidthUnitValues.Dxa },
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

                Wordprocessing.TableCell CreateWordCell(string text, uint width, bool isBold = false, bool isRight = false, bool isCenter = false, string? bgHex = null)
                {
                    var tcPr = new Wordprocessing.TableCellProperties(
                        new Wordprocessing.TableCellWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = width.ToString() },
                        new Wordprocessing.NoWrap()
                    );
                    if (!string.IsNullOrEmpty(bgHex))
                    {
                        tcPr.AppendChild(new Wordprocessing.Shading { Fill = bgHex });
                    }

                    var pPr = new Wordprocessing.ParagraphProperties(
                        new Wordprocessing.SpacingBetweenLines { Before = "40", After = "40" }
                    );
                    if (isRight)
                    {
                        pPr.AppendChild(new Wordprocessing.Justification { Val = Wordprocessing.JustificationValues.Right });
                    }
                    else if (isCenter)
                    {
                        pPr.AppendChild(new Wordprocessing.Justification { Val = Wordprocessing.JustificationValues.Center });
                    }

                    var runPr = new Wordprocessing.RunProperties(new Wordprocessing.FontSize { Val = "16" });
                    if (isBold) runPr.AppendChild(new Wordprocessing.Bold());
                    if (bgHex == PrimaryGoldHex) runPr.AppendChild(new Wordprocessing.Color { Val = "FFFFFF" });

                    var p = new Wordprocessing.Paragraph(pPr, new Wordprocessing.Run(runPr, new Wordprocessing.Text(text)));
                    return new Wordprocessing.TableCell(tcPr, p);
                }

                // Header Row
                var trHeader = new Wordprocessing.TableRow();
                trHeader.AppendChild(new Wordprocessing.TableRowProperties(new Wordprocessing.TableHeader()));
                string[] headers = { "S.No", "Item Details", "Pieces", "Gross Wt (g)", "Purity", "Pure Wt (g)", "Making Cost", "Making Total", "Metal Value (AED)" };
                for (int i = 0; i < headers.Length; i++)
                {
                    trHeader.AppendChild(CreateWordCell(headers[i], colWidths[i], isBold: true, isRight: i >= 2, isCenter: i == 0, bgHex: PrimaryGoldHex));
                }
                table.AppendChild(trHeader);

                // Data Rows
                foreach (var row in data.Items)
                {
                    var tr = new Wordprocessing.TableRow();
                    tr.AppendChild(CreateWordCell(row.SlNo.ToString(CultureInfo.InvariantCulture), colWidths[0], isCenter: true));
                    tr.AppendChild(CreateWordCell($"{row.ItemCode} - {row.ItemDescription}", colWidths[1]));
                    tr.AppendChild(CreateWordCell(row.Pieces.ToString("N0", CultureInfo.InvariantCulture), colWidths[2], isRight: true));
                    tr.AppendChild(CreateWordCell(row.GrossWeight.ToString("N3", CultureInfo.InvariantCulture), colWidths[3], isRight: true));
                    tr.AppendChild(CreateWordCell(row.Purity.ToString("0.0000", CultureInfo.InvariantCulture), colWidths[4], isRight: true));
                    tr.AppendChild(CreateWordCell(row.PureWeight.ToString("N2", CultureInfo.InvariantCulture), colWidths[5], isBold: true, isRight: true));
                    tr.AppendChild(CreateWordCell(row.MakingCost.ToString("N2", CultureInfo.InvariantCulture), colWidths[6], isRight: true));
                    tr.AppendChild(CreateWordCell(row.MakingAmount.ToString("N2", CultureInfo.InvariantCulture), colWidths[7], isRight: true));
                    tr.AppendChild(CreateWordCell(row.MetalValue.ToString("N2", CultureInfo.InvariantCulture), colWidths[8], isBold: true, isRight: true));
                    table.AppendChild(tr);
                }

                // Summary Totals Row (Span over columns 1 & 2)
                var trTotal = new Wordprocessing.TableRow();

                var totalTitleCell = CreateWordCell("SUMMARY TOTAL", colWidths[0] + colWidths[1], isBold: true, bgHex: AmberTotalHex);
                totalTitleCell.TableCellProperties!.AppendChild(new Wordprocessing.GridSpan { Val = 2 });
                trTotal.AppendChild(totalTitleCell);

                trTotal.AppendChild(CreateWordCell(data.TotalPieces.ToString("N0", CultureInfo.InvariantCulture), colWidths[2], isBold: true, isRight: true, bgHex: AmberTotalHex));
                trTotal.AppendChild(CreateWordCell(data.TotalGrossWeight.ToString("N3", CultureInfo.InvariantCulture), colWidths[3], isBold: true, isRight: true, bgHex: AmberTotalHex));
                trTotal.AppendChild(CreateWordCell(string.Empty, colWidths[4], bgHex: AmberTotalHex));
                trTotal.AppendChild(CreateWordCell(data.TotalPureWeight.ToString("N2", CultureInfo.InvariantCulture), colWidths[5], isBold: true, isRight: true, bgHex: AmberTotalHex));
                trTotal.AppendChild(CreateWordCell(string.Empty, colWidths[6], bgHex: AmberTotalHex));
                trTotal.AppendChild(CreateWordCell(data.TotalMakingAmount.ToString("N2", CultureInfo.InvariantCulture), colWidths[7], isBold: true, isRight: true, bgHex: AmberTotalHex));
                trTotal.AppendChild(CreateWordCell(data.TotalMetalValue.ToString("N2", CultureInfo.InvariantCulture), colWidths[8], isBold: true, isRight: true, bgHex: AmberTotalHex));
                table.AppendChild(trTotal);

                body.AppendChild(table);

                // Full-Width Balanced Audit Sign-Off Section
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
                        new Wordprocessing.TableCellProperties(
                            new Wordprocessing.TableCellWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = width.ToString() },
                            new Wordprocessing.NoWrap()
                        ),
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