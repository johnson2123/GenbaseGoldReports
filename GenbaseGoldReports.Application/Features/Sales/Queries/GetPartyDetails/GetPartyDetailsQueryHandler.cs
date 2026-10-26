using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetPartyDetails
{
    public class GetPartyDetailsQueryHandler : IRequestHandler<GetPartyDetailsQuery, PartyDetailsDto?>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetPartyDetailsQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PartyDetailsDto?> Handle(GetPartyDetailsQuery request, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
            -- Party Profile
            SELECT 
                accode AS Accode, 
                acname AS Acname, 
                ISNULL(trnno, '') AS Trnno, 
                ISNULL(addr1, '') AS Addr1, 
                ISNULL(city, '') AS City, 
                ISNULL(curr, 'AED') AS DefaultCurr, 
                ISNULL(currrate, 1.0) AS DefaultCurrRate,
                ISNULL(saltxntype, 'B2BSAL') AS DefaultVatType
            FROM accmst 
            WHERE accode = @Accode;

            -- Allowed Currencies for this Party
            SELECT curr AS Curr, ISNULL(currname, curr) AS Currname 
            FROM accdet 
            WHERE accode = @Accode;
        ";

            using var multi = await connection.QueryMultipleAsync(sql, new { request.Accode });

            var party = await multi.ReadFirstOrDefaultAsync<PartyDetailsDto>();
            if (party == null) return null;

            var currencies = (await multi.ReadAsync<PartyCurrencyDto>()).AsList();

            // If no explicit currencies assigned in accdet, fallback to default account currency
            if (!currencies.Any() && !string.IsNullOrEmpty(party.DefaultCurr))
            {
                currencies.Add(new PartyCurrencyDto(party.DefaultCurr, party.DefaultCurr));
            }

            return party with { Currencies = currencies };
        }
    }
}
