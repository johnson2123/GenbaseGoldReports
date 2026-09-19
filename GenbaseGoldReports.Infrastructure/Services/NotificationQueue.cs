using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using System.Threading.Channels;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class NotificationQueue : INotificationQueue
    {
        private readonly Channel<NotificationJob> _channel;

        public NotificationQueue()
        {
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<NotificationJob>(options);
        }

        public async ValueTask QueueNotificationAsync(NotificationJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            await _channel.Writer.WriteAsync(job);
        }

        public async ValueTask<NotificationJob> DequeueNotificationAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
