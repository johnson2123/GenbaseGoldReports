namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos
{
    public class CurrencyLookupDto
    {
        public string Currcode { get; set; } = string.Empty;
        public string Curr => Currcode; // alias to prevent mismatch
        public string Currname { get; set; } = string.Empty;
        public decimal Currrate { get; set; } = 1.0m;
    }
}
