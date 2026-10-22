using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance
{
    public class GetTrialBalanceQueryHandler : IRequestHandler<GetTrialBalanceQuery, TrialBalanceReportDto>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public GetTrialBalanceQueryHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<TrialBalanceReportDto> Handle(GetTrialBalanceQuery request, CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            const string sql = @"
                WITH LedgerAgg AS (
                    SELECT 
                        l.accode,
                        SUM(COALESCE(l.lcamount, 0)) AS LcAmount,
                        SUM(CASE WHEN l.mtltype = 'G' THEN COALESCE(l.mtlqty, 0) ELSE 0 END) AS GoldGrams,
                        SUM(CASE WHEN l.mtltype = 'S' THEN COALESCE(l.mtlqty, 0) ELSE 0 END) AS SilverGrams
                    FROM ledgertran l
                    WHERE l.docdate <= @AsOfDate
                    GROUP BY l.accode
                )
                SELECT 
                    CAST(COALESCE(a.keycode, 0) AS DECIMAL(28, 4)) AS KeyCode,
                    RTRIM(LTRIM(COALESCE(a.accode, ''))) AS AccountCode,
                    RTRIM(LTRIM(COALESCE(a.acname, ''))) AS AccountName,
                    COALESCE(TRY_CAST(a.levelid AS INT), 0) AS LevelId,
                    RTRIM(LTRIM(COALESCE(a.actype, 'G'))) AS AccountType,
                    CASE WHEN a.isacc = '1' THEN 1 ELSE 0 END AS IsAccount,
                    COALESCE(TRY_CAST(a.grpcode AS INT), 0) AS GroupCode,
                    COALESCE(TRY_CAST(a.maingroup AS INT), 0) AS MainGroup,
                    l.LcAmount,
                    l.GoldGrams,
                    l.SilverGrams
                FROM accmst a
                LEFT JOIN LedgerAgg l ON l.accode = a.accode
                ORDER BY a.keycode ASC;";

            var endOfDay = request.AsOfDate.Date.AddDays(1).AddTicks(-1);

            var rawRows = (await connection.QueryAsync<RawTrialBalanceRow>(
                new CommandDefinition(
                    sql,
                    new { AsOfDate = endOfDay },
                    cancellationToken: cancellationToken
                )
            )).ToList();

            var resultItems = new List<TrialBalanceItemDto>(rawRows.Count * 2);
            var groupStack = new Stack<GroupAccumulator>();

            decimal grandTotalLc = 0m;
            decimal grandTotalGold = 0m;
            decimal grandTotalSilver = 0m;
            decimal grandTotalNet = 0m;

            foreach (var row in rawRows)
            {
                while (groupStack.Count > 0 && row.LevelId <= groupStack.Peek().LevelId)
                {
                    var closedGroup = groupStack.Pop();

                    if (closedGroup.HasChildren)
                    {
                        var subNet = Math.Round(
                            closedGroup.SumLc + (closedGroup.SumGold * request.GoldRate) + (closedGroup.SumSilver * request.SilverRate),
                            2,
                            MidpointRounding.AwayFromZero
                        );

                        resultItems.Add(new TrialBalanceItemDto(
                            KeyCode: closedGroup.KeyCode,
                            AccountCode: string.Empty,
                            AccountName: $"Sub Total : {closedGroup.Name}",
                            LevelId: closedGroup.LevelId,
                            AccountType: "SUBTOTAL",
                            IsAccount: false,
                            GroupCode: 0,
                            MainGroup: 0,
                            LcAmount: Math.Round(closedGroup.SumLc, 2, MidpointRounding.AwayFromZero),
                            GoldGrams: Math.Round(closedGroup.SumGold, 3, MidpointRounding.AwayFromZero),
                            SilverGrams: Math.Round(closedGroup.SumSilver, 3, MidpointRounding.AwayFromZero),
                            NetAmount: subNet,
                            IsSubTotal: true
                        ));

                        if (groupStack.Count > 0)
                        {
                            var parent = groupStack.Peek();
                            parent.SumLc += closedGroup.SumLc;
                            parent.SumGold += closedGroup.SumGold;
                            parent.SumSilver += closedGroup.SumSilver;
                            parent.HasChildren = true;
                        }
                    }
                }

                if (row.IsAccount == 0)
                {
                    resultItems.Add(new TrialBalanceItemDto(
                        KeyCode: row.KeyCode,
                        AccountCode: row.AccountCode,
                        AccountName: row.AccountName,
                        LevelId: row.LevelId,
                        AccountType: row.AccountType,
                        IsAccount: false,
                        GroupCode: row.GroupCode,
                        MainGroup: row.MainGroup,
                        LcAmount: null,
                        GoldGrams: null,
                        SilverGrams: null,
                        NetAmount: null,
                        IsSubTotal: false
                    ));

                    groupStack.Push(new GroupAccumulator
                    {
                        KeyCode = row.KeyCode,
                        Name = row.AccountName,
                        LevelId = row.LevelId
                    });
                }
                else
                {
                    var lc = row.LcAmount ?? 0m;
                    var gold = row.GoldGrams ?? 0m;
                    var silver = row.SilverGrams ?? 0m;

                    var net = Math.Round(
                        lc + (gold * request.GoldRate) + (silver * request.SilverRate),
                        2,
                        MidpointRounding.AwayFromZero
                    );

                    resultItems.Add(new TrialBalanceItemDto(
                        KeyCode: row.KeyCode,
                        AccountCode: row.AccountCode,
                        AccountName: row.AccountName,
                        LevelId: row.LevelId,
                        AccountType: row.AccountType,
                        IsAccount: true,
                        GroupCode: row.GroupCode,
                        MainGroup: row.MainGroup,
                        LcAmount: row.LcAmount.HasValue ? Math.Round(lc, 2, MidpointRounding.AwayFromZero) : null,
                        GoldGrams: row.GoldGrams.HasValue ? Math.Round(gold, 3, MidpointRounding.AwayFromZero) : null,
                        SilverGrams: row.SilverGrams.HasValue ? Math.Round(silver, 3, MidpointRounding.AwayFromZero) : null,
                        NetAmount: (row.LcAmount.HasValue || row.GoldGrams.HasValue || row.SilverGrams.HasValue) ? net : null,
                        IsSubTotal: false
                    ));

                    if (groupStack.Count > 0)
                    {
                        var currentGroup = groupStack.Peek();
                        currentGroup.SumLc += lc;
                        currentGroup.SumGold += gold;
                        currentGroup.SumSilver += silver;
                        currentGroup.HasChildren = true;
                    }

                    grandTotalLc += lc;
                    grandTotalGold += gold;
                    grandTotalSilver += silver;
                    grandTotalNet += net;
                }
            }

            while (groupStack.Count > 0)
            {
                var closedGroup = groupStack.Pop();
                if (closedGroup.HasChildren)
                {
                    var subNet = Math.Round(
                        closedGroup.SumLc + (closedGroup.SumGold * request.GoldRate) + (closedGroup.SumSilver * request.SilverRate),
                        2,
                        MidpointRounding.AwayFromZero
                    );

                    resultItems.Add(new TrialBalanceItemDto(
                        KeyCode: closedGroup.KeyCode,
                        AccountCode: string.Empty,
                        AccountName: $"Sub Total : {closedGroup.Name}",
                        LevelId: closedGroup.LevelId,
                        AccountType: "SUBTOTAL",
                        IsAccount: false,
                        GroupCode: 0,
                        MainGroup: 0,
                        LcAmount: Math.Round(closedGroup.SumLc, 2, MidpointRounding.AwayFromZero),
                        GoldGrams: Math.Round(closedGroup.SumGold, 3, MidpointRounding.AwayFromZero),
                        SilverGrams: Math.Round(closedGroup.SumSilver, 3, MidpointRounding.AwayFromZero),
                        NetAmount: subNet,
                        IsSubTotal: true
                    ));

                    if (groupStack.Count > 0)
                    {
                        var parent = groupStack.Peek();
                        parent.SumLc += closedGroup.SumLc;
                        parent.SumGold += closedGroup.SumGold;
                        parent.SumSilver += closedGroup.SumSilver;
                        parent.HasChildren = true;
                    }
                }
            }

            return new TrialBalanceReportDto(
                AsOfDate: request.AsOfDate,
                GoldRate: request.GoldRate,
                SilverRate: request.SilverRate,
                Items: resultItems,
                TotalLcAmount: Math.Round(grandTotalLc, 2, MidpointRounding.AwayFromZero),
                TotalGoldGrams: Math.Round(grandTotalGold, 3, MidpointRounding.AwayFromZero),
                TotalSilverGrams: Math.Round(grandTotalSilver, 3, MidpointRounding.AwayFromZero),
                TotalNetAmount: Math.Round(grandTotalNet, 2, MidpointRounding.AwayFromZero)
            );
        }

        private sealed class GroupAccumulator
        {
            public decimal KeyCode { get; init; }
            public string Name { get; init; } = string.Empty;
            public int LevelId { get; init; }
            public decimal SumLc { get; set; }
            public decimal SumGold { get; set; }
            public decimal SumSilver { get; set; }
            public bool HasChildren { get; set; }
        }

        private sealed class RawTrialBalanceRow
        {
            public decimal KeyCode { get; set; }
            public string AccountCode { get; set; } = string.Empty;
            public string AccountName { get; set; } = string.Empty;
            public int LevelId { get; set; }
            public string AccountType { get; set; } = string.Empty;
            public int IsAccount { get; set; }
            public int GroupCode { get; set; }
            public int MainGroup { get; set; }
            public decimal? LcAmount { get; set; }
            public decimal? GoldGrams { get; set; }
            public decimal? SilverGrams { get; set; }
        }
    }
}