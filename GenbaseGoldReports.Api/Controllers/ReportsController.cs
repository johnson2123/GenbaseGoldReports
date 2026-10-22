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
            var tenantName = !string.IsNullOrWhiteSpace(tenantContext.Identifier)
                ? tenantContext.Identifier
                : "Enterprise Node";

            var dateStamp = query.AsOfDate.ToString("yyyyMMdd");
            var requestedFormat = (format ?? "excel").Trim().ToLowerInvariant();

            Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

            return requestedFormat switch
            {
                "pdf" => File(
                    exportService.GeneratePdf(result, tenantName),
                    "application/pdf",
                    $"Stock_Balance_{dateStamp}.pdf"),

                "word" => File(
                    exportService.GenerateWord(result, tenantName),
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    $"Stock_Balance_{dateStamp}.docx"),

                "excel" or _ => File(
                    exportService.GenerateExcel(result, tenantName),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Stock_Balance_{dateStamp}.xlsx")
            };
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
            var tenantName = !string.IsNullOrWhiteSpace(tenantContext.Identifier)
                ? tenantContext.Identifier
                : "Enterprise Node";

            var dateStamp = query.AsOfDate.ToString("yyyyMMdd");
            var requestedFormat = (format ?? "excel").Trim().ToLowerInvariant();

            Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

            return requestedFormat switch
            {
                "pdf" => File(
                    exportService.GeneratePdf(result, tenantName),
                    "application/pdf",
                    $"Net_Gold_Position_{dateStamp}.pdf"),

                "word" => File(
                    exportService.GenerateWord(result, tenantName),
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    $"Net_Gold_Position_{dateStamp}.docx"),

                "excel" or _ => File(
                    exportService.GenerateExcel(result, tenantName),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Net_Gold_Position_{dateStamp}.xlsx")
            };
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
            var tenantName = !string.IsNullOrWhiteSpace(tenantContext.Identifier)
                ? tenantContext.Identifier
                : "Enterprise Node";

            var dateStamp = query.AsOfDate.ToString("yyyyMMdd");
            var requestedFormat = (format ?? "excel").Trim().ToLowerInvariant();

            Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

            return requestedFormat switch
            {
                "pdf" => File(
                    exportService.GeneratePdf(result, tenantName),
                    "application/pdf",
                    $"Trial_Balance_{dateStamp}.pdf"),

                "word" => File(
                    exportService.GenerateWord(result, tenantName),
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    $"Trial_Balance_{dateStamp}.docx"),

                "excel" or _ => File(
                    exportService.GenerateExcel(result, tenantName),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Trial_Balance_{dateStamp}.xlsx")
            };
        }
    }
}
