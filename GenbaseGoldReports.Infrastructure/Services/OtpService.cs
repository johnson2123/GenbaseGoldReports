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

        public OtpService(MasterDbContext db)
        {
            _db = db;
        }

        public async Task GenerateAndSendOtpAsync(string target, string communicationChannel, string purpose, CancellationToken cancellationToken = default)
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
                throw new InvalidOperationException("User account not found for the provided email or phone number.");
            }

            var activeOtps = await _db.OtpLogs
                .Where(x => x.Target == target && x.Purpose == purpose && !x.IsUsed)
                .ToListAsync(cancellationToken);

            foreach (var oldOtp in activeOtps)
            {
                oldOtp.IsUsed = true;
            }

            var newOtp = new OtpLog
            {
                Guid = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                Target = target,
                OtpCode = GenerateSecureOtp(),
                Purpose = purpose,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                AttemptCount = 0,
                CreatedDate = DateTime.UtcNow,
                IsConsumed = false
            };

            _db.OtpLogs.Add(newOtp);
            await _db.SaveChangesAsync(cancellationToken);

            // Dispatches notification (Email/SMS queuing can be hooked up here later)
        }

        public async Task<string> VerifyOtpAsync(string target, string otpCode, string purpose, CancellationToken cancellationToken = default)
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

        public async Task<bool> ValidateActionTokenAsync(string target, string token, string purpose, CancellationToken cancellationToken = default)
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
    }
}
