using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Sales.Commands.CreateJewellerySale;
using GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleById;
using GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleInit;
using GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySalesLedger;
using GenbaseGoldReports.Application.Features.Sales.Queries.GetPartyDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    [Authorize]
    [Route("api/sales/jewellery")]
    public class JewellerySalesController : BaseApiController
    {
        private readonly ISender _mediator;

        public JewellerySalesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("init")]
        [ProducesResponseType(typeof(ApiResponse<JewellerySaleInitDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInitData(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetJewellerySaleInitQuery(), cancellationToken);
            return Ok(ApiResponse<JewellerySaleInitDto>.Success(result, "Initialization data loaded successfully."));
        }

        [HttpGet("party/{accode}")]
        [ProducesResponseType(typeof(ApiResponse<PartyDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPartyDetails(string accode, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPartyDetailsQuery(accode), cancellationToken);
            if (result == null)
                return NotFound(ApiResponse<object>.Failure($"Party with code '{accode}' not found."));

            return Ok(ApiResponse<PartyDetailsDto>.Success(result, "Party details loaded successfully."));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<JewellerySalesLedgerResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalesLedger(
            [FromQuery] GetJewellerySalesLedgerQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(ApiResponse<JewellerySalesLedgerResponse>.Success(result, "Sales ledger retrieved successfully."));
        }

        [HttpGet("{docno:int}")]
        [ProducesResponseType(typeof(ApiResponse<JewellerySaleDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaleById(
            int docno,
            [FromQuery] string? doctype = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetJewellerySaleByIdQuery(docno, doctype), cancellationToken);
            if (result == null)
                return NotFound(ApiResponse<object>.Failure($"Voucher with DocNo {docno} was not found."));

            return Ok(ApiResponse<JewellerySaleDetailsDto>.Success(result, "Voucher details retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreateJewellerySaleResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale(
            [FromBody] CreateJewellerySaleCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            var response = ApiResponse<CreateJewellerySaleResult>.Success(result, "Jewellery sale posted successfully.");
            return CreatedAtAction(nameof(GetSaleById), new { docno = result.Docno }, response);
        }
    }

}
