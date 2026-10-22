using GenbaseGoldReports.Domain.Entities.Navigation;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Infrastructure.Persistence.Seeding
{
    public static class NavigationSeeder
    {
        public static async Task SeedNavigationAsync(MasterDbContext context)
        {
            if (await context.MenuItems.AnyAsync())
            {
                return;
            }

            var utcNow = DateTime.UtcNow;

            // 1. Dashboard (Common to all roles)
            var dashboard = new MenuItem
            {
                Id = Guid.NewGuid(),
                Title = "DASHBOARD",
                Path = "/dashboard",
                Icon = "LayoutDashboard",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            // 2. Tenants (SuperAdmin only)
            var tenantMgmt = new MenuItem
            {
                Id = Guid.NewGuid(),
                Title = "TENANTS",
                Path = "/tenants",
                Icon = "Building2",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            // 3. Reports Parent (TenantAdmin)
            var reportsParent = new MenuItem
            {
                Id = Guid.NewGuid(),
                Title = "REPORTS",
                Path = "/reports",
                Icon = "FileBarChart",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var stockBalance = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = reportsParent.Id,
                Title = "Stock Balance",
                Path = "/reports/stock-balance",
                Icon = "Boxes",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var netGoldPosition = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = reportsParent.Id,
                Title = "Net Gold Position",
                Path = "/reports/net-gold-position",
                Icon = "Scale",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var trialBalance = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = reportsParent.Id,
                Title = "Trial Balance",
                Path = "/reports/trial-balance",
                Icon = "ReceiptText",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            // 4. Sales Section (TenantAdmin)
            var salesParent = new MenuItem
            {
                Id = Guid.NewGuid(),
                Title = "SALES",
                Path = "/sales",
                Icon = "BadgePercent",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var invoices = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Invoices",
                Path = "/sales/invoices",
                Icon = "FileText",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var creditNotes = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Credit Notes",
                Path = "/sales/credit-notes",
                Icon = "FileDiff",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalSalesJewellery = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Metal Sales Jewellery",
                Path = "/sales/metal-sales-jewellery",
                Icon = "Gem",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalSalesBullion = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Metal Sales Bullion",
                Path = "/sales/metal-sales-bullion",
                Icon = "Coins",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalPayment = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Metal Payment",
                Path = "/sales/metal-payment",
                Icon = "CreditCard",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalSalesFixing = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Metal Sales Fixing",
                Path = "/sales/metal-sales-fixing",
                Icon = "SlidersHorizontal",
                DisplayOrder = 6,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalSalesHedging = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = salesParent.Id,
                Title = "Metal Sales Hedging",
                Path = "/sales/metal-sales-hedging",
                Icon = "ShieldAlert",
                DisplayOrder = 7,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            // 5. Purchases Section (TenantAdmin)
            var purchasesParent = new MenuItem
            {
                Id = Guid.NewGuid(),
                Title = "PURCHASES",
                Path = "/purchases",
                Icon = "ShoppingBag",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var bills = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Bills",
                Path = "/purchases/bills",
                Icon = "Receipt",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var debitNotes = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Debit Notes",
                Path = "/purchases/debit-notes",
                Icon = "FileMinus",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalPurchaseJewellery = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Metal Purchase Jewellery",
                Path = "/purchases/metal-purchase-jewellery",
                Icon = "Sparkles",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalPurchaseBullion = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Metal Purchase Bullion",
                Path = "/purchases/metal-purchase-bullion",
                Icon = "Boxes",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalReceipts = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Metal Receipts",
                Path = "/purchases/metal-receipts",
                Icon = "Receipt",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalPurchaseFixing = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Metal Purchase Fixing",
                Path = "/purchases/metal-purchase-fixing",
                Icon = "Pin",
                DisplayOrder = 6,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var metalPurchaseHedging = new MenuItem
            {
                Id = Guid.NewGuid(),
                ParentId = purchasesParent.Id,
                Title = "Metal Purchase Hedging",
                Path = "/purchases/metal-purchase-hedging",
                Icon = "ShieldCheck",
                DisplayOrder = 7,
                IsActive = true,
                CreatedAtUtc = utcNow
            };

            var menuItems = new List<MenuItem>
            {
                dashboard,
                tenantMgmt,
                reportsParent,
                stockBalance,
                netGoldPosition,
                trialBalance,
                salesParent,
                invoices,
                creditNotes,
                metalSalesJewellery,
                metalSalesBullion,
                metalPayment,
                metalSalesFixing,
                metalSalesHedging,
                purchasesParent,
                bills,
                debitNotes,
                metalPurchaseJewellery,
                metalPurchaseBullion,
                metalReceipts,
                metalPurchaseFixing,
                metalPurchaseHedging
            };

            await context.MenuItems.AddRangeAsync(menuItems);

            var roleMenuItems = new List<RoleMenuItem>
            {
                // SuperAdmin Permissions
                new RoleMenuItem { MenuItemId = dashboard.Id, RoleName = "SuperAdmin" },
                new RoleMenuItem { MenuItemId = tenantMgmt.Id, RoleName = "SuperAdmin" },

                // TenantAdmin Core Permissions
                new RoleMenuItem { MenuItemId = dashboard.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = reportsParent.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = stockBalance.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = netGoldPosition.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = trialBalance.Id, RoleName = "TenantAdmin" },

                // TenantAdmin Sales
                new RoleMenuItem { MenuItemId = salesParent.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = invoices.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = creditNotes.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalSalesJewellery.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalSalesBullion.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalPayment.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalSalesFixing.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalSalesHedging.Id, RoleName = "TenantAdmin" },

                // TenantAdmin Purchases
                new RoleMenuItem { MenuItemId = purchasesParent.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = bills.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = debitNotes.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalPurchaseJewellery.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalPurchaseBullion.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalReceipts.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalPurchaseFixing.Id, RoleName = "TenantAdmin" },
                new RoleMenuItem { MenuItemId = metalPurchaseHedging.Id, RoleName = "TenantAdmin" },

                // User Permissions
                new RoleMenuItem { MenuItemId = dashboard.Id, RoleName = "User" }
            };

            await context.RoleMenuItems.AddRangeAsync(roleMenuItems);
            await context.SaveChangesAsync();
        }
    }
}