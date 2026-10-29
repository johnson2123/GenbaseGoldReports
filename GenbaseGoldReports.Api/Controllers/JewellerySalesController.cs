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
    public class JewellerySalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JewellerySalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("init")]
        [ProducesResponseType(typeof(JewellerySaleInitDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<JewellerySaleInitDto>> GetInitData(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetJewellerySaleInitQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("party/{accode}")]
        [ProducesResponseType(typeof(PartyDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PartyDetailsDto>> GetPartyDetails(string accode, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPartyDetailsQuery(accode), cancellationToken);
            if (result == null)
                return NotFound(new { message = $"Party with code '{accode}' not found." });

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(JewellerySalesLedgerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<JewellerySalesLedgerResponse>> GetSalesLedger(
            [FromQuery] GetJewellerySalesLedgerQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{docno:int}")]
        [ProducesResponseType(typeof(JewellerySaleDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JewellerySaleDetailsDto>> GetSaleById(
            int docno,
            [FromQuery] string? doctype = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetJewellerySaleByIdQuery(docno, doctype), cancellationToken);
            if (result == null)
                return NotFound(new { message = $"Voucher with DocNo {docno} was not found." });

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateJewellerySaleResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateJewellerySaleResult>> CreateSale(
            [FromBody] CreateJewellerySaleCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSaleById), new { docno = result.Docno }, result);
        }
    }

}
