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
                nextDocNo = docMaster.Lastdocno + 1;
                docMaster.Lastdocno = nextDocNo;
            }
            else
            {
                docMaster = new Docmst
                {
                    Brncode = request.Brncode,
                    Divcode = request.Divcode,
                    Doctype = request.Doctype,
                    Lastdocno = nextDocNo,
                    Mdoctype = "MTL",
                    Sdoctype = request.Doctype
                };
                _context.Docmsts.Add(docMaster);
            }

            decimal gramRate = Math.Round((request.Gozrate * request.Usdrate) / ozFactor, 7);
            var currentTime = DateTime.UtcNow.ToString("HH:mm:ss");

            var msal = new Msal
            {
                Brncode = request.Brncode,
                Divcode = request.Divcode,
                Doctype = request.Doctype,
                Docno = nextDocNo,
                Mdoctype = "MTL",
                Sdoctype = request.Doctype,
                Docdate = request.Docdate,
                Stimestamp = currentTime,
                Partycode = request.Partycode,
                Partyname = request.Partyname,
                Partytrn = request.Partytrn,
                Addr1 = request.Addr1,
                Sman = request.Sman,
                Ratetype = request.Ratetype,
                Gozrate = request.Gozrate,
                Usdrate = request.Usdrate,
                Ozfactor = ozFactor,
                Gramrate = gramRate,
                Curr = request.Curr,
                Currcode = request.Curr,
                Currrate = request.Currrate,
                Lccurr = request.Curr,
                Fixunfix = request.Fixunfix,
                Impexptype = request.Impexptype,
                Ishedging = request.Ishedging ? 1 : 0,
                Autohedgingdocno = request.HedgingDocno,
                Mpydocno = request.HedgingDocno,
                Refdocno = request.Refdocno,
                Narration = request.Narration,
                Hnarration = request.Narration
            };

            int lineIndex = 1;
            decimal totalGrossWt = 0;
            decimal totalPureWt = 0;
            int totalPcs = 0;
            decimal totalMakingAmt = 0;
            decimal totalMetalAmt = 0;
            decimal totalTaxAmt = 0;

            foreach (var line in request.Lines)
            {
                decimal pureWt = Math.Round(line.Grqty * line.Apurity, 3);
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
                totalTaxAmt += lineTax;

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
                    Docdate = request.Docdate,
                    Stimestamp = currentTime,
                    Partycode = request.Partycode,
                    Impexptype = request.Impexptype,
                    Fixunfix = request.Fixunfix,
                    Sman = request.Sman,
                    Stkcode = line.Stkcode,
                    Stksubcode = line.Stksubcode,
                    Pieces = line.Pieces,
                    Pcs = line.Pieces,
                    Grqty = line.Grqty,
                    Apurity = line.Apurity,
                    Spurity = line.Apurity,
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
                    Currrate = request.Currrate
                });

                lineIndex++;
            }

            decimal grandTotal = totalMetalAmt + totalMakingAmt + totalTaxAmt + request.Premiumamount - request.Discountamount;
            decimal partyAmount = request.Fixunfix == "FIX"
                ? grandTotal
                : totalMakingAmt + totalTaxAmt;

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
