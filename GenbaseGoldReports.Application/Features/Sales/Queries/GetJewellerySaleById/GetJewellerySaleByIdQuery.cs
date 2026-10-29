using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleById
{
    public record GetJewellerySaleByIdQuery(
    int Docno,
    string? Doctype = null) : IRequest<JewellerySaleDetailsDto?>;
}
