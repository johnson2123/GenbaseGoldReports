using GenbaseGoldReports.Application.Common.Models;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySalesLedger
{
    public record GetJewellerySalesLedgerQuery : PagedRequest, IRequest<JewellerySalesLedgerResponse>
    {
        public string? Brncode { get; init; }
        public string? Divcode { get; init; }
        public DateTime? FromDate { get; init; }
        public DateTime? ToDate { get; init; }
        public string? FixUnfix { get; init; }
    }
}
