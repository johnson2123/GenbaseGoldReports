using GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockFormInit
{
    public record GetMetalStockFormInitQuery : IRequest<MetalStockFormInitDto>;
}
