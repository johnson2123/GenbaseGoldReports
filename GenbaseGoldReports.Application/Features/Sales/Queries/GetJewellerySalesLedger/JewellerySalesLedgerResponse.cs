using GenbaseGoldReports.Application.Common.Models;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySalesLedger
{
    public record JewellerySalesLedgerResponse(
        PagedResult<JewellerySaleLedgerItemDto> GridData,
        JewellerySalesSummaryKpiDto SummaryKpis
    );
}
