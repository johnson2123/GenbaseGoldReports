using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockById
{
    public record GetMetalStockByIdQuery(string Stkcode) : IRequest<MetalStockDetailDto>;

}
