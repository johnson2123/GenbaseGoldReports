namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetPartyDetails
{
    public record PartyDetailsDto
    {
        public string Accode { get; init; } = string.Empty;
        public string Acname { get; init; } = string.Empty;
        public string Trnno { get; init; } = string.Empty;
        public string Addr1 { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string DefaultCurr { get; init; } = "AED";
        public decimal DefaultCurrRate { get; init; } = 1.0m;
        public string DefaultVatType { get; init; } = "B2BSAL";
        public List<PartyCurrencyDto> Currencies { get; init; } = new();
    }
}
