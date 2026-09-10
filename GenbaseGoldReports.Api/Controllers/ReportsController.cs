using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition;
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
    }
}
