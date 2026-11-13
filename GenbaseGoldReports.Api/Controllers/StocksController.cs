using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Commands.CreateMetalStock;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Commands.UpdateMetalStock;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockById;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockFormInit;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    [Authorize]
    [Route("api/masters/stocks")]
    public class StocksController : BaseApiController
    {
        private readonly ISender _mediator;

        public StocksController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("init")]
        [ProducesResponseType(typeof(ApiResponse<MetalStockFormInitDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMetalStockFormInit(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMetalStockFormInitQuery(), cancellationToken);

            return Ok(ApiResponse<MetalStockFormInitDto>.Success(
                data: result,
                message: "Metal stock initialization data retrieved successfully."
            ));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MetalStockListItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMetalStockList(
            [FromQuery] GetMetalStockListQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<PagedResult<MetalStockListItemDto>>.Success(
                data: result,
                message: "Metal stock list retrieved successfully."
            ));
        }

        [HttpGet("{stkcode}")]
        [ProducesResponseType(typeof(ApiResponse<MetalStockDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMetalStockById(
            [FromRoute] string stkcode,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMetalStockByIdQuery(stkcode), cancellationToken);

            return Ok(ApiResponse<MetalStockDetailDto>.Success(
                data: result,
                message: "Metal stock details retrieved successfully."
            ));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateMetalStock(
            [FromBody] CreateMetalStockCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, ApiResponse<string>.Success(
                data: result,
                message: "Metal stock item created successfully."
            ));
        }

        [HttpPut("{stkcode}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMetalStock(
            [FromRoute] string stkcode,
            [FromBody] UpdateMetalStockCommand command,
            CancellationToken cancellationToken)
        {
            if (stkcode != command.Stkcode)
            {
                return BadRequest(ApiResponse<object>.Failure(
                    "Item code in route parameter does not match the request payload.",
                    "Validation Error"
                ));
            }

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<bool>.Success(
                data: result,
                message: "Metal stock item updated successfully."
            ));
        }
    }
}
