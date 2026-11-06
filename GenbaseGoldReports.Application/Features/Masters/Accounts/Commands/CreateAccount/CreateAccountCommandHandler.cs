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

                try
                {
                    string parentCode = "0";
                    string parentName = "ROOT";
                    int calculatedLevelId = 0;
                    decimal parentKeycode = 100000000000m;

                    if (!string.IsNullOrWhiteSpace(request.ParentCode) && request.ParentCode != "0")
                    {
                        var parent = await _context.Accmsts
                            .FirstOrDefaultAsync(a => a.Code == request.ParentCode, cancellationToken)
                            ?? throw new InvalidOperationException($"Parent node with Code '{request.ParentCode}' was not found.");

                        parentCode = parent.Code;
                        parentName = parent.Acname ?? string.Empty;
                        calculatedLevelId = (parent.Levelid ?? 0) + 1;
                        parentKeycode = parent.Keycode ?? 100000000000m;
                    }

                    var allCodes = await _context.Accmsts
                        .Select(a => a.Code)
                        .ToListAsync(cancellationToken);

                    int maxNumericCode = allCodes
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
                            var numericAccodes = await _context.Accmsts
                                .Where(a => a.Isacc == 1 && a.Accode != null && a.Accode != "0")
                                .Select(a => a.Accode)
                                .ToListAsync(cancellationToken);

                            int maxAccode = numericAccodes
                                .Select(c => int.TryParse(c, out var val) ? val : 0)
                                .DefaultIfEmpty(10000)
                                .Max();

                            assignedAccode = (maxAccode + 1).ToString();
                        }
                    }

                    var siblingKeycodes = await _context.Accmsts
                        .Where(a => a.Grpcode == parentCode && a.Keycode != null)
                        .Select(a => a.Keycode!.Value)
                        .ToListAsync(cancellationToken);

                    decimal assignedKeycode;
                    if (siblingKeycodes.Count > 0)
                    {
                        assignedKeycode = siblingKeycodes.Max() + 1000.0000m;
                    }
                    else
                    {
                        assignedKeycode = parentKeycode + 10000.0000m;
                    }

                    var now = DateTime.UtcNow;
                    int assignedDocNo = int.TryParse(assignedCode, out var dVal) ? dVal : 0;
                    string assignedSlNo = assignedDocNo.ToString("D6");

                    // --- Hardened Accmst Record ---
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
                        Curr = string.IsNullOrWhiteSpace(request.Curr) ? "AED" : request.Curr.Trim().ToUpperInvariant(),
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

                        // Legacy Sequence & Audit Hardening
                        Docno = assignedDocNo,
                        Mrtype = "0",
                        Srno = 0,
                        Mstsrno = "0",
                        Doctype = "ACC",
                        Slno = assignedSlNo,
                        Parcode = "0",
                        Maingroup = 1,
                        Mdoctype = "ACC",
                        Sdoctype = "ACC",
                        Bnkcode = "0",
                        Bnkactype = "0",
                        Bnkacno = "0",
                        Mobileno = "0",
                        Isconfacc = 0,
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

                    // --- Synchronize Accdet ---
                    if (!request.IsGroup)
                    {
                        var baseCurr = newAcc.Curr ?? "AED";
                        var distinctCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { baseCurr };

                        if (request.AdditionalCurrencies != null)
                        {
                            foreach (var c in request.AdditionalCurrencies.Where(x => !string.IsNullOrWhiteSpace(x.Curr)))
                            {
                                distinctCurrencies.Add(c.Curr.Trim().ToUpperInvariant());
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
                        Acname = newAcc.Acname,
                        Accode = newAcc.Accode,
                        Grpname = newAcc.Grpname,
                        Isacc = newAcc.Isacc == 1,
                        Levelid = newAcc.Levelid ?? 0,
                        Keycode = newAcc.Keycode ?? 0m,
                        Actype = newAcc.Actype,
                        ActypeDescription = string.Empty,
                        Class = newAcc.Class,
                        Curr = newAcc.Curr,
                        Trnno = newAcc.Trnno,
                        Taxrate = newAcc.Taxrate ?? 0m
                    };
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}