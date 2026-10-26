using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetPartyDetails
{
    public record GetPartyDetailsQuery(string Accode) : IRequest<PartyDetailsDto?>;
}
