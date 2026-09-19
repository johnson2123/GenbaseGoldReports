namespace GenbaseGoldReports.Application.Common.Models
{
    public record NotificationJob(
        NotificationType Type,
        string Target,
        string Subject,
        string Message
    );
}
