namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance
{
    public record StockBalanceReportDto(
        DateTime AsOfDate,
        decimal GoldRate,
        IReadOnlyCollection<StockBalanceItemDto> Items,
        int TotalPieces,
        decimal TotalGrossWeight,
        decimal TotalPureWeight,
        decimal TotalMakingAmount,
        decimal TotalMetalValue
    );
}
