using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Navigation.Dtos;
using GenbaseGoldReports.Application.Features.Navigation.Queries.GetMyMenu;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GenbaseGoldReports.Api.Controllers
{
    [Authorize]
    public class NavigationController : BaseApiController
    {
        private readonly ISender _mediator;

        public NavigationController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my-menu")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MenuItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyMenu(CancellationToken cancellationToken)
        {
            var userRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .Distinct()
                .ToList();

            var query = new GetMyMenuQuery(userRoles);
            var menuItems = await _mediator.Send(query, cancellationToken);

            var response = ApiResponse<IReadOnlyList<MenuItemDto>>.Success(
                data: menuItems,
                message: "User navigation menu retrieved successfully."
            );

            return Ok(response);
        }
    }
}
