using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Queries.GetAccountFormInit
{
    public class GetAccountFormInitQueryHandler : IRequestHandler<GetAccountFormInitQuery, AccountFormInitDto>
    {
        private readonly IApplicationDbContext _context;

        public GetAccountFormInitQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountFormInitDto> Handle(GetAccountFormInitQuery request, CancellationToken cancellationToken)
        {
            string parentCode = "0";
            string parentName = "ROOT";
            int calculatedLevelId = 0;
            string? inheritedClass = null;

            if (!string.IsNullOrWhiteSpace(request.ParentCode) && request.ParentCode != "0")
            {
                var parent = await _context.Accmsts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Code == request.ParentCode, cancellationToken);

                if (parent != null)
                {
                    parentCode = parent.Code;
                    parentName = parent.Acname ?? string.Empty;
                    calculatedLevelId = (parent.Levelid ?? 0) + 1;
                    inheritedClass = parent.Class;
                }
            }

            string nextAccode = "0";
            if (!request.IsGroup)
            {
                var numericCodes = await _context.Accmsts
                    .AsNoTracking()
                    .Where(a => a.Isacc == 1 && a.Accode != null && a.Accode != "0")
                    .Select(a => a.Accode)
                    .ToListAsync(cancellationToken);

                var maxCode = numericCodes
                    .Select(c => int.TryParse(c, out var val) ? val : 0)
                    .DefaultIfEmpty(10000)
                    .Max();

                nextAccode = (maxCode + 1).ToString();
            }

            var accountTypes = await _context.Actypes
                .AsNoTracking()
                .Select(at => new AccountTypeLookupDto
                {
                    Code = at.Code,
                    Description = at.Descript ?? at.Code
                })
                .ToListAsync(cancellationToken);

            var classes = await _context.Classmsts
                .AsNoTracking()
                .Select(cm => new ClassLookupDto
                {
                    Code = cm.Code,
                    Description = cm.Descript ?? cm.Code
                })
                .ToListAsync(cancellationToken);

            var rawCurrencies = await _context.Currmsts
                .AsNoTracking()
                .Where(c => !string.IsNullOrEmpty(c.Currcode))
                .Select(c => new
                {
                    Currcode = c.Currcode.Trim(),
                    Currname = c.Currname != null ? c.Currname.Trim() : c.Currcode.Trim(),
                    Currrate = c.Currrate ?? 1.0m
                })
                .ToListAsync(cancellationToken);

            var currencies = rawCurrencies
                .GroupBy(c => c.Currcode, StringComparer.OrdinalIgnoreCase)
                .Select(g => new CurrencyLookupDto
                {
                    Currcode = g.Key,
                    Currname = g.First().Currname,
                    Currrate = g.First().Currrate
                })
                .OrderBy(c => c.Currcode)
                .ToList();

            var config = await _context.Configmsts
                .AsNoTracking()
                .Select(cfg => new
                {
                    BaseCurr = cfg.Basecurr,
                    TaxRate = cfg.Mkgtaxrate
                })
                .FirstOrDefaultAsync(cancellationToken);

            string baseCurr = !string.IsNullOrWhiteSpace(config?.BaseCurr) ? config.BaseCurr.Trim() : "AED";
            decimal defaultTaxRate = config?.TaxRate ?? 5.0m;

            var groups = await _context.Accmsts
                .AsNoTracking()
                .Where(a => a.Isacc != 1)
                .Select(a => new GroupLookupDto
                {
                    Code = a.Code,
                    Name = a.Acname ?? string.Empty,
                    LevelId = a.Levelid ?? 0,
                    Class = a.Class
                })
                .OrderBy(a => a.LevelId)
                .ThenBy(a => a.Name)
                .ToListAsync(cancellationToken);

            return new AccountFormInitDto
            {
                NextAccode = nextAccode,
                ParentCode = parentCode,
                ParentName = parentName,
                CalculatedLevelId = calculatedLevelId,
                InheritedClass = inheritedClass,
                BaseCurrency = baseCurr,
                DefaultTaxRate = defaultTaxRate,
                AccountTypes = accountTypes,
                Classes = classes,
                Currencies = currencies,
                Groups = groups
            };
        }
    }
}
