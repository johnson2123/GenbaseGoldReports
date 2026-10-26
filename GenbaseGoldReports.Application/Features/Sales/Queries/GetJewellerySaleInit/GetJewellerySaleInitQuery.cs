using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleInit
{
    public record GetJewellerySaleInitQuery : IRequest<JewellerySaleInitDto>;
}
