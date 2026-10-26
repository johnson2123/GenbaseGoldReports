using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleInit
{
    public class GetJewellerySaleInitQueryHandler : IRequestHandler<GetJewellerySaleInitQuery, JewellerySaleInitDto>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetJewellerySaleInitQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<JewellerySaleInitDto> Handle(GetJewellerySaleInitQuery request, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
            -- Branches
            SELECT brncode AS Code, brnname AS Name FROM brnmst;

            -- Divisions
            SELECT divcode AS Code, divname AS Name FROM divmst;

            -- VAT Types
            SELECT code AS Code, description AS Name FROM impexpmst;

            -- Salesmen
            SELECT code AS Code, description AS Name FROM smanmst;

            -- Rate Types
            SELECT code AS Code, description AS Description, ISNULL(convfact, 31.10348) AS ConvFact, 
                   ISNULL(currrate, 1.0) AS CurrRate, ISNULL(gozrate, 0.0) AS GozRate 
            FROM ratetypemst;

            -- Stock Items
            SELECT stkcode AS Stkcode, stkname AS Stkname, ISNULL(apurity, 0.920) AS Apurity, 
                   ISNULL(spurity, 0.920) AS Spurity, ISNULL(mkgtaxrate, 0.0) AS MkgTaxRate, 
                   ISNULL(mtltaxrate, 0.0) AS MtlTaxRate 
            FROM metalmaster;

            -- Parties (Customer list)
            SELECT accode AS Accode, acname AS Acname, ISNULL(trnno, '') AS Trnno 
            FROM accmst 
            WHERE actype = 'C' OR actype = 'G';
        ";

            using var multi = await connection.QueryMultipleAsync(sql);

            var branches = (await multi.ReadAsync<LookupItemDto>()).AsList();
            var divisions = (await multi.ReadAsync<LookupItemDto>()).AsList();
            var vatTypes = (await multi.ReadAsync<LookupItemDto>()).AsList();
            var salesmen = (await multi.ReadAsync<LookupItemDto>()).AsList();
            var rateTypes = (await multi.ReadAsync<RateTypeLookupDto>()).AsList();
            var stockItems = (await multi.ReadAsync<StockItemLookupDto>()).AsList();
            var parties = (await multi.ReadAsync<PartyLookupDto>()).AsList();

            return new JewellerySaleInitDto
            {
                Branches = branches,
                Divisions = divisions,
                VatTypes = vatTypes,
                Salesmen = salesmen,
                RateTypes = rateTypes,
                StockItems = stockItems,
                Parties = parties
            };
        }
    }
}
