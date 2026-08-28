namespace GenbaseGoldReports.Application.Common.Options
{
    public class TenantSettings
    {
        public const string SectionName = "TenantSettings";
        public int CacheDurationInMinutes { get; set; } = 60;
    }
}
