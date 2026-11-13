namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos
{
    public class KaratLookupDto
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal StandardPurity { get; set; }      
        public decimal SalesPurity { get; set; }         
    }
}
