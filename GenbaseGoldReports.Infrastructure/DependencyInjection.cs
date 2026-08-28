using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Options;
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

            services.AddMemoryCache();

            services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MasterConnection")));

            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver, TenantResolver>();

            services.AddDbContext<ApplicationDbContext>();

            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            return services;
        }
    }
}
