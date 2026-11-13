using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Domain.Entities.Masters;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Commands.CreateMetalStock
{
    public class CreateMetalStockCommandHandler : IRequestHandler<CreateMetalStockCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public CreateMetalStockCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateMetalStockCommand request, CancellationToken cancellationToken)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                var cleanCode = request.Stkcode.Trim();

                var maxDocNo = await _context.Metalmasters
                    .MaxAsync(x => (int?)x.Docno, cancellationToken) ?? 10000;
                var nextDocNo = maxDocNo + 1;

                var recentSlNos = await _context.Metalmasters
                    .Where(x => x.Slno != null)
                    .OrderByDescending(x => x.Slno)
                    .Select(x => x.Slno)
                    .Take(20)
                    .ToListAsync(cancellationToken);

                var maxSlNo = recentSlNos
                    .Select(s => int.TryParse(s, out var n) ? n : 0)
                    .DefaultIfEmpty(100000)
                    .Max();
                var nextSlNo = (maxSlNo + 1).ToString();

                var entity = new Metalmaster
                {
                    Stkcode = cleanCode,
                    Stksubcode = $"{cleanCode}-D",
                    Stkname = request.Stkname?.Trim(),
                    Stkdetails = request.Stkdetails?.Trim(),
                    Doctype = string.IsNullOrWhiteSpace(request.Doctype) ? "MSTK" : request.Doctype,
                    Docno = nextDocNo,
                    Slno = nextSlNo,
                    Docdate = DateTime.UtcNow,

                    Mtltype = request.Mtltype,
                    Karat = request.Karat,
                    Spurity = request.Spurity,
                    Salpurity = request.Salpurity,
                    Apurity = 0.00m,

                    Uom = request.Uom,
                    Costunit = request.Uom,
                    Priceunit = request.Uom,
                    Suom = request.Uom,
                    Mainuom = request.Uom,
                    Subuom = request.Uom,
                    Tranuom = request.Uom,

                    Cat = request.Cat,
                    Subcat = request.Subcat,
                    Type = request.Type,
                    Brand = request.Brand,
                    Lcccode = request.Lcccode,
                    Mkglcccode = request.Mkglcccode,

                    Mkgrate = request.Mkgrate,
                    Isratebypcs = (short)(request.Isratebypcs ? 1 : 0),
                    Taxrate = request.Taxrate,
                    Mtltaxrate = request.Mtltaxrate,
                    Mkgtaxrate = request.Mkgtaxrate,

                    Ispcs = (short)(request.Ispcs ? 1 : 0),
                    Isweight = (short)(request.Isweight ? 1 : 0),
                    Isstone = (short)(request.Isstone ? 1 : 0),
                    Isscrap = (short)(request.Isscrap ? 1 : 0),
                    Allowminus = (short)(request.Allowminus ? 1 : 0),
                    Issingle = 1,
                    Autopl = 1,

                    Pcweight = request.Pcweight,
                    Cost = request.Cost,
                    Costfc = request.Cost,
                    Curr = request.Curr,
                    Currrate = request.Currrate,

                    Piclocation = request.Piclocation,
                    Tray = request.Tray,
                    Ctry = request.Ctry,
                    Loccode = "0",
                    Usercode = "0"
                };

                _context.Metalmasters.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return entity.Stkcode;
            });
        }
    }
}
