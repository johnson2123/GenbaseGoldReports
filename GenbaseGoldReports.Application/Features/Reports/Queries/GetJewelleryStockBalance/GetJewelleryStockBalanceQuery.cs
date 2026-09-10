using MediatR;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance
{
    public record GetJewelleryStockBalanceQuery(
        DateTime AsOfDate,
        decimal GoldRate,
        string? SearchItemCode = null
    ) : IRequest<StockBalanceReportDto>;
}
