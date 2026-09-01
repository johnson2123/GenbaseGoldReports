using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Options;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Options;
using GenbaseGoldReports.Infrastructure.Persistence;
using GenbaseGoldReports.Infrastructure.Services;
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

            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOtpService, OtpService>();

            return services;
        }
    }
}