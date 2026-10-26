namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleInit
{
    public record StockItemLookupDto(string Stkcode, string Stkname, decimal Apurity, decimal Spurity, decimal MkgTaxRate, decimal MtlTaxRate);
}
