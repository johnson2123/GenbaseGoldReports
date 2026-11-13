using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockList
{
    public class GetMetalStockListQueryHandler : IRequestHandler<GetMetalStockListQuery, PagedResult<MetalStockListItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetMetalStockListQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<MetalStockListItemDto>> Handle(GetMetalStockListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Metalmasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x => x.Stkcode.Contains(term) || (x.Stkname != null && x.Stkname.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Mtltype))
            {
                query = query.Where(x => x.Mtltype == request.Mtltype);
            }

            if (!string.IsNullOrWhiteSpace(request.Karat))
            {
                query = query.Where(x => x.Karat == request.Karat);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Docdate)
                .ThenByDescending(x => x.Docno)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MetalStockListItemDto
                {
                    Stkcode = x.Stkcode,
                    Stksubcode = x.Stksubcode ?? (x.Stkcode + "-D"),
                    Stkname = x.Stkname,
                    Doctype = x.Doctype,
                    Docno = x.Docno,
                    Slno = x.Slno,
                    Mtltype = x.Mtltype,
                    Karat = x.Karat,
                    Spurity = x.Spurity,
                    Salpurity = x.Salpurity,
                    Uom = x.Uom,
                    Cat = x.Cat,
                    Subcat = x.Subcat,
                    Type = x.Type,
                    Brand = x.Brand,
                    Mkgrate = x.Mkgrate,
                    Isratebypcs = x.Isratebypcs == 1,
                    Taxrate = x.Taxrate,
                    Cost = x.Cost,
                    Docdate = x.Docdate
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<MetalStockListItemDto>(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
