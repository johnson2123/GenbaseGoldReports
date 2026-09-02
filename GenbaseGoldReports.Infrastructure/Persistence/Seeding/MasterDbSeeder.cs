using GenbaseGoldReports.Domain.Entities;
using GenbaseGoldReports.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenbaseGoldReports.Infrastructure.Persistence.Seeding;

public static class MasterDbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.MigrateAsync();

        var masterConn = configuration.GetConnectionString("MasterConnection")
            ?? throw new InvalidOperationException("ConnectionString 'MasterConnection' missing.");

        var superAdminEmail = configuration["Seed:SuperAdminEmail"] ?? "superadmin@genbasegold.com";
        var superAdminPass = configuration["Seed:SuperAdminPassword"] ?? "SuperAdmin@123456";
        var clientAdminEmail = configuration["Seed:ClientAdminEmail"] ?? "admin@clienta.com";
        var clientAdminPass = configuration["Seed:ClientAdminPassword"] ?? "ClientAdmin@123456";

        string[] roles = { "SuperAdmin", "TenantAdmin", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName, $"{roleName} role"));
            }
        }

        var adminTenant = await context.Tenants.FirstOrDefaultAsync(t => t.Identifier == "admin");
        if (adminTenant == null)
        {
            // System Control Platform: Enterprise plan with no expiration
            adminTenant = new Tenant(
                name: "System Control Platform",
                identifier: "admin",
                connectionString: masterConn,
                subscriptionPlan: TenantSubscriptionPlan.Enterprise,
                subscriptionExpiresAtUtc: null
            );
            await context.Tenants.AddAsync(adminTenant);
            await context.SaveChangesAsync();
        }

        var clientATenant = await context.Tenants.FirstOrDefaultAsync(t => t.Identifier == "clienta");
        if (clientATenant == null)
        {
            var devConn = BuildTenantConnString(masterConn, "GenbaseGoldReports_ClientA");
            // Seed Client A: Standard plan expiring in 1 year
            clientATenant = new Tenant(
                name: "Client A Testing",
                identifier: "clienta",
                connectionString: devConn,
                subscriptionPlan: TenantSubscriptionPlan.Standard,
                subscriptionExpiresAtUtc: DateTime.UtcNow.AddYears(1)
            );
            await context.Tenants.AddAsync(clientATenant);
            await context.SaveChangesAsync();
        }

        var superUser = await userManager.FindByEmailAsync(superAdminEmail);
        if (superUser == null)
        {
            superUser = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                FirstName = "System",
                LastName = "SuperAdmin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(superUser, superAdminPass);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superUser, "SuperAdmin");
                context.UserTenants.Add(new UserTenant(superUser.Id, adminTenant.Id, "SuperAdmin"));
                await context.SaveChangesAsync();
            }
        }

        var clientUser = await userManager.FindByEmailAsync(clientAdminEmail);
        if (clientUser == null)
        {
            clientUser = new ApplicationUser
            {
                UserName = clientAdminEmail,
                Email = clientAdminEmail,
                FirstName = "ClientA",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(clientUser, clientAdminPass);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(clientUser, "TenantAdmin");
                context.UserTenants.Add(new UserTenant(clientUser.Id, clientATenant.Id, "TenantAdmin"));
                await context.SaveChangesAsync();
            }
        }
    }

    private static string BuildTenantConnString(string masterConn, string dbName)
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(masterConn)
        {
            InitialCatalog = dbName
        };
        return builder.ConnectionString;
    }
}