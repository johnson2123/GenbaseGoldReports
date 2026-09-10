namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance
{
    public record StockBalanceItemDto(
        int SlNo,
        string ItemCode,
        string ItemDescription,
        int Pieces,
        decimal GrossWeight,
        decimal Purity,
        decimal PureWeight,
        decimal MakingCost,
        decimal MakingAmount,
        decimal MetalValue
    );
}
