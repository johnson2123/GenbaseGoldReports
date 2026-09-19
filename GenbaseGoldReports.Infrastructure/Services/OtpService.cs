using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using GenbaseGoldReports.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly MasterDbContext _db;
        private readonly INotificationQueue _queue;

        public OtpService(MasterDbContext db, INotificationQueue queue)
        {
            _db = db;
            _queue = queue;
        }

        public async Task GenerateAndSendOtpAsync(
            string target,
            string communicationChannel,
            string purpose,
            CancellationToken cancellationToken = default)
        {
            var hasRecentOtp = await _db.OtpLogs
                .AnyAsync(x => x.Target == target &&
                               x.Purpose == purpose &&
                               x.CreatedDate > DateTime.UtcNow.AddSeconds(-60),
                          cancellationToken);

            if (hasRecentOtp)
            {
                throw new InvalidOperationException("Please wait 60 seconds before requesting a new verification code.");
            }

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.Email != null && u.Email == target) ||
                                          (u.PhoneNumber != null && u.PhoneNumber == target),
                                     cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException("User account not found for the provided target.");
            }

            var activeOtps = await _db.OtpLogs
                .Where(x => x.Target == target && x.Purpose == purpose && !x.IsUsed)
                .ToListAsync(cancellationToken);

            foreach (var oldOtp in activeOtps)
            {
                oldOtp.IsUsed = true;
            }

            var otpCode = GenerateSecureOtp();

            var newOtp = new OtpLog
            {
                Guid = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                Target = target,
                OtpCode = otpCode,
                Purpose = purpose,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                AttemptCount = 0,
                CreatedDate = DateTime.UtcNow,
                IsConsumed = false
            };

            _db.OtpLogs.Add(newOtp);
            await _db.SaveChangesAsync(cancellationToken);

            await DispatchOtpNotificationAsync(user, otpCode, communicationChannel);
        }

        public async Task<string> VerifyOtpAsync(
            string target,
            string otpCode,
            string purpose,
            CancellationToken cancellationToken = default)
        {
            var otpLog = await _db.OtpLogs
                .Where(x => x.Target == target && x.Purpose == purpose && !x.IsUsed && x.ExpiryTime > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (otpLog == null)
            {
                throw new InvalidOperationException("The verification code is invalid or has expired.");
            }

            if (otpLog.AttemptCount >= 3)
            {
                otpLog.IsUsed = true;
                await _db.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("Too many incorrect attempts. Please request a new verification code.");
            }

            if (otpLog.OtpCode != otpCode)
            {
                otpLog.AttemptCount++;
                await _db.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("The verification code is incorrect.");
            }

            otpLog.IsUsed = true;
            otpLog.VerifiedDate = DateTime.UtcNow;
            otpLog.ActionToken = GenerateSecureToken();
            otpLog.ActionTokenExpiry = DateTime.UtcNow.AddMinutes(10);

            await _db.SaveChangesAsync(cancellationToken);

            return otpLog.ActionToken;
        }

        public async Task<bool> ValidateActionTokenAsync(
            string target,
            string token,
            string purpose,
            CancellationToken cancellationToken = default)
        {
            return await _db.OtpLogs
                .AnyAsync(x => x.Target == target &&
                               x.ActionToken == token &&
                               x.Purpose == purpose &&
                               !x.IsConsumed &&
                               x.ActionTokenExpiry > DateTime.UtcNow,
                          cancellationToken);
        }

        public async Task InvalidateActionTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var otpLog = await _db.OtpLogs
                .FirstOrDefaultAsync(x => x.ActionToken == token, cancellationToken);

            if (otpLog != null)
            {
                otpLog.IsConsumed = true;
                otpLog.ConsumedDate = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        private static string GenerateSecureOtp()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        private static string GenerateSecureToken()
        {
            var buffer = new byte[32];
            RandomNumberGenerator.Fill(buffer);
            return Convert.ToHexString(buffer);
        }

        private async Task DispatchOtpNotificationAsync(ApplicationUser user, string otpCode, string communicationChannel)
        {
            bool sendEmail = string.Equals(communicationChannel, "Email", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(communicationChannel, "All", StringComparison.OrdinalIgnoreCase);

            bool sendSms = string.Equals(communicationChannel, "Sms", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(communicationChannel, "All", StringComparison.OrdinalIgnoreCase);

            if (sendEmail && !string.IsNullOrWhiteSpace(user.Email))
            {
                var emailJob = new NotificationJob(
                    Type: NotificationType.Email,
                    Target: user.Email,
                    Subject: "Genbase Gold Reports - Verification Code",
                    Message: BuildOtpEmailBody(otpCode)
                );

                await _queue.QueueNotificationAsync(emailJob);
            }

            if (sendSms && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var smsJob = new NotificationJob(
                    Type: NotificationType.Sms,
                    Target: user.PhoneNumber,
                    Subject: string.Empty,
                    Message: $"Your Genbase Gold Reports verification code is: {otpCode}. Valid for 5 minutes."
                );

                await _queue.QueueNotificationAsync(smsJob);
            }
        }

        private static string BuildOtpEmailBody(string otpCode)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 520px; margin: 0 auto; padding: 24px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                <h2 style='color: #d97706; margin-top: 0;'>Verification Code</h2>
                <p style='color: #475569; font-size: 15px;'>Use the code below to complete your authentication request.</p>
                <div style='background-color: #f8fafc; border: 1px dashed #cbd5e1; text-align: center; padding: 18px; margin: 24px 0; border-radius: 6px;'>
                    <span style='font-size: 32px; font-weight: 700; letter-spacing: 6px; color: #0f172a;'>{otpCode}</span>
                </div>
                <p style='color: #94a3b8; font-size: 12px; margin-bottom: 0;'>This code is valid for 5 minutes. If you did not request this code, please ignore this message.</p>
            </div>";
        }
    }
}
