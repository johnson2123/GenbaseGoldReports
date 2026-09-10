namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition
{
    public record NetGoldPositionItemDto(
        int SlNo,
        string Description,
        decimal PureWeight,
        decimal MetalValue
    );
}
