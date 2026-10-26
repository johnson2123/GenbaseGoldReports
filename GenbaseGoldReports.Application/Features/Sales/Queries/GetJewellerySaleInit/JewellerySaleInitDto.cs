namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleInit
{
    public record JewellerySaleInitDto
    {
        public List<LookupItemDto> Branches { get; init; } = new();
        public List<LookupItemDto> Divisions { get; init; } = new();
        public List<LookupItemDto> VatTypes { get; init; } = new();
        public List<LookupItemDto> Salesmen { get; init; } = new();
        public List<RateTypeLookupDto> RateTypes { get; init; } = new();
        public List<StockItemLookupDto> StockItems { get; init; } = new();
        public List<PartyLookupDto> Parties { get; init; } = new();
    }
}
