namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos
{
    public class GroupLookupDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int LevelId { get; set; }
        public string? Class { get; set; }
    }
}
