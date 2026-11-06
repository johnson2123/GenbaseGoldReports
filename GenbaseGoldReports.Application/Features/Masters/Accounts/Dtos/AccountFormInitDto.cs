namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos
{
    public class AccountFormInitDto
    {
        public string NextAccode { get; set; } = string.Empty;
        public string ParentCode { get; set; } = "0";
        public string ParentName { get; set; } = string.Empty;
        public int CalculatedLevelId { get; set; }
        public List<AccountTypeLookupDto> AccountTypes { get; set; } = new();
        public List<string> Currencies { get; set; } = new();
    }
}
