using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition;
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

using Wordprocessing = DocumentFormat.OpenXml.Wordprocessing;

namespace GenbaseGoldReports.Infrastructure.Services.Export
{
    public class NetGoldPositionExportService : IReportExportService<NetGoldPositionReportDto>
    {
        private const string PrimaryGoldHex = "C59747";
        private const string DarkBannerHex = "1E293B";
        private const string AmberTotalHex = "FEF3C7";
        private const string NegativeRedHex = "DC2626";

        static NetGoldPositionExportService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // -------------------------------------------------------------
        // 1. EXCEL EXPORT (ClosedXML)
        // -------------------------------------------------------------
        public byte[] GenerateExcel(NetGoldPositionReportDto data, string tenantName)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Net Gold Position");
            ws.ShowGridLines = true;

            // Letterhead
            ws.Cell(1, 1).Value = tenantName.ToUpperInvariant();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 13;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml($"#{PrimaryGoldHex}");

            ws.Cell(1, 4).Value = $"Doc Ref: NGP-{data.AsOfDate:yyyyMMdd}";
            ws.Cell(1, 4).Style.Font.FontSize = 8;
            ws.Cell(1, 4).Style.Font.FontColor = XLColor.DimGray;
            ws.Cell(1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell(2, 1).Value = "Operational Metal Ledger Aggregation Scan";
            ws.Cell(2, 1).Style.Font.FontSize = 8.5;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.DimGray;

            ws.Cell(2, 4).Value = $"Run Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm} UTC";
            ws.Cell(2, 4).Style.Font.FontSize = 8;
            ws.Cell(2, 4).Style.Font.FontColor = XLColor.DimGray;
            ws.Cell(2, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Banner Ribbon
            var ribbon = ws.Range(4, 1, 4, 4);
            ribbon.Merge();
            ribbon.Value = "NET GOLD POSITION EXPOSURE";
            ribbon.Style.Font.Bold = true;
            ribbon.Style.Font.FontSize = 10;
            ribbon.Style.Font.FontColor = XLColor.White;
            ribbon.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{DarkBannerHex}");
            ribbon.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ribbon.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Row(4).Height = 22;

            // Scope Sub-Header
            var metaScope = ws.Range(5, 1, 5, 4);
            metaScope.Merge();
            metaScope.Value = $"As Of: {data.AsOfDate:dd-MM-yyyy}   |   Valuation Benchmark: {data.GoldRate:N2} /g";
            metaScope.Style.Font.Italic = true;
            metaScope.Style.Font.FontSize = 8.5;
            metaScope.Style.Font.FontColor = XLColor.FromHtml("#475569");
            metaScope.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers
            var headers = new[] { "REF", "EXECUTIVE CATEGORY", "PURE WEIGHT (G)", $"VALUATION ({data.GoldRate:N2}/G)" };
            int startRow = 7;
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(startRow, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 8.5;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = col >= 2 ? XLAlignmentHorizontalValues.Right : (col == 0 ? XLAlignmentHorizontalValues.Center : XLAlignmentHorizontalValues.Left);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#A17C34");
            }
            ws.Row(startRow).Height = 20;

            // Data Rows
            int currentRow = startRow + 1;
            foreach (var item in data.Items)
            {
                ws.Cell(currentRow, 1).SetValue(item.SlNo.ToString("00")).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(currentRow, 2).SetValue(item.Description).Style.Font.Bold = true;

                var pureCell = ws.Cell(currentRow, 3);
                pureCell.SetValue(item.PureWeight).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
                pureCell.Style.Font.Bold = true;

                var valCell = ws.Cell(currentRow, 4);
                valCell.SetValue(item.MetalValue).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
                valCell.Style.Font.Bold = true;

                if (item.PureWeight < 0) pureCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");
                if (item.MetalValue < 0) valCell.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

                for (int c = 1; c <= 4; c++)
                {
                    var cell = ws.Cell(currentRow, c);
                    cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.BottomBorderColor = XLColor.FromHtml("#F1F5F9");
                    if (currentRow % 2 == 0) cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FAFAF9");
                }
                currentRow++;
            }

            // Summary Totals
            var totalRange = ws.Range(currentRow, 1, currentRow, 4);
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml($"#{AmberTotalHex}");
            ws.Row(currentRow).Height = 22;

            ws.Cell(currentRow, 1).Value = "NET BULLION POSITION EXPOSURE";
            ws.Range(currentRow, 1, currentRow, 2).Merge();

            var totalPure = ws.Cell(currentRow, 3);
            totalPure.SetValue(data.TotalPureWeight).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
            if (data.TotalPureWeight < 0) totalPure.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

            var totalVal = ws.Cell(currentRow, 4);
            totalVal.SetValue(data.TotalMetalValue).Style.NumberFormat.Format = "#,##0.00;[Red]-#,##0.00";
            if (data.TotalMetalValue < 0) totalVal.Style.Font.FontColor = XLColor.FromHtml($"#{NegativeRedHex}");

            for (int c = 1; c <= 4; c++)
            {
                var cell = ws.Cell(currentRow, c);
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.TopBorderColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                cell.Style.Border.BottomBorderColor = XLColor.FromHtml($"#{PrimaryGoldHex}");
            }

            // Sign-offs
            int signRow = currentRow + 3;
            ws.Cell(signRow, 1).Value = "Prepared By: System Automated";
            ws.Cell(signRow, 1).Style.Font.Italic = true;
            ws.Cell(signRow, 1).Style.Font.FontSize = 8.5;

            ws.Cell(signRow, 3).Value = "Approved By: ___________________";
            ws.Cell(signRow, 3).Style.Font.Italic = true;
            ws.Cell(signRow, 3).Style.Font.FontSize = 8.5;

            ws.Cell(signRow, 4).Value = "Sign-off Date: _______________";
            ws.Cell(signRow, 4).Style.Font.Italic = true;
            ws.Cell(signRow, 4).Style.Font.FontSize = 8.5;
            ws.Cell(signRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Column(1).Width = 8;
            ws.Column(2).Width = 32;
            ws.Column(3).Width = 24;
            ws.Column(4).Width = 28;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // -------------------------------------------------------------
        // 2. PDF EXPORT (PDFsharp-MigraDoc) - Portrait A4 (515 pt printable)
        // -------------------------------------------------------------
        public byte[] GeneratePdf(NetGoldPositionReportDto data, string tenantName)
        {
            var document = new MigraDocDoc();
            var section = document.AddSection();

            section.PageSetup.Orientation = MigraDocOrientation.Portrait;
            section.PageSetup.PageFormat = MigraDocPageFormat.A4;
            section.PageSetup.LeftMargin = MigraDocUnit.FromPoint(40);
            section.PageSetup.RightMargin = MigraDocUnit.FromPoint(40);
            section.PageSetup.TopMargin = MigraDocUnit.FromPoint(36);
            section.PageSetup.BottomMargin = MigraDocUnit.FromPoint(36);

            var footerPara = section.Footers.Primary.AddParagraph();
            footerPara.Format.Font.Size = 7.5;
            footerPara.Format.Font.Color = MigraDocColors.Gray;
            footerPara.Format.Alignment = MigraDocParagraphAlignment.Center;
            footerPara.AddText("CONFIDENTIAL  |  Genbase Gold Enterprise System  |  Page ");
            footerPara.AddPageField();
            footerPara.AddText(" of ");
            footerPara.AddNumPagesField();

            // Letterhead: 325 pt + 190 pt = 515 pt
            var headTable = section.AddTable();
            headTable.AddColumn(MigraDocUnit.FromPoint(325));
            headTable.AddColumn(MigraDocUnit.FromPoint(190));

            var headRow = headTable.AddRow();
            var leftCell = headRow.Cells[0];
            var pTenant = leftCell.AddParagraph(tenantName.ToUpperInvariant());
            pTenant.Format.Font.Size = 13;
            pTenant.Format.Font.Bold = true;
            pTenant.Format.Font.Color = new MigraDocColor(197, 151, 71);

            var pSub = leftCell.AddParagraph("Operational Metal Ledger Aggregation Scan");
            pSub.Format.Font.Size = 8;
            pSub.Format.Font.Color = MigraDocColors.DarkSlateGray;

            var rightCell = headRow.Cells[1];
            rightCell.Format.Alignment = MigraDocParagraphAlignment.Right;
            var pRef = rightCell.AddParagraph($"Doc Ref: NGP-{data.AsOfDate:yyyyMMdd}");
            pRef.Format.Font.Size = 7.5;
            pRef.Format.Font.Color = MigraDocColors.Gray;
            var pRun = rightCell.AddParagraph($"Run Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm} UTC");
            pRun.Format.Font.Size = 7.5;
            pRun.Format.Font.Color = MigraDocColors.Gray;

            section.AddParagraph().Format.SpaceAfter = MigraDocUnit.FromPoint(6);

            var banner = section.AddParagraph("NET GOLD POSITION EXPOSURE");
            banner.Format.Font.Size = 9.5;
            banner.Format.Font.Bold = true;
            banner.Format.Font.Color = MigraDocColors.White;
            banner.Format.Alignment = MigraDocParagraphAlignment.Center;
            banner.Format.Shading.Color = new MigraDocColor(30, 41, 59);
            banner.Format.SpaceBefore = MigraDocUnit.FromPoint(3);
            banner.Format.SpaceAfter = MigraDocUnit.FromPoint(3);

            var meta = section.AddParagraph($"As Of: {data.AsOfDate:dd-MM-yyyy}   |   Valuation Benchmark: {data.GoldRate:N2} /g");
            meta.Format.Font.Size = 8;
            meta.Format.Font.Italic = true;
            meta.Format.Font.Color = MigraDocColors.SlateGray;
            meta.Format.Alignment = MigraDocParagraphAlignment.Center;
            meta.Format.SpaceAfter = MigraDocUnit.FromPoint(12);

            // Printable Width: 515 pt -> 45 + 190 + 130 + 150
            var table = section.AddTable();
            table.Borders.Width = 0.3;
            table.Borders.Color = new MigraDocColor(226, 232, 240);

            double[] colWidths = { 45, 190, 130, 150 };
            foreach (var w in colWidths) table.AddColumn(MigraDocUnit.FromPoint(w));

            var th = table.AddRow();
            th.Shading.Color = new MigraDocColor(197, 151, 71);
            th.HeadingFormat = true;
            th.Format.Font.Size = 8;
            th.Format.Font.Bold = true;
            th.Format.Font.Color = MigraDocColors.White;

            string[] headersArr = { "REF", "EXECUTIVE CATEGORY", "PURE WEIGHT (G)", $"VALUATION ({data.GoldRate:N2}/G)" };
            for (int i = 0; i < headersArr.Length; i++)
            {
                var cell = th.Cells[i];
                cell.AddParagraph(headersArr[i]);
                cell.Format.Alignment = i >= 2 ? MigraDocParagraphAlignment.Right : (i == 0 ? MigraDocParagraphAlignment.Center : MigraDocParagraphAlignment.Left);
            }

            bool alt = false;
            foreach (var item in data.Items)
            {
                var row = table.AddRow();
                row.Format.Font.Size = 8.5;
                if (alt) row.Shading.Color = new MigraDocColor(250, 250, 249);

                row.Cells[0].AddParagraph(item.SlNo.ToString("00")).Format.Alignment = MigraDocParagraphAlignment.Center;
                row.Cells[1].AddParagraph(item.Description).Format.Font.Bold = true;

                var pPure = row.Cells[2].AddParagraph(item.PureWeight.ToString("N2", CultureInfo.InvariantCulture));
                pPure.Format.Font.Bold = true;
                pPure.Format.Alignment = MigraDocParagraphAlignment.Right;
                if (item.PureWeight < 0) pPure.Format.Font.Color = new MigraDocColor(220, 38, 38);

                var pVal = row.Cells[3].AddParagraph(item.MetalValue.ToString("N2", CultureInfo.InvariantCulture));
                pVal.Format.Font.Bold = true;
                pVal.Format.Alignment = MigraDocParagraphAlignment.Right;
                if (item.MetalValue < 0) pVal.Format.Font.Color = new MigraDocColor(220, 38, 38);

                alt = !alt;
            }

            var tf = table.AddRow();
            tf.Shading.Color = new MigraDocColor(254, 243, 199);
            tf.Format.Font.Bold = true;
            tf.Format.Font.Size = 8.5;

            tf.Cells[0].AddParagraph("NET BULLION POSITION EXPOSURE");
            tf.Cells[0].MergeRight = 1;

            var tPure = tf.Cells[2].AddParagraph(data.TotalPureWeight.ToString("N2", CultureInfo.InvariantCulture));
            tPure.Format.Alignment = MigraDocParagraphAlignment.Right;
            if (data.TotalPureWeight < 0) tPure.Format.Font.Color = new MigraDocColor(220, 38, 38);

            var tVal = tf.Cells[3].AddParagraph(data.TotalMetalValue.ToString("N2", CultureInfo.InvariantCulture));
            tVal.Format.Alignment = MigraDocParagraphAlignment.Right;
            if (data.TotalMetalValue < 0) tVal.Format.Font.Color = new MigraDocColor(220, 38, 38);

            section.AddParagraph().Format.SpaceAfter = MigraDocUnit.FromPoint(24);

            // Sign-offs: 170 + 175 + 170 = 515 pt
            var signTable = section.AddTable();
            signTable.AddColumn(MigraDocUnit.FromPoint(170));
            signTable.AddColumn(MigraDocUnit.FromPoint(175));
            signTable.AddColumn(MigraDocUnit.FromPoint(170));

            var signRow = signTable.AddRow();
            signRow.Format.Font.Size = 8;
            signRow.Format.Font.Italic = true;
            signRow.Format.Font.Color = MigraDocColors.DarkSlateGray;

            signRow.Cells[0].AddParagraph("Prepared By: System Automated");
            signRow.Cells[1].AddParagraph("Approved By: ____________________").Format.Alignment = MigraDocParagraphAlignment.Center;
            signRow.Cells[2].AddParagraph("Sign-off Date: ____________").Format.Alignment = MigraDocParagraphAlignment.Right;

            var renderer = new PdfDocumentRenderer(unicode: true) { Document = document };
            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, closeStream: false);
            return stream.ToArray();
        }

        // -------------------------------------------------------------
        // 3. WORD EXPORT (DocumentFormat.OpenXml) - Portrait A4 (10,480 dxa)
        // -------------------------------------------------------------
        public byte[] GenerateWord(NetGoldPositionReportDto data, string tenantName)
        {
            using var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new Wordprocessing.Body());

                // Portrait A4 Setup
                var secPr = new Wordprocessing.SectionProperties(
                    new Wordprocessing.PageSize { Width = 11906U, Height = 16838U, Orient = Wordprocessing.PageOrientationValues.Portrait },
                    new Wordprocessing.PageMargin { Top = 720, Bottom = 720, Left = 720, Right = 720 }
                );
                body.AppendChild(secPr);

                // Corporate Letterhead
                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text(tenantName.ToUpperInvariant()))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Bold(), new Wordprocessing.FontSize { Val = "26" }, new Wordprocessing.Color { Val = PrimaryGoldHex })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "40" }) });

                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text("NET GOLD POSITION EXPOSURE"))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Bold(), new Wordprocessing.FontSize { Val = "20" })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "40" }) });

                body.AppendChild(new Wordprocessing.Paragraph(new Wordprocessing.Run(new Wordprocessing.Text($"As Of: {data.AsOfDate:dd-MM-yyyy}  |  Valuation Benchmark: {data.GoldRate:N2} /g"))
                {
                    RunProperties = new Wordprocessing.RunProperties(new Wordprocessing.Italic(), new Wordprocessing.FontSize { Val = "16" }, new Wordprocessing.Color { Val = "64748B" })
                })
                { ParagraphProperties = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { After = "160" }) });

                // Table Layout: 10,480 dxa total (900 + 3880 + 2800 + 2900)
                var table = new Wordprocessing.Table();
                var colWidths = new uint[] { 900, 3880, 2800, 2900 };

                var tblPr = new Wordprocessing.TableProperties(
                    new Wordprocessing.TableWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = "10480" },
                    new Wordprocessing.TableLayout { Type = Wordprocessing.TableLayoutValues.Fixed },
                    new Wordprocessing.TableCellMarginDefault(
                        new Wordprocessing.TopMargin { Width = "120", Type = Wordprocessing.TableWidthUnitValues.Dxa },
                        new Wordprocessing.BottomMargin { Width = "120", Type = Wordprocessing.TableWidthUnitValues.Dxa },
                        new Wordprocessing.TableCellLeftMargin { Width = 120, Type = Wordprocessing.TableWidthValues.Dxa },
                        new Wordprocessing.TableCellRightMargin { Width = 120, Type = Wordprocessing.TableWidthValues.Dxa }
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

                Wordprocessing.TableCell CreateWordCell(string text, uint width, bool isBold = false, bool isRight = false, bool isCenter = false, string? bgHex = null, string? textHex = null)
                {
                    var tcPr = new Wordprocessing.TableCellProperties(
                        new Wordprocessing.TableCellWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = width.ToString() },
                        new Wordprocessing.NoWrap()
                    );
                    if (!string.IsNullOrEmpty(bgHex)) tcPr.AppendChild(new Wordprocessing.Shading { Fill = bgHex });

                    var pPr = new Wordprocessing.ParagraphProperties(new Wordprocessing.SpacingBetweenLines { Before = "40", After = "40" });
                    if (isRight) pPr.AppendChild(new Wordprocessing.Justification { Val = Wordprocessing.JustificationValues.Right });
                    else if (isCenter) pPr.AppendChild(new Wordprocessing.Justification { Val = Wordprocessing.JustificationValues.Center });

                    var runPr = new Wordprocessing.RunProperties(new Wordprocessing.FontSize { Val = "17" });
                    if (isBold) runPr.AppendChild(new Wordprocessing.Bold());
                    if (!string.IsNullOrEmpty(textHex)) runPr.AppendChild(new Wordprocessing.Color { Val = textHex });
                    else if (bgHex == PrimaryGoldHex) runPr.AppendChild(new Wordprocessing.Color { Val = "FFFFFF" });

                    return new Wordprocessing.TableCell(tcPr, new Wordprocessing.Paragraph(pPr, new Wordprocessing.Run(runPr, new Wordprocessing.Text(text))));
                }

                // Table Header
                var trHeader = new Wordprocessing.TableRow();
                trHeader.AppendChild(new Wordprocessing.TableRowProperties(new Wordprocessing.TableHeader()));
                string[] headers = { "REF", "EXECUTIVE CATEGORY", "PURE WEIGHT (G)", $"VALUATION ({data.GoldRate:N2}/G)" };
                for (int i = 0; i < headers.Length; i++)
                {
                    trHeader.AppendChild(CreateWordCell(headers[i], colWidths[i], isBold: true, isRight: i >= 2, isCenter: i == 0, bgHex: PrimaryGoldHex));
                }
                table.AppendChild(trHeader);

                // Table Rows
                foreach (var row in data.Items)
                {
                    var tr = new Wordprocessing.TableRow();
                    tr.AppendChild(CreateWordCell(row.SlNo.ToString("00"), colWidths[0], isCenter: true));
                    tr.AppendChild(CreateWordCell(row.Description, colWidths[1], isBold: true));
                    tr.AppendChild(CreateWordCell(row.PureWeight.ToString("N2", CultureInfo.InvariantCulture), colWidths[2], isBold: true, isRight: true, textHex: row.PureWeight < 0 ? NegativeRedHex : null));
                    tr.AppendChild(CreateWordCell(row.MetalValue.ToString("N2", CultureInfo.InvariantCulture), colWidths[3], isBold: true, isRight: true, textHex: row.MetalValue < 0 ? NegativeRedHex : null));
                    table.AppendChild(tr);
                }

                // Total Summary Row
                var trTotal = new Wordprocessing.TableRow();
                var totalTitle = CreateWordCell("NET BULLION POSITION EXPOSURE", colWidths[0] + colWidths[1], isBold: true, bgHex: AmberTotalHex);
                totalTitle.TableCellProperties!.AppendChild(new Wordprocessing.GridSpan { Val = 2 });
                trTotal.AppendChild(totalTitle);

                trTotal.AppendChild(CreateWordCell(data.TotalPureWeight.ToString("N2", CultureInfo.InvariantCulture), colWidths[2], isBold: true, isRight: true, bgHex: AmberTotalHex, textHex: data.TotalPureWeight < 0 ? NegativeRedHex : null));
                trTotal.AppendChild(CreateWordCell(data.TotalMetalValue.ToString("N2", CultureInfo.InvariantCulture), colWidths[3], isBold: true, isRight: true, bgHex: AmberTotalHex, textHex: data.TotalMetalValue < 0 ? NegativeRedHex : null));
                table.AppendChild(trTotal);

                body.AppendChild(table);

                // Sign-off
                var signTable = new Wordprocessing.Table();
                var signTblPr = new Wordprocessing.TableProperties(
                    new Wordprocessing.TableWidth { Type = Wordprocessing.TableWidthUnitValues.Dxa, Width = "10480" },
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
                signGrid.AppendChild(new Wordprocessing.GridColumn { Width = "3493" });
                signGrid.AppendChild(new Wordprocessing.GridColumn { Width = "3494" });
                signGrid.AppendChild(new Wordprocessing.GridColumn { Width = "3493" });
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

                signRow.AppendChild(CreateSignCell("Prepared By: System Automated", string.Empty, Wordprocessing.JustificationValues.Left, 3493));
                signRow.AppendChild(CreateSignCell("Approved By: ", "                         ", Wordprocessing.JustificationValues.Center, 3494));
                signRow.AppendChild(CreateSignCell("Sign-off Date: ", "               ", Wordprocessing.JustificationValues.Right, 3493));

                signTable.AppendChild(signRow);
                body.AppendChild(signTable);

                mainPart.Document.Save();
            }

            return stream.ToArray();
        }
    }
}