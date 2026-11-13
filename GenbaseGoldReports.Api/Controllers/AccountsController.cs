using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Commands.CreateAccount;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Queries.GetAccountFormInit;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Queries.GetAccountsTree;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    [Authorize]
    [Route("api/masters/accounts")]
    public class AccountsController : BaseApiController
    {
        private readonly ISender _mediator;

        public AccountsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("tree")]
        [ProducesResponseType(typeof(ApiResponse<List<AccountTreeItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccountsTree(
            [FromQuery] string? search,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAccountsTreeQuery(search), cancellationToken);

            var response = ApiResponse<List<AccountTreeItemDto>>.Success(
                data: result,
                message: "Accounts tree hierarchy retrieved successfully."
            );

            return Ok(response);
        }

        [HttpGet("init")]
        [ProducesResponseType(typeof(ApiResponse<AccountFormInitDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccountFormInit(
            [FromQuery] string? parentCode,
            [FromQuery] bool isGroup = false,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAccountFormInitQuery(parentCode, isGroup), cancellationToken);

            var response = ApiResponse<AccountFormInitDto>.Success(
                data: result,
                message: "Account form initialization data resolved successfully."
            );

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AccountTreeItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAccount(
            [FromBody] CreateAccountCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<AccountTreeItemDto>.Success(
                data: result,
                message: command.IsGroup ? "Group created successfully." : "Account created successfully."
            );

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
