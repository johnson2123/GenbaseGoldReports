using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance
{
    public class GetJewelleryStockBalanceQueryHandler
        : IRequestHandler<GetJewelleryStockBalanceQuery, StockBalanceReportDto>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public GetJewelleryStockBalanceQueryHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<StockBalanceReportDto> Handle(
            GetJewelleryStockBalanceQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            const string sql = @"
                WITH StockSummary AS (
                    SELECT 
                        s.stkcode,
                        SUM(s.pieces * s.nfactor) AS Pieces,
                        SUM(s.grqty * s.nfactor) AS GrossWeight
                    FROM stocktransdet s
                    WHERE s.docdate <= @AsOfDate
                    GROUP BY s.stkcode
                    HAVING SUM(s.pieces * s.nfactor) <> 0 OR SUM(s.grqty * s.nfactor) <> 0
                )
                SELECT 
                    m.stkcode AS ItemCode,
                    m.stkname AS ItemDescription,
                    CAST(s.Pieces AS INT) AS Pieces,
                    CAST(s.GrossWeight AS DECIMAL(18, 3)) AS GrossWeight,
                    CAST(m.spurity AS DECIMAL(18, 5)) AS Purity,
                    CAST(COALESCE(m.mkgrate, 0) AS DECIMAL(18, 2)) AS MakingCost
                FROM StockSummary s
                INNER JOIN metalmaster m ON m.stkcode = s.stkcode
                WHERE (@SearchItemCode IS NULL OR m.stkcode = @SearchItemCode)
                ORDER BY m.stkcode;";

            var endOfDay = request.AsOfDate.Date.AddDays(1).AddTicks(-1);

            var rawItems = await connection.QueryAsync<RawStockBalanceQueryResult>(
                new CommandDefinition(
                    sql,
                    new { AsOfDate = endOfDay, SearchItemCode = request.SearchItemCode },
                    cancellationToken: cancellationToken
                )
            );

            var items = new List<StockBalanceItemDto>();
            int slNo = 1;

            foreach (var item in rawItems)
            {
                var pureWeight = Math.Round(item.GrossWeight * item.Purity, 2, MidpointRounding.AwayFromZero);
                var makingAmount = Math.Round(item.GrossWeight * item.MakingCost, 2, MidpointRounding.AwayFromZero);
                var metalValue = Math.Round(pureWeight * request.GoldRate, 2, MidpointRounding.AwayFromZero);

                items.Add(new StockBalanceItemDto(
                    SlNo: slNo++,
                    ItemCode: item.ItemCode,
                    ItemDescription: item.ItemDescription,
                    Pieces: item.Pieces,
                    GrossWeight: item.GrossWeight,
                    Purity: item.Purity,
                    PureWeight: pureWeight,
                    MakingCost: item.MakingCost,
                    MakingAmount: makingAmount,
                    MetalValue: metalValue
                ));
            }

            int totalPieces = items.Sum(i => i.Pieces);
            decimal totalGrossWeight = items.Sum(i => i.GrossWeight);
            decimal totalPureWeight = items.Sum(i => i.PureWeight);
            decimal totalMakingAmount = items.Sum(i => i.MakingAmount);
            decimal totalMetalValue = items.Sum(i => i.MetalValue);

            return new StockBalanceReportDto(
                AsOfDate: request.AsOfDate,
                GoldRate: request.GoldRate,
                Items: items,
                TotalPieces: totalPieces,
                TotalGrossWeight: totalGrossWeight,
                TotalPureWeight: totalPureWeight,
                TotalMakingAmount: totalMakingAmount,
                TotalMetalValue: totalMetalValue
            );
        }

        private sealed record RawStockBalanceQueryResult(
            string ItemCode,
            string ItemDescription,
            int Pieces,
            decimal GrossWeight,
            decimal Purity,
            decimal MakingCost
        );
    }
}