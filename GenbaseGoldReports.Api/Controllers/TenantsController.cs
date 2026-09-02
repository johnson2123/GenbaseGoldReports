using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Tenants.Commands.CreateTenant;
using GenbaseGoldReports.Application.Features.Tenants.Commands.ToggleTenantStatus;
using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using GenbaseGoldReports.Application.Features.Tenants.Queries.GetTenantsPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TenantsController : BaseApiController
    {
        private readonly ISender _mediator;

        public TenantsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            var response = ApiResponse<TenantDto>.Success(
                data: result,
                message: $"Tenant '{result.Name}' created successfully."
            );

            return CreatedAtAction(nameof(CreateTenant), new { id = result.Id }, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TenantDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTenantsPaged([FromQuery] GetTenantsPagedQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            var response = ApiResponse<PagedResult<TenantDto>>.Success(
                data: result,
                message: "Tenants retrieved successfully."
            );

            return Ok(response);
        }

        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleTenantStatus(Guid id, [FromBody] ToggleTenantStatusRequestBody body, CancellationToken cancellationToken)
        {
            var command = new ToggleTenantStatusCommand(id, body.IsActive);
            var result = await _mediator.Send(command, cancellationToken);

            var statusText = result.IsActive ? "activated" : "deactivated";
            var response = ApiResponse<TenantDto>.Success(
                data: result,
                message: $"Tenant '{result.Name}' has been {statusText} successfully."
            );

            return Ok(response);
        }
    }
}
