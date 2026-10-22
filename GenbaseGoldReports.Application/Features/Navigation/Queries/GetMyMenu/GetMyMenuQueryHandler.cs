using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Navigation.Dtos;
using GenbaseGoldReports.Domain.Entities.Navigation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Navigation.Queries.GetMyMenu
{
    public class GetMyMenuQueryHandler : IRequestHandler<GetMyMenuQuery, IReadOnlyList<MenuItemDto>>
    {
        private readonly IMasterDbContext _context;

        public GetMyMenuQueryHandler(IMasterDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<MenuItemDto>> Handle(GetMyMenuQuery request, CancellationToken cancellationToken)
        {
            if (request.UserRoles == null || !request.UserRoles.Any())
            {
                return Array.Empty<MenuItemDto>();
            }

            var roles = request.UserRoles.Select(r => r.Trim()).Distinct().ToList();

            var menuItems = await _context.RoleMenuItems
                .AsNoTracking()
                .Where(rm => roles.Contains(rm.RoleName) && rm.MenuItem.IsActive)
                .Select(rm => rm.MenuItem)
                .Distinct()
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(cancellationToken);

            var topLevelItems = menuItems
                .Where(m => m.ParentId == null)
                .Select(m => MapToDto(m, menuItems))
                .ToList();

            return topLevelItems;
        }

        private static MenuItemDto MapToDto(MenuItem item, List<MenuItem> allMenuItems)
        {
            return new MenuItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Path = item.Path,
                Icon = item.Icon,
                DisplayOrder = item.DisplayOrder,
                Items = allMenuItems
                    .Where(m => m.ParentId == item.Id)
                    .OrderBy(m => m.DisplayOrder)
                    .Select(child => MapToDto(child, allMenuItems))
                    .ToList()
            };
        }
    }
}
