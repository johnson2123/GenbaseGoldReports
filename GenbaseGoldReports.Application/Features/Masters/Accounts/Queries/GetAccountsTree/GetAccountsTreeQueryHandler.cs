using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Queries.GetAccountsTree
{
    public class GetAccountsTreeQueryHandler : IRequestHandler<GetAccountsTreeQuery, List<AccountTreeItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAccountsTreeQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountTreeItemDto>> Handle(GetAccountsTreeQuery request, CancellationToken cancellationToken)
        {
            var query = from acc in _context.Accmsts.AsNoTracking()
                        join act in _context.Actypes.AsNoTracking()
                            on acc.Actype equals act.Code into actGroup
                        from act in actGroup.DefaultIfEmpty()
                        select new
                        {
                            acc.Code,
                            acc.Grpcode,
                            acc.Acname,
                            acc.Accode,
                            acc.Grpname,
                            acc.Isacc,
                            acc.Levelid,
                            acc.Keycode,
                            acc.Actype,
                            ActypeDescription = act != null ? act.Descript : string.Empty,
                            acc.Class,
                            acc.Curr,
                            acc.Trnno,
                            acc.Taxrate
                        };

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a => a.Acname != null && a.Acname.ToLower().Contains(search)
                                      || a.Accode != null && a.Accode.ToLower().Contains(search));
            }

            var rawItems = await query
                .OrderBy(a => a.Keycode)
                .ToListAsync(cancellationToken);

            return rawItems.Select(acc => new AccountTreeItemDto
            {
                Code = acc.Code,
                Grpcode = acc.Grpcode ?? string.Empty,
                Acname = acc.Acname ?? string.Empty,
                Accode = acc.Accode ?? string.Empty,
                Grpname = acc.Grpname ?? string.Empty,
                Isacc = acc.Isacc == 1,
                Levelid = acc.Levelid ?? 0,
                Keycode = acc.Keycode ?? 0m,
                Actype = acc.Actype ?? string.Empty,
                ActypeDescription = acc.ActypeDescription ?? string.Empty,
                Class = acc.Class ?? string.Empty,
                Curr = acc.Curr ?? "AED",
                Trnno = acc.Trnno ?? string.Empty,
                Taxrate = acc.Taxrate ?? 0m
            }).ToList();
        }
    }
}
