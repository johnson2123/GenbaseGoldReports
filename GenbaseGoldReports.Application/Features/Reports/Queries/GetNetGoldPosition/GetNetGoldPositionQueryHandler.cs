using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition
{
    public class GetNetGoldPositionQueryHandler
        : IRequestHandler<GetNetGoldPositionQuery, NetGoldPositionReportDto>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public GetNetGoldPositionQueryHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<NetGoldPositionReportDto> Handle(
            GetNetGoldPositionQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            const string sql = @"
                WITH Config AS (
                    SELECT TOP 1
                        CAST(goldsohgroup AS VARCHAR(50)) AS SohGroup,
                        CAST(goldworkshopgroup AS VARCHAR(50)) AS WorkshopGroup,
                        CAST(goldtransitgroup AS VARCHAR(50)) AS TransitGroup
                    FROM configmst3
                ),
                Categories AS (
                    SELECT 1 AS SlNo, 'Customers' AS Description
                    UNION ALL SELECT 2, 'Suppliers'
                    UNION ALL SELECT 3, 'Stock on Hand'
                    UNION ALL SELECT 4, 'Stock at Factory'
                    UNION ALL SELECT 5, 'Stock at Transit'
                    UNION ALL SELECT 6, 'Others'
                ),
                LedgerSummary AS (
                    SELECT 
                        CASE 
                            WHEN a.actype = 'C' AND a.isacc = '1' THEN 1
                            WHEN a.actype = 'S' AND a.isacc = '1' THEN 2
                            WHEN c.SohGroup IS NOT NULL AND a.grpcode = c.SohGroup THEN 3
                            WHEN c.WorkshopGroup IS NOT NULL AND a.grpcode = c.WorkshopGroup THEN 4
                            WHEN c.TransitGroup IS NOT NULL AND a.grpcode = c.TransitGroup THEN 5
                            WHEN a.maingroup <= 2 THEN 6
                            ELSE 0
                        END AS CategoryId,
                        SUM(l.mtlqty) AS PureWeight
                    FROM ledgertran l
                    INNER JOIN accmst a ON a.accode = l.accode
                    CROSS JOIN Config c
                    WHERE l.docdate <= @AsOfDate
                    GROUP BY 
                        CASE 
                            WHEN a.actype = 'C' AND a.isacc = '1' THEN 1
                            WHEN a.actype = 'S' AND a.isacc = '1' THEN 2
                            WHEN c.SohGroup IS NOT NULL AND a.grpcode = c.SohGroup THEN 3
                            WHEN c.WorkshopGroup IS NOT NULL AND a.grpcode = c.WorkshopGroup THEN 4
                            WHEN c.TransitGroup IS NOT NULL AND a.grpcode = c.TransitGroup THEN 5
                            WHEN a.maingroup <= 2 THEN 6
                            ELSE 0
                        END
                )
                SELECT 
                    cat.SlNo,
                    cat.Description,
                    CAST(COALESCE(s.PureWeight, 0) AS DECIMAL(18, 2)) AS PureWeight
                FROM Categories cat
                LEFT JOIN LedgerSummary s ON cat.SlNo = s.CategoryId
                ORDER BY cat.SlNo;";

            var endOfDay = request.AsOfDate.Date.AddDays(1).AddTicks(-1);

            var rawItems = await connection.QueryAsync<RawPositionQueryResult>(
                new CommandDefinition(
                    sql,
                    new { AsOfDate = endOfDay },
                    cancellationToken: cancellationToken
                )
            );

            var items = new List<NetGoldPositionItemDto>();

            foreach (var item in rawItems)
            {
                var pureWeight = Math.Round(item.PureWeight, 2, MidpointRounding.AwayFromZero);
                var metalValue = Math.Round(pureWeight * request.GoldRate, 2, MidpointRounding.AwayFromZero);

                items.Add(new NetGoldPositionItemDto(
                    SlNo: item.SlNo,
                    Description: item.Description,
                    PureWeight: pureWeight,
                    MetalValue: metalValue
                ));
            }

            decimal totalPureWeight = items.Sum(i => i.PureWeight);
            decimal totalMetalValue = items.Sum(i => i.MetalValue);

            return new NetGoldPositionReportDto(
                AsOfDate: request.AsOfDate,
                GoldRate: request.GoldRate,
                Items: items,
                TotalPureWeight: totalPureWeight,
                TotalMetalValue: totalMetalValue
            );
        }

        private sealed record RawPositionQueryResult(
            int SlNo,
            string Description,
            decimal PureWeight
        );
    }
}