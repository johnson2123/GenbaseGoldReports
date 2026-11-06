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

            var currencies = new List<string> { "AED", "USD", "EUR", "GBP", "SAR" };

            return new AccountFormInitDto
            {
                NextAccode = nextAccode,
                ParentCode = parentCode,
                ParentName = parentName,
                CalculatedLevelId = calculatedLevelId,
                AccountTypes = accountTypes,
                Currencies = currencies
            };
        }
    }
}
