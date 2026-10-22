using MediatR;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance
{
    public record GetTrialBalanceQuery(
        DateTime AsOfDate,
        decimal GoldRate = 240.00m,
        decimal SilverRate = 3.50m
    ) : IRequest<TrialBalanceReportDto>;
}
