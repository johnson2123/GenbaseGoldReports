using GenbaseGoldReports.Application.Features.Navigation.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Navigation.Queries.GetMyMenu
{
    public record GetMyMenuQuery(IEnumerable<string> UserRoles) : IRequest<IReadOnlyList<MenuItemDto>>;

}
