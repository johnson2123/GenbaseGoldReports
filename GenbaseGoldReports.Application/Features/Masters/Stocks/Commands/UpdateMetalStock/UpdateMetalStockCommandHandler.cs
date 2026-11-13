using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Commands.UpdateMetalStock
{
    public class UpdateMetalStockCommandHandler : IRequestHandler<UpdateMetalStockCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMetalStockCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMetalStockCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Metalmasters
                .FirstOrDefaultAsync(x => x.Stkcode == request.Stkcode, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException($"Metal stock item with code '{request.Stkcode}' was not found.");
            }

            entity.Stkname = request.Stkname?.Trim();
            entity.Stkdetails = request.Stkdetails?.Trim();
            entity.Doctype = string.IsNullOrWhiteSpace(request.Doctype) ? entity.Doctype : request.Doctype;

            entity.Mtltype = request.Mtltype;
            entity.Karat = request.Karat;
            entity.Spurity = request.Spurity;
            entity.Salpurity = request.Salpurity;

            entity.Uom = request.Uom;
            entity.Costunit = request.Uom;
            entity.Priceunit = request.Uom;
            entity.Suom = request.Uom;
            entity.Mainuom = request.Uom;
            entity.Subuom = request.Uom;
            entity.Tranuom = request.Uom;

            entity.Cat = request.Cat;
            entity.Subcat = request.Subcat;
            entity.Type = request.Type;
            entity.Brand = request.Brand;
            entity.Lcccode = request.Lcccode;
            entity.Mkglcccode = request.Mkglcccode;

            entity.Mkgrate = request.Mkgrate;
            entity.Isratebypcs = (short)(request.Isratebypcs ? 1 : 0);
            entity.Taxrate = request.Taxrate;
            entity.Mtltaxrate = request.Mtltaxrate;
            entity.Mkgtaxrate = request.Mkgtaxrate;

            entity.Ispcs = (short)(request.Ispcs ? 1 : 0);
            entity.Isweight = (short)(request.Isweight ? 1 : 0);
            entity.Isstone = (short)(request.Isstone ? 1 : 0);
            entity.Isscrap = (short)(request.Isscrap ? 1 : 0);
            entity.Allowminus = (short)(request.Allowminus ? 1 : 0);

            entity.Pcweight = request.Pcweight;
            entity.Cost = request.Cost;
            entity.Costfc = request.Cost;
            entity.Curr = request.Curr;
            entity.Currrate = request.Currrate;

            entity.Piclocation = request.Piclocation;
            entity.Tray = request.Tray;
            entity.Ctry = request.Ctry;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
