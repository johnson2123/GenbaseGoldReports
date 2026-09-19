using GenbaseGoldReports.Application.Common.Models;

namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface INotificationQueue
    {
        ValueTask QueueNotificationAsync(NotificationJob job);
        ValueTask<NotificationJob> DequeueNotificationAsync(CancellationToken cancellationToken);
    }
}
