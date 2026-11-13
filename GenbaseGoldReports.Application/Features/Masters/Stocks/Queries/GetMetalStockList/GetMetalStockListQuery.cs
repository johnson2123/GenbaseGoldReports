using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockList
{
    public record GetMetalStockListQuery : PagedRequest, IRequest<PagedResult<MetalStockListItemDto>>
    {
        public string? Mtltype { get; init; }
        public string? Karat { get; init; }
    }
}
