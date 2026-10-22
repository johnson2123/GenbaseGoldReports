namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance
{
    public record TrialBalanceReportDto(
        DateTime AsOfDate,
        decimal GoldRate,
        decimal SilverRate,
        IReadOnlyList<TrialBalanceItemDto> Items,
        decimal TotalLcAmount,
        decimal TotalGoldGrams,
        decimal TotalSilverGrams,
        decimal TotalNetAmount
    );
}
