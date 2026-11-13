using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockFormInit
{
    public class GetMetalStockFormInitQueryHandler : IRequestHandler<GetMetalStockFormInitQuery, MetalStockFormInitDto>
    {
        private readonly IApplicationDbContext _context;

        public GetMetalStockFormInitQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MetalStockFormInitDto> Handle(GetMetalStockFormInitQuery request, CancellationToken cancellationToken)
        {
            var metalTypes = await _context.Mtltypemsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            var karats = await _context.Karatmsts
                .AsNoTracking()
                .Select(x => new KaratLookupDto
                {
                    Code = x.Code,
                    Description = x.Description ?? x.Code,
                    StandardPurity = x.Spurity,
                    SalesPurity = x.Purity
                })
                .ToListAsync(cancellationToken);

            var uoms = await _context.Uommsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            var categories = await _context.Catmsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            var subCategories = await _context.Subcatmsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            var types = await _context.Typemsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            var brands = await _context.Brandmsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            var lccs = await _context.Lccmsts
                .AsNoTracking()
                .Select(x => new LookupItemDto { Code = x.Code, Description = x.Description ?? x.Code })
                .ToListAsync(cancellationToken);

            // Sequence suggestions
            var maxDocNo = await _context.Metalmasters
                .MaxAsync(x => (int?)x.Docno, cancellationToken) ?? 10000;

            // Legacy slno is varchar(6)
            var lastSlNos = await _context.Metalmasters
                .Where(x => x.Slno != null)
                .OrderByDescending(x => x.Slno)
                .Select(x => x.Slno)
                .Take(10)
                .ToListAsync(cancellationToken);

            int maxSlNo = lastSlNos
                .Select(s => int.TryParse(s, out var n) ? n : 0)
                .DefaultIfEmpty(100000)
                .Max();

            return new MetalStockFormInitDto
            {
                MetalTypes = metalTypes,
                Karats = karats,
                Uoms = uoms,
                Categories = categories,
                SubCategories = subCategories,
                Types = types,
                Brands = brands,
                Lccs = lccs,
                MakingLccs = lccs,
                SuggestedDocNo = maxDocNo + 1,
                SuggestedSlNo = (maxSlNo + 1).ToString()
            };
        }
    }
}
