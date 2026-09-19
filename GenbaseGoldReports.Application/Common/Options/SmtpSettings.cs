namespace GenbaseGoldReports.Application.Common.Options
{
    public class SmtpSettings
    {
        public const string SectionName = "SmtpSettings";

        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 465;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
