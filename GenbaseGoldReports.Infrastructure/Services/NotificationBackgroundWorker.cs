using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class NotificationBackgroundWorker : BackgroundService
    {
        private readonly INotificationQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundWorker> _logger;

        public NotificationBackgroundWorker(
            INotificationQueue queue,
            IServiceProvider serviceProvider,
            ILogger<NotificationBackgroundWorker> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var job = await _queue.DequeueNotificationAsync(stoppingToken);

                    _logger.LogInformation("Processing background notification ({Type}) for: {Target}", job.Type, job.Target);

                    using var scope = _serviceProvider.CreateScope();

                    if (job.Type == NotificationType.Email)
                    {
                        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                        await emailSender.SendEmailAsync(
                            job.Target,
                            job.Subject,
                            job.Message,
                            isHtml: true,
                            cancellationToken: stoppingToken);
                    }
                    else if (job.Type == NotificationType.Sms)
                    {
                        var smsSender = scope.ServiceProvider.GetRequiredService<ISmsSender>();
                        await smsSender.SendSmsAsync(
                            job.Target,
                            job.Message,
                            cancellationToken: stoppingToken);
                    }

                    _logger.LogInformation("Successfully dispatched notification to: {Target}", job.Target);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to execute background notification job.");
                }
            }

            _logger.LogInformation("Notification Background Worker stopped.");
        }
    }
}
