using Serilog;

namespace GenbaseGoldReports.Api.Extensions
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());
        }
    }
}
