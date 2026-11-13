using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using GenbaseGoldReports.Domain.Entities.Masters;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountTreeItemDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateAccountCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountTreeItemDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Acname))
            {
                throw new ArgumentException("Account / Group Name is required.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                string parentCode = "0";
                string parentName = "ROOT";
                int calculatedLevelId = 0;
                decimal parentKeycode = 100000000000m;
                int? parentMainGroup = 1;

                if (!string.IsNullOrWhiteSpace(request.ParentCode) && request.ParentCode != "0")
                {
                    var parent = await _context.Accmsts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Code == request.ParentCode, cancellationToken)
                        ?? throw new InvalidOperationException($"Parent node with Code '{request.ParentCode}' was not found.");

                    parentCode = parent.Code;
                    parentName = parent.Acname ?? string.Empty;
                    calculatedLevelId = (parent.Levelid ?? 0) + 1;
                    parentKeycode = parent.Keycode ?? 100000000000m;
                    parentMainGroup = parent.Maingroup ?? 1;
                }

                // Retrieve top numeric codes without pulling full table to memory
                var recentCodes = await _context.Accmsts
                    .AsNoTracking()
                    .Where(a => a.Code.Length <= 8)
                    .OrderByDescending(a => a.Code.Length)
                    .ThenByDescending(a => a.Code)
                    .Select(a => a.Code)
                    .Take(100)
                    .ToListAsync(cancellationToken);

                int maxNumericCode = recentCodes
                    .Select(c => int.TryParse(c, out var val) ? val : 0)
                    .DefaultIfEmpty(10000)
                    .Max();

                string assignedCode = (maxNumericCode + 1).ToString();

                string assignedAccode = "0";
                if (!request.IsGroup)
                {
                    if (!string.IsNullOrWhiteSpace(request.Accode) && request.Accode != "0")
                    {
                        var exists = await _context.Accmsts.AnyAsync(a => a.Accode == request.Accode.Trim(), cancellationToken);
                        if (exists)
                        {
                            throw new InvalidOperationException($"Account Code '{request.Accode}' already exists in ledger.");
                        }
                        assignedAccode = request.Accode.Trim();
                    }
                    else
                    {
                        var recentAccodes = await _context.Accmsts
                            .AsNoTracking()
                            .Where(a => a.Isacc == 1 && a.Accode != null && a.Accode != "0" && a.Accode.Length <= 8)
                            .OrderByDescending(a => a.Accode!.Length)
                            .ThenByDescending(a => a.Accode)
                            .Select(a => a.Accode!)
                            .Take(100)
                            .ToListAsync(cancellationToken);

                        int maxAccode = recentAccodes
                            .Select(c => int.TryParse(c, out var val) ? val : 0)
                            .DefaultIfEmpty(10000)
                            .Max();

                        assignedAccode = (maxAccode + 1).ToString();
                    }
                }

                var siblingKeycodes = await _context.Accmsts
                    .AsNoTracking()
                    .Where(a => a.Grpcode == parentCode && a.Keycode != null)
                    .Select(a => a.Keycode!.Value)
                    .ToListAsync(cancellationToken);

                decimal assignedKeycode = siblingKeycodes.Count > 0
                    ? siblingKeycodes.Max() + 1000.0000m
                    : parentKeycode + 10000.0000m;

                var tenantBaseCurr = await _context.Configmsts
                    .AsNoTracking()
                    .Select(c => c.Basecurr)
                    .FirstOrDefaultAsync(cancellationToken);

                string resolvedCurrency = !string.IsNullOrWhiteSpace(request.Curr)
                    ? request.Curr.Trim().ToUpperInvariant()
                    : (!string.IsNullOrWhiteSpace(tenantBaseCurr) ? tenantBaseCurr.Trim().ToUpperInvariant() : "AED");

                var now = DateTime.UtcNow;
                int assignedDocNo = int.TryParse(assignedCode, out var dVal) ? dVal : 0;
                string assignedSlNo = assignedDocNo.ToString("D6");

                var newAcc = new Accmst
                {
                    Code = assignedCode,
                    Grpcode = parentCode,
                    Acname = request.Acname.Trim().ToUpperInvariant(),
                    Accode = assignedAccode,
                    Grpname = parentName,
                    Isacc = request.IsGroup ? (short)0 : (short)1,
                    Levelid = calculatedLevelId,
                    Keycode = assignedKeycode,
                    Actype = request.Actype?.Trim() ?? "G",
                    Class = request.Class?.Trim() ?? "010",
                    Brncode = "HO",
                    Curr = resolvedCurrency,
                    Currrate = request.Currrate > 0 ? request.Currrate : 1.0m,
                    Trnno = string.IsNullOrWhiteSpace(request.Trnno) ? "0" : request.Trnno.Trim(),
                    Taxrate = request.Taxrate,
                    Isvatgroup = request.Isvatgroup ? (short)1 : (short)0,
                    Addr1 = string.IsNullOrWhiteSpace(request.Addr1) ? "0" : request.Addr1.Trim(),
                    Addr2 = string.IsNullOrWhiteSpace(request.Addr2) ? "0" : request.Addr2.Trim(),
                    Addr3 = string.IsNullOrWhiteSpace(request.Addr3) ? "0" : request.Addr3.Trim(),
                    City = string.IsNullOrWhiteSpace(request.City) ? "0" : request.City.Trim(),
                    Country = string.IsNullOrWhiteSpace(request.Country) ? "0" : request.Country.Trim(),
                    Area = string.IsNullOrWhiteSpace(request.Area) ? "DXB" : request.Area.Trim(),
                    Phone = string.IsNullOrWhiteSpace(request.Phone) ? "0" : request.Phone.Trim(),
                    Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? "0" : request.Mobile.Trim(),
                    Contactperson = string.IsNullOrWhiteSpace(request.Contactperson) ? "0" : request.Contactperson.Trim(),
                    Email = string.IsNullOrWhiteSpace(request.Email) ? "0" : request.Email.Trim(),
                    Website = string.IsNullOrWhiteSpace(request.Website) ? "0" : request.Website.Trim(),
                    Licencenumber = string.IsNullOrWhiteSpace(request.Licencenumber) ? "0" : request.Licencenumber.Trim(),
                    Licencedate = string.IsNullOrWhiteSpace(request.Licencedate) ? "0" : request.Licencedate.Trim(),
                    Selected = 1,
                    Docdate = now,
                    Docno = assignedDocNo,
                    Mrtype = "0",
                    Srno = 0,
                    Mstsrno = "0",
                    Doctype = "ACC",
                    Slno = assignedSlNo,
                    Parcode = "0",
                    Maingroup = parentMainGroup,
                    Mdoctype = "ACC",
                    Sdoctype = "ACC",
                    Bnkcode = "0",
                    Bnkactype = "0",
                    Bnkacno = "0",
                    Mobileno = "0",
                    Isconfacc = request.Isconfacc ? (short)1 : (short)0,
                    Purusdrate = request.Purusdrate,
                    Salusdrate = request.Salusdrate,
                    Cvrno = "0",
                    Isspecialacc = "0",
                    Custcat = "0",
                    Custtype = "0",
                    Isalloc = 0,
                    Purgozrate = 0.00m,
                    Salgozrate = 0.00m,
                    Purconvrate = 0.00m,
                    Salconvrate = 0.00m,
                    Purtxntype = "0",
                    Saltxntype = "0",
                    Marginperc = request.Marginperc,
                    Marginconsumeperc = request.Marginconsumeperc,
                    Custarea = "0",
                    Sharepercentage = 0.00m,
                    Loccode = "0",
                    Usercode = "0"
                };

                await _context.Accmsts.AddAsync(newAcc, cancellationToken);

                if (!request.IsGroup)
                {
                    var distinctCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { resolvedCurrency };

                    if (request.AdditionalCurrencies != null)
                    {
                        foreach (var c in request.AdditionalCurrencies.Where(x => !string.IsNullOrWhiteSpace(x.Currcode)))
                        {
                            distinctCurrencies.Add(c.Currcode.Trim().ToUpperInvariant());
                        }
                    }

                    int subIndex = 1;
                    foreach (var currCode in distinctCurrencies)
                    {
                        var accdetRow = new Accdet
                        {
                            Accode = assignedAccode,
                            Curr = currCode,
                            Doctype = "0",
                            Docno = "0",
                            Docdoctype = "0",
                            Mdoctype = "0",
                            Sdoctype = "0",
                            Currname = currCode,
                            Docdate = now,
                            Levelcode = "0",
                            Uniqueid = decimal.Parse($"4.9{subIndex}"),
                            Loccode = "0",
                            Usercode = "0"
                        };
                        await _context.Accdets.AddAsync(accdetRow, cancellationToken);
                        subIndex++;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new AccountTreeItemDto
                {
                    Code = newAcc.Code,
                    Grpcode = newAcc.Grpcode ?? string.Empty,
                    Acname = newAcc.Acname ?? string.Empty,
                    Accode = newAcc.Accode ?? string.Empty,
                    Grpname = newAcc.Grpname ?? string.Empty,
                    Isacc = newAcc.Isacc == 1,
                    Levelid = newAcc.Levelid ?? 0,
                    Keycode = newAcc.Keycode ?? 0m,
                    Actype = newAcc.Actype ?? string.Empty,
                    ActypeDescription = string.Empty,
                    Class = newAcc.Class ?? string.Empty,
                    Curr = newAcc.Curr ?? "AED",
                    Trnno = newAcc.Trnno ?? string.Empty,
                    Taxrate = newAcc.Taxrate ?? 0m
                };
            });
        }
    }
}