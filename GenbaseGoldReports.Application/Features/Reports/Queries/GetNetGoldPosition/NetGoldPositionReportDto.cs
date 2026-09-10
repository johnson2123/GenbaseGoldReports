namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition
{
    public record NetGoldPositionReportDto(
        DateTime AsOfDate,
        decimal GoldRate,
        IReadOnlyList<NetGoldPositionItemDto> Items,
        decimal TotalPureWeight,
        decimal TotalMetalValue
    );
}
