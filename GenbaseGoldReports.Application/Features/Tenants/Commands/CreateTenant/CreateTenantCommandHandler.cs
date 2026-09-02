using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using GenbaseGoldReports.Domain.Entities;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly IMasterDbContext _masterDb;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ITenantProvisioningService _tenantProvisioningService;

    public CreateTenantCommandHandler(
        IMasterDbContext masterDb,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ITenantProvisioningService tenantProvisioningService)
    {
        _masterDb = masterDb;
        _userManager = userManager;
        _configuration = configuration;
        _tenantProvisioningService = tenantProvisioningService;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var sanitizedIdentifier = request.Identifier.Trim().ToLowerInvariant();

        var identifierExists = await _masterDb.Tenants
            .AnyAsync(t => t.Identifier == sanitizedIdentifier, cancellationToken);

        if (identifierExists)
        {
            throw new InvalidOperationException($"Tenant with identifier '{sanitizedIdentifier}' already exists.");
        }

        string tenantConnString;
        bool isCustomDb = !string.IsNullOrWhiteSpace(request.CustomConnectionString);

        if (isCustomDb)
        {
            tenantConnString = request.CustomConnectionString!.Trim();
        }
        else
        {
            var masterConn = _configuration.GetConnectionString("MasterConnection")
                ?? throw new InvalidOperationException("ConnectionString 'MasterConnection' is missing from configuration.");

            var tenantDbName = $"GenbaseGoldReports_{sanitizedIdentifier}";
            tenantConnString = BuildTenantConnectionString(masterConn, tenantDbName);
        }

        var tenant = new Tenant(
            name: request.Name.Trim(),
            identifier: sanitizedIdentifier,
            connectionString: tenantConnString,
            subscriptionPlan: request.SubscriptionPlan,
            subscriptionExpiresAtUtc: request.SubscriptionExpiresAtUtc
        );

        _masterDb.Tenants.Add(tenant);
        await _masterDb.SaveChangesAsync(cancellationToken);

        if (!isCustomDb)
        {
            await _tenantProvisioningService.ProvisionTenantDatabaseAsync(tenantConnString, cancellationToken);
        }

        var adminUser = await _userManager.FindByEmailAsync(request.AdminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = request.AdminEmail,
                Email = request.AdminEmail,
                FirstName = request.AdminFirstName,
                LastName = request.AdminLastName,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(adminUser, request.AdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create tenant admin user: {errors}");
            }

            await _userManager.AddToRoleAsync(adminUser, "TenantAdmin");
        }

        _masterDb.UserTenants.Add(new UserTenant(adminUser.Id, tenant.Id, "TenantAdmin"));
        await _masterDb.SaveChangesAsync(cancellationToken);

        return new TenantDto(
            tenant.Id,
            tenant.Name,
            tenant.Identifier,
            tenant.IsActive,
            tenant.SubscriptionPlan,
            tenant.SubscriptionExpiresAtUtc,
            tenant.CreatedAtUtc,
            tenant.UpdatedAtUtc
        );
    }

    private static string BuildTenantConnectionString(string masterConn, string dbName)
    {
        var builder = new SqlConnectionStringBuilder(masterConn)
        {
            InitialCatalog = dbName
        };
        return builder.ConnectionString;
    }
}