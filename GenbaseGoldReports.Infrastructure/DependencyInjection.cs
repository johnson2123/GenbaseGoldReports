using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Options;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Options;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition;
using GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance;
using GenbaseGoldReports.Infrastructure.Persistence;
using GenbaseGoldReports.Infrastructure.Services;
using GenbaseGoldReports.Infrastructure.Services.Export;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenbaseGoldReports.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<TenantSettings>(configuration.GetSection(TenantSettings.SectionName));
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services.AddMemoryCache();

            services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MasterConnection")));
            services.AddScoped<IMasterDbContext>(provider => provider.GetRequiredService<MasterDbContext>());

            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver, TenantResolver>();
            services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();

            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOtpService, OtpService>();

            services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
            services.AddSingleton<INotificationQueue, NotificationQueue>();
            services.AddTransient<IEmailSender, SmtpEmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddHostedService<NotificationBackgroundWorker>();

            services.AddScoped<IReportExportService<StockBalanceReportDto>, StockBalanceExportService>();
            services.AddScoped<IReportExportService<NetGoldPositionReportDto>, NetGoldPositionExportService>();
            services.AddScoped<IReportExportService<TrialBalanceReportDto>, TrialBalanceExportService>();

            return services;
        }
    }
}