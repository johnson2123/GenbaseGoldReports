namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos
{
    public class AccountFormInitDto
    {
        public string NextAccode { get; set; } = string.Empty;
        public string ParentCode { get; set; } = "0";
        public string ParentName { get; set; } = string.Empty;
        public int CalculatedLevelId { get; set; }
        public string BaseCurrency { get; set; } = "AED";
        public decimal DefaultTaxRate { get; set; } = 5.0m;
        public string? InheritedClass { get; set; }
        public List<AccountTypeLookupDto> AccountTypes { get; set; } = new();
        public List<ClassLookupDto> Classes { get; set; } = new();
        public List<CurrencyLookupDto> Currencies { get; set; } = new();
        public List<GroupLookupDto> Groups { get; set; } = new();
    }
}
