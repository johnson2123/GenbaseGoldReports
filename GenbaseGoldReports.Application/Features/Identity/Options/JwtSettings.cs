namespace GenbaseGoldReports.Application.Features.Identity.Options
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string Key { get; set; } = string.Empty;
        public string RefreshTokenHashKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpireMinutes { get; set; } = 30;
        public int RefreshTokenExpireMinutes { get; set; } = 120;
    }
}
