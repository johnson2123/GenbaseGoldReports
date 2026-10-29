using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Domain.Entities.Sales;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Sales.Commands.CreateJewellerySale
{
    public class CreateJewellerySaleCommandHandler : IRequestHandler<CreateJewellerySaleCommand, CreateJewellerySaleResult>
    {
        private readonly IApplicationDbContext _context;

        public CreateJewellerySaleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateJewellerySaleResult> Handle(CreateJewellerySaleCommand request, CancellationToken cancellationToken)
        {
            const decimal ozFactor = 31.10350m;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var docMaster = await _context.Docmsts
                .FromSql($"SELECT * FROM docmst WITH (UPDLOCK, ROWLOCK) WHERE brncode = {request.Brncode} AND divcode = {request.Divcode} AND doctype = {request.Doctype}")
                .FirstOrDefaultAsync(cancellationToken);

            int nextDocNo = 1;
            if (docMaster != null)
            {
                if (int.TryParse(docMaster.Lastdocno, out int lastNum))
                {
                    nextDocNo = lastNum + 1;
                }
                docMaster.Lastdocno = nextDocNo.ToString();
            }
            else
            {
                docMaster = new Docmst
                {
                    Brncode = request.Brncode,
                    Divcode = request.Divcode,
                    Doctype = request.Doctype,
                    Lastdocno = nextDocNo.ToString(),
                    Mdoctype = "MTL",
                    Sdoctype = request.Doctype
                };
                _context.Docmsts.Add(docMaster);
            }

            var maxSlnoStr = await _context.Msals
                .FromSql($"SELECT TOP 1 * FROM msal ORDER BY CAST(slno AS BIGINT) DESC")
                .Select(x => x.Slno)
                .FirstOrDefaultAsync(cancellationToken);

            long nextSlnoLong = 1;
            if (!string.IsNullOrEmpty(maxSlnoStr) && long.TryParse(maxSlnoStr, out long parsedSlno))
            {
                nextSlnoLong = parsedSlno + 1;
            }

            string nextSlno = nextSlnoLong.ToString();

            decimal effectiveUsdRate = request.Usdrate > 0 ? request.Usdrate : 3.67400m;
            decimal gramRate = Math.Round((request.Gozrate / ozFactor) * effectiveUsdRate, 7);
            var currentTime = DateTime.UtcNow.ToString("HH:mm:ss");

            var msal = new Msal
            {
                Brncode = request.Brncode,
                Divcode = request.Divcode,
                Doctype = request.Doctype,
                Docno = nextDocNo,
                Slno = nextSlno,
                Mdoctype = "MTL",
                Sdoctype = request.Doctype,
                Docdate = request.Docdate.Date,
                Stimestamp = currentTime,
                Partycode = request.Partycode,
                Partyname = request.Partyname,
                Partytrn = request.Partytrn,
                Addr1 = request.Addr1,
                Sman = string.IsNullOrEmpty(request.Sman) ? "0" : request.Sman,
                Mtltype = "G",
                Ratetype = string.IsNullOrEmpty(request.Ratetype) ? "GMS" : request.Ratetype,
                Gozrate = request.Gozrate,
                Usdrate = effectiveUsdRate,
                Ozfactor = ozFactor,
                Gramrate = gramRate,
                Curr = request.Curr,
                Curr1 = "0",
                Currcode = request.Curr,
                Currrate = request.Currrate,
                Lccurr = request.Curr,
                Basecurr = request.Curr,
                Fixunfix = request.Fixunfix,
                Impexptype = string.IsNullOrEmpty(request.Impexptype) ? "B2BSAL" : request.Impexptype,
                Ishedging = request.Ishedging ? 1 : 0,
                Autohedgingdocno = request.HedgingDocno,
                Mpydocno = request.HedgingDocno,
                Refdocno = request.Refdocno,
                Narration = request.Narration,
                Hnarration = request.Narration,
                Loccode = "L1",
                Usercode = "0"
            };

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
                    Brncode = request.Brncode,
                    Divcode = request.Divcode,
                    Doctype = request.Doctype,
                    Docno = nextDocNo,
                    Slno = lineIndex,
                    Slno2 = lineIndex,
                    Nfactor = -1,
                    Mdoctype = "MTL",
                    Sdoctype = request.Doctype,
                    Docdate = request.Docdate.Date,
                    Stimestamp = currentTime,
                    Partycode = request.Partycode,
                    Impexptype = request.Impexptype,
                    Fixunfix = request.Fixunfix,
                    Sman = request.Sman,
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
                    Currcode = request.Curr,
                    Currrate = request.Currrate,
                    Loccode = "L1",
                    Usercode = "0",
                    Mtltype = "G",
                    Gord = 0
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

            msal.Partyamount = partyAmount;
            msal.Totnetamtbc = partyAmount;
            msal.Totnetamtfc = request.Currrate > 0 ? Math.Round(partyAmount / request.Currrate, 4) : partyAmount;
            msal.Totalpartyamt = partyAmount;
            msal.Goldamt = totalMetalAmt;
            msal.Makingvalue = totalMakingAmt;
            msal.Totalvat = totalTaxAmt;

            msal.Gsrate = 0.0m;
            msal.Stnamt = 0.0m;
            msal.Totamtfc = 0.0m;
            msal.Amtpaid = 0.0m;
            msal.Totdedamtfc = 0.0m;
            msal.Totaddamtfc = 0.0m;
            msal.Refcharges = 0.0m;

            msal.Premiumperc = request.Premiumperc;
            msal.Premiumamount = request.Premiumamount;
            msal.Discountperc = request.Discountperc;
            msal.Discountamount = request.Discountamount;

            _context.Msals.Add(msal);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CreateJewellerySaleResult(nextDocNo, $"{request.Doctype}-{nextDocNo}", partyAmount);
        }
    }
}
