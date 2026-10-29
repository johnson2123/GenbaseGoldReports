namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySalesLedger
{
    public record JewellerySalesSummaryKpiDto(
        int TotalInvoicesCount,
        decimal TotalDispatchedPureWeight,
        decimal TotalMakingReceivable,
        decimal TotalUnfixedGoldExposure
    );
}
