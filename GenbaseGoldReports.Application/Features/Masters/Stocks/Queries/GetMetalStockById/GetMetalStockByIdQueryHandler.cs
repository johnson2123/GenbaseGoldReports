using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockById
{
    public class GetMetalStockByIdQueryHandler : IRequestHandler<GetMetalStockByIdQuery, MetalStockDetailDto>
    {
        private readonly IApplicationDbContext _context;

        public GetMetalStockByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MetalStockDetailDto> Handle(GetMetalStockByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _context.Metalmasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Stkcode == request.Stkcode, cancellationToken);

            if (item == null)
            {
                throw new NotFoundException($"Metal stock item with code '{request.Stkcode}' was not found.");
            }

            return new MetalStockDetailDto
            {
                Stkcode = item.Stkcode,
                Stksubcode = item.Stksubcode ?? (item.Stkcode + "-D"),
                Stkname = item.Stkname,
                Stkdetails = item.Stkdetails,
                Doctype = item.Doctype,
                Docno = item.Docno,
                Slno = item.Slno,
                Docdate = item.Docdate,

                Mtltype = item.Mtltype,
                Karat = item.Karat,
                Spurity = item.Spurity,
                Salpurity = item.Salpurity,
                Apurity = item.Apurity,

                Uom = item.Uom,
                Cat = item.Cat,
                Subcat = item.Subcat,
                Type = item.Type,
                Brand = item.Brand,
                Lcccode = item.Lcccode,
                Mkglcccode = item.Mkglcccode,

                Mkgrate = item.Mkgrate,
                Isratebypcs = item.Isratebypcs == 1,
                Taxrate = item.Taxrate,
                Mtltaxrate = item.Mtltaxrate,
                Mkgtaxrate = item.Mkgtaxrate,

                Ispcs = item.Ispcs == 1,
                Isweight = item.Isweight == 1,
                Isstone = item.Isstone == 1,
                Isscrap = item.Isscrap == 1,
                Allowminus = item.Allowminus == 1,
                Pcweight = item.Pcweight,

                Cost = item.Cost,
                Costfc = item.Costfc,
                Curr = item.Curr,
                Currrate = item.Currrate,

                Piclocation = item.Piclocation,
                Tray = item.Tray,
                Ctry = item.Ctry
            };
        }
    }
}
