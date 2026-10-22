namespace GenbaseGoldReports.Api.Extensions
{
    public static class CorsExtensions
    {
        public const string PolicyName = "GenbaseCorsPolicy";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var configuredOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:5173", "https://localhost:5173" };

            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy
                        .WithOrigins(configuredOrigins)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() 
                        .WithExposedHeaders("Content-Disposition", "X-Tenant-Id");
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            return app.UseCors(PolicyName);
        }
    }
}
