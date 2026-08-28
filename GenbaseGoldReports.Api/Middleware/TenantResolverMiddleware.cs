using System.Net;
using System.Text.Json;
using GenbaseGoldReports.Application.Common.Interfaces;

namespace GenbaseGoldReports.Api.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver, ITenantContext tenantContext)
    {
        var host = context.Request.Host.Host;
        var identifier = ExtractSubdomain(host);

        // Fallback for Local Development & Swagger Testing (reads X-Tenant-Id header if no subdomain)
        if (string.IsNullOrEmpty(identifier) && context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenant))
        {
            identifier = headerTenant.ToString().Trim();
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            var tenant = await tenantResolver.ResolveAsync(identifier);

            if (tenant == null)
            {
                await WriteResponseAsync(context, HttpStatusCode.NotFound, $"Tenant '{identifier}' was not found.");
                return;
            }

            if (!tenant.IsActive)
            {
                await WriteResponseAsync(context, HttpStatusCode.Forbidden, $"Tenant '{identifier}' subscription is inactive or suspended.");
                return;
            }

            tenantContext.SetTenant(tenant.Id, tenant.Identifier, tenant.ConnectionString);
        }

        await _next(context);
    }

    private static string ExtractSubdomain(string host)
    {
        if (string.IsNullOrWhiteSpace(host) || host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        var parts = host.Split('.');

        if (parts.Length >= 3)
        {
            return parts[0];
        }

        return string.Empty;
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new { Message = message, StatusCode = (int)statusCode };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}