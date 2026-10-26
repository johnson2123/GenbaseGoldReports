namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleInit
{
    public record RateTypeLookupDto(string Code, string Description, decimal ConvFact, decimal CurrRate, decimal GozRate);
}
