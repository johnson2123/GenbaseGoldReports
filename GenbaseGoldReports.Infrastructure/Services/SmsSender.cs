using GenbaseGoldReports.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(ILogger<SmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("SMS dispatch triggered to: {PhoneNumber}. Payload: {Message}", phoneNumber, message);

            return Task.CompletedTask;
        }
    }
}
