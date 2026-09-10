using MediatR;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition
{
    public record GetNetGoldPositionQuery(
        DateTime AsOfDate,
        decimal GoldRate
    ) : IRequest<NetGoldPositionReportDto>;
}
