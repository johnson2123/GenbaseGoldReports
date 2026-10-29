using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleById
{
    public class GetJewellerySaleByIdQueryHandler : IRequestHandler<GetJewellerySaleByIdQuery, JewellerySaleDetailsDto?>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetJewellerySaleByIdQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<JewellerySaleDetailsDto?> Handle(GetJewellerySaleByIdQuery request, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            // 1. Fetch the voucher header safely using TOP 1
            const string headerSql = @"
            SELECT TOP 1
                brncode AS Brncode,
                divcode AS Divcode,
                doctype AS Doctype,
                DOCNO AS Docno,
                docdate AS Docdate,
                stimestamp AS Stimestamp,
                partycode AS Partycode,
                partyname AS Partyname,
                partytrn AS Partytrn,
                addr1 AS Addr1,
                sman AS Sman,
                ratetype AS Ratetype,
                gozrate AS Gozrate,
                usdrate AS Usdrate,
                gramrate AS Gramrate,
                curr AS Curr,
                currrate AS Currrate,
                fixunfix AS Fixunfix,
                impexptype AS Impexptype,
                totpcs AS Totpcs,
                totgrqty AS Totgrqty,
                totpuqty AS Totpuqty,
                totmkgamt AS Totmkgamt,
                totalmtlamt AS Totalmtlamt,
                tottaxamt AS Tottaxamt,
                totgrossamt AS Totgrossamt,
                partyamount AS Partyamount,
                premiumperc AS Premiumperc,
                premiumamount AS Premiumamount,
                discountperc AS Discountperc,
                discountamount AS Discountamount,
                ishedging AS Ishedging,
                autohedgingdocno AS Autohedgingdocno,
                refdocno AS Refdocno,
                narration AS Narration
            FROM msal
            WHERE DOCNO = @Docno
              AND (@Doctype IS NULL OR doctype = @Doctype)
            ORDER BY docdate DESC;";

            var header = await connection.QueryFirstOrDefaultAsync<JewellerySaleDetailsDto>(
                headerSql,
                new { request.Docno, request.Doctype });

            if (header == null) return null;

            // 2. Fetch the corresponding lines matching this exact voucher
            const string linesSql = @"
            SELECT 
                slno AS Slno,
                stkcode AS Stkcode,
                stksubcode AS Stksubcode,
                pieces AS Pieces,
                grqty AS Grqty,
                apurity AS Apurity,
                puqty AS Puqty,
                mkgrate AS Mkgrate,
                mkgamt AS Mkgamt,
                mtlrate AS Mtlrate,
                mtlamt AS Mtlamt,
                mkgtaxamt AS Mkgtaxamt,
                mtltaxamt AS Mtltaxamt,
                taxamt AS Taxamt,
                totalamt AS Totalamt
            FROM msaldet
            WHERE brncode = @Brncode 
              AND divcode = @Divcode 
              AND doctype = @Doctype 
              AND DOCNO = @Docno
            ORDER BY slno ASC;";

            var lines = (await connection.QueryAsync<JewellerySaleDetailLineDto>(linesSql, new
            {
                header.Brncode,
                header.Divcode,
                header.Doctype,
                header.Docno
            })).AsList();

            return header with { Lines = lines };
        }
    }
}
