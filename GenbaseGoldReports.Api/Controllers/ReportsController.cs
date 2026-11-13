using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    [Authorize]
    public class ReportsController : BaseApiController
    {
        private readonly ISender _mediator;

        public ReportsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("stock-balance")]
        [ProducesResponseType(typeof(ApiResponse<StockBalanceReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStockBalance(
            [FromQuery] GetJewelleryStockBalanceQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            var response = ApiResponse<StockBalanceReportDto>.Success(
                data: result,
                message: "Stock Balance Report generated successfully."
            );

            return Ok(response);
        }

        [HttpGet("stock-balance/export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportStockBalance(
            [FromQuery] GetJewelleryStockBalanceQuery query,
            [FromQuery] string? format,
            [FromServices] IReportExportService<StockBalanceReportDto> exportService,
            [FromServices] ITenantContext tenantContext,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ExportFile(result, "Stock_Balance", query.AsOfDate, format, exportService, tenantContext);
        }

        [HttpGet("net-gold-position")]
        [ProducesResponseType(typeof(ApiResponse<NetGoldPositionReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNetGoldPosition(
            [FromQuery] GetNetGoldPositionQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            var response = ApiResponse<NetGoldPositionReportDto>.Success(
                data: result,
                message: "Net Gold Position Report generated successfully."
            );

            return Ok(response);
        }

        [HttpGet("net-gold-position/export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportNetGoldPosition(
            [FromQuery] GetNetGoldPositionQuery query,
            [FromQuery] string? format,
            [FromServices] IReportExportService<NetGoldPositionReportDto> exportService,
            [FromServices] ITenantContext tenantContext,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ExportFile(result, "Net_Gold_Position", query.AsOfDate, format, exportService, tenantContext);
        }

        [HttpGet("trial-balance")]
        [ProducesResponseType(typeof(ApiResponse<TrialBalanceReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTrialBalance(
            [FromQuery] GetTrialBalanceQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            var response = ApiResponse<TrialBalanceReportDto>.Success(
                data: result,
                message: "Hierarchical Trial Balance Report generated successfully."
            );

            return Ok(response);
        }

        [HttpGet("trial-balance/export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportTrialBalance(
            [FromQuery] GetTrialBalanceQuery query,
            [FromQuery] string? format,
            [FromServices] IReportExportService<TrialBalanceReportDto> exportService,
            [FromServices] ITenantContext tenantContext,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ExportFile(result, "Trial_Balance", query.AsOfDate, format, exportService, tenantContext);
        }

        private FileContentResult ExportFile<TDto>(
            TDto data,
            string baseFileName,
            DateTime asOfDate,
            string? format,
            IReportExportService<TDto> exportService,
            ITenantContext tenantContext)
        {
            var tenantName = !string.IsNullOrWhiteSpace(tenantContext.Identifier)
                ? tenantContext.Identifier
                : "Enterprise Node";

            var dateStamp = asOfDate.ToString("yyyyMMdd");
            var requestedFormat = (format ?? "excel").Trim().ToLowerInvariant();

            Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

            return requestedFormat switch
            {
                "pdf" => File(
                    exportService.GeneratePdf(data, tenantName),
                    "application/pdf",
                    $"{baseFileName}_{dateStamp}.pdf"),

                "word" => File(
                    exportService.GenerateWord(data, tenantName),
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.wordprocessingml.document",
                    $"{baseFileName}_{dateStamp}.docx"),

                _ => File(
                    exportService.GenerateExcel(data, tenantName),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{baseFileName}_{dateStamp}.xlsx")
            };
        }
    }
}
