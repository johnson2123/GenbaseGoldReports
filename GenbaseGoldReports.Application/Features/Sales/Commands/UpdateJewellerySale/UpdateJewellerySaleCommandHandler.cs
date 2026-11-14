using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Domain.Entities.Sales;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Sales.Commands.UpdateJewellerySale
{
    public class UpdateJewellerySaleCommandHandler : IRequestHandler<UpdateJewellerySaleCommand, UpdateJewellerySaleResult>
    {
        private readonly IApplicationDbContext _context;

        public UpdateJewellerySaleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateJewellerySaleResult> Handle(UpdateJewellerySaleCommand request, CancellationToken cancellationToken)
        {
            const decimal ozFactor = 31.10350m;
            const decimal ttbGramsFactor = 116.6380m;

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                var msal = await _context.Msals
                    .Include(x => x.Details)
                    .FirstOrDefaultAsync(x =>
                        x.Brncode == request.Brncode &&
                        x.Divcode == request.Divcode &&
                        x.Doctype == request.Doctype &&
                        x.Docno == request.Docno,
                        cancellationToken);

                if (msal == null)
                {
                    throw new NotFoundException(nameof(Msal), $"{request.Doctype}-{request.Docno}");
                }

                decimal effectiveUsdRate = request.Usdrate > 0 ? request.Usdrate : 3.67400m;
                string rateType = (request.Ratetype ?? "GMS").ToUpper();
                decimal gramRate = rateType switch
                {
                    "GMS" => Math.Round(request.Gozrate, 7),
                    "TTB" => Math.Round(request.Gozrate / ttbGramsFactor, 7),
                    _ => Math.Round((request.Gozrate / ozFactor) * effectiveUsdRate, 7)
                };

                msal.Docdate = request.Docdate.Date;
                msal.Partycode = request.Partycode;
                msal.Partyname = request.Partyname;
                msal.Partytrn = request.Partytrn;
                msal.Addr1 = request.Addr1;
                msal.Sman = string.IsNullOrEmpty(request.Sman) ? "0" : request.Sman;

                msal.Ratetype = rateType;
                msal.Gozrate = request.Gozrate;
                msal.Usdrate = effectiveUsdRate;
                msal.Ozfactor = ozFactor;
                msal.Gramrate = gramRate;

                msal.Curr = request.Curr;
                msal.Currcode = request.Curr;
                msal.Currrate = request.Currrate;
                msal.Lccurr = request.Curr;
                msal.Basecurr = request.Curr;

                msal.Fixunfix = request.Fixunfix;
                msal.Impexptype = string.IsNullOrEmpty(request.Impexptype) ? "B2BSAL" : request.Impexptype;
                msal.Ishedging = request.Ishedging ? 1 : 0;
                msal.Autohedgingdocno = request.HedgingDocno;
                msal.Mpydocno = request.HedgingDocno;
                msal.Refdocno = request.Refdocno;
                msal.Narration = request.Narration;
                msal.Hnarration = request.Narration;

                // Replace detail lines cleanly to handle additions, removals, and reordering
                msal.Details.Clear();

                int lineIndex = 1;
                decimal totalGrossWt = 0;
                decimal totalPureWt = 0;
                decimal totalPcs = 0;
                decimal totalMakingAmt = 0;
                decimal totalMetalAmt = 0;
                decimal totalMakingTax = 0;
                decimal totalMetalTax = 0;

                foreach (var line in request.Lines)
                {
                    decimal effectivePurity = line.Apurity > 0 ? line.Apurity : 0.920m;
                    decimal pureWt = Math.Round(line.Grqty * effectivePurity, 3);
                    decimal mkgAmt = Math.Round(line.Grqty * line.Mkgrate, 2);
                    decimal mtlAmt = Math.Round(pureWt * gramRate, 2);
                    decimal mkgTax = Math.Round(mkgAmt * (line.Mkgtaxrate / 100m), 2);
                    decimal mtlTax = Math.Round(mtlAmt * (line.Mtltaxrate / 100m), 2);
                    decimal lineTax = mkgTax + mtlTax;
                    decimal lineTotal = mtlAmt + mkgAmt + lineTax;

                    totalGrossWt += line.Grqty;
                    totalPureWt += pureWt;
                    totalPcs += line.Pieces;
                    totalMakingAmt += mkgAmt;
                    totalMetalAmt += mtlAmt;
                    totalMakingTax += mkgTax;
                    totalMetalTax += mtlTax;

                    msal.Details.Add(new Msaldet
                    {
                        Brncode = msal.Brncode,
                        Divcode = msal.Divcode,
                        Doctype = msal.Doctype,
                        Docno = msal.Docno,
                        Slno = lineIndex,
                        Slno2 = lineIndex,
                        Nfactor = -1,
                        Mdoctype = msal.Mdoctype,
                        Sdoctype = msal.Sdoctype,
                        Docdate = msal.Docdate,
                        Stimestamp = msal.Stimestamp,
                        Partycode = msal.Partycode,
                        Impexptype = msal.Impexptype,
                        Fixunfix = msal.Fixunfix,
                        Sman = msal.Sman,
                        Stkcode = line.Stkcode,
                        Stksubcode = string.IsNullOrEmpty(line.Stksubcode) ? line.Stkcode : line.Stksubcode,
                        Pieces = line.Pieces,
                        Pcs = line.Pieces,
                        Grqty = line.Grqty,
                        Apurity = effectivePurity,
                        Spurity = effectivePurity,
                        Puqty = pureWt,
                        Mkgrate = line.Mkgrate,
                        Mkgamt = mkgAmt,
                        Mtlrate = gramRate,
                        Mtlamt = mtlAmt,
                        Mkgtaxamt = mkgTax,
                        Mtltaxamt = mtlTax,
                        Taxamt = lineTax,
                        Totalamt = lineTotal,
                        Currcode = msal.Curr,
                        Currrate = msal.Currrate,
                        Loccode = msal.Loccode ?? "L1",
                        Usercode = msal.Usercode ?? "0",
                        Mtltype = msal.Mtltype ?? "G",
                        Gord = 0,
                        Rate = msal.Gozrate,
                        Ratetype = rateType,
                        Gozrate = msal.Gozrate,
                        Usdrate = effectiveUsdRate
                    });

                    lineIndex++;
                }

                decimal totalTaxAmt = totalMakingTax + totalMetalTax;
                decimal grandTotal = totalMetalAmt + totalMakingAmt + totalTaxAmt + request.Premiumamount - request.Discountamount;
                decimal partyAmount = request.Fixunfix == "FIX"
                    ? grandTotal
                    : totalMakingAmt + totalMakingTax;

                msal.Totpcs = totalPcs;
                msal.Pieces = totalPcs;
                msal.Totgrqty = totalGrossWt;
                msal.Totpuqty = totalPureWt;
                msal.Totmkgamt = totalMakingAmt;
                msal.Totalmakingamount = totalMakingAmt;
                msal.Totalmtlamt = totalMetalAmt;
                msal.Totalmetalamount = totalMetalAmt;
                msal.Tottaxamt = totalTaxAmt;
                msal.Totalvatamount = totalTaxAmt;
                msal.Totgrossamt = grandTotal;
                msal.Gtotalamt = grandTotal;
                msal.Totalamount = grandTotal;

                msal.Mkgtaxamt = totalMakingTax;
                msal.Mkgtaxableamt = totalMakingAmt;
                msal.Mtltaxamt = totalMetalTax;
                msal.Mtltaxableamt = totalMetalAmt;
                msal.Totaltaxableamt = totalMakingAmt + totalMetalAmt;

                msal.Partyamount = partyAmount;
                msal.Totnetamtbc = partyAmount;
                msal.Totnetamtfc = request.Currrate > 0 ? Math.Round(partyAmount / request.Currrate, 4) : partyAmount;
                msal.Totalpartyamt = partyAmount;
                msal.Goldamt = totalMetalAmt;
                msal.Makingvalue = totalMakingAmt;
                msal.Totalvat = totalTaxAmt;

                msal.Premiumperc = request.Premiumperc;
                msal.Premiumamount = request.Premiumamount;
                msal.Discountperc = request.Discountperc;
                msal.Discountamount = request.Discountamount;

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new UpdateJewellerySaleResult(msal.Docno, $"{msal.Doctype}-{msal.Docno}", partyAmount);
            });
        }
    }
}
