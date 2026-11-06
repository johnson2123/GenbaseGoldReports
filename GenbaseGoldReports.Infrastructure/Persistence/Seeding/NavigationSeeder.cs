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

            var now = DateTime.UtcNow;
            var menuItems = new List<MenuItem>();
            var roleMenuItems = new List<RoleMenuItem>();

            MenuItem CreateItem(string title, string? path, string icon, int order, Guid? parentId = null)
            {
                var item = new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Path = path,
                    Icon = icon,
                    DisplayOrder = order,
                    ParentId = parentId,
                    IsActive = true,
                    CreatedAtUtc = now
                };
                menuItems.Add(item);
                return item;
            }

            void AssignRoles(MenuItem item, params string[] roles)
            {
                foreach (var role in roles)
                {
                    roleMenuItems.Add(new RoleMenuItem { MenuItemId = item.Id, RoleName = role });
                }
            }

            // ==========================================
            // 1. SUPERADMIN PLATFORM PORTAL (ISOLATED)
            // ==========================================
            var saDashboard = CreateItem("PLATFORM DASHBOARD", "/admin/dashboard", "LayoutDashboard", 1);
            var saTenants = CreateItem("TENANT MANAGEMENT", "/admin/tenants", "Building2", 2);
            var saHealth = CreateItem("SYSTEM HEALTH", "/admin/system-health", "Activity", 3);
            var saAudit = CreateItem("PLATFORM AUDIT", "/admin/audit-logs", "ShieldAlert", 4);

            AssignRoles(saDashboard, "SuperAdmin");
            AssignRoles(saTenants, "SuperAdmin");
            AssignRoles(saHealth, "SuperAdmin");
            AssignRoles(saAudit, "SuperAdmin");

            // ==========================================
            // 2. TENANT ERP PORTAL (BUSINESS SUITE)
            // ==========================================

            // --- 1. DASHBOARD ---
            var dashboard = CreateItem("DASHBOARD", "/dashboard", "LayoutDashboard", 1);
            AssignRoles(dashboard, "TenantAdmin", "User");

            // --- 2. MASTERS (Root) ---
            var masters = CreateItem("MASTERS", null, "FolderKanban", 2);
            AssignRoles(masters, "TenantAdmin", "User");

            // 2.1 Account Masters
            var accMasters = CreateItem("Account Masters", null, "Users", 1, masters.Id);
            AssignRoles(accMasters, "TenantAdmin", "User");
            var mAccounts = CreateItem("Accounts Master", "/masters/accounts", "BookUser", 1, accMasters.Id);
            var mParties = CreateItem("Customer & Supplier Profiles", "/masters/parties", "Contact", 2, accMasters.Id);
            var mSalesmen = CreateItem("Salesmen", "/masters/salesmen", "BadgeCheck", 3, accMasters.Id);
            AssignRoles(mAccounts, "TenantAdmin", "User");
            AssignRoles(mParties, "TenantAdmin", "User");
            AssignRoles(mSalesmen, "TenantAdmin", "User");

            // 2.2 Stock Masters
            var stockMasters = CreateItem("Stock Masters", null, "Boxes", 2, masters.Id);
            AssignRoles(stockMasters, "TenantAdmin", "User");
            var mMetal = CreateItem("Stock Item Master", "/masters/metal-items", "Gem", 1, stockMasters.Id);
            var mBranches = CreateItem("Branch Master", "/masters/branches", "Network", 2, stockMasters.Id);
            var mDivisions = CreateItem("Division Master", "/masters/divisions", "Layers", 3, stockMasters.Id);
            var mRateTypes = CreateItem("Rate Type Master", "/masters/rate-types", "Sliders", 4, stockMasters.Id);
            AssignRoles(mMetal, "TenantAdmin", "User");
            AssignRoles(mBranches, "TenantAdmin", "User");
            AssignRoles(mDivisions, "TenantAdmin", "User");
            AssignRoles(mRateTypes, "TenantAdmin", "User");

            // --- 3. TRANSACTIONS (Root) ---
            var transactions = CreateItem("TRANSACTIONS", null, "ArrowLeftRight", 3);
            AssignRoles(transactions, "TenantAdmin", "User");

            // 3.1 Sales
            var salesGroup = CreateItem("Sales", null, "BadgePercent", 1, transactions.Id);
            AssignRoles(salesGroup, "TenantAdmin", "User");
            var sInv = CreateItem("Invoices", "/sales/invoices", "FileText", 1, salesGroup.Id);
            var sCn = CreateItem("Credit Notes", "/sales/credit-notes", "FileDiff", 2, salesGroup.Id);
            var sMsj = CreateItem("Metal Sales Jewellery", "/sales/metal-sales-jewellery", "Sparkles", 3, salesGroup.Id);
            var sMsb = CreateItem("Metal Sales Bullion", "/sales/metal-sales-bullion", "Coins", 4, salesGroup.Id);
            var sPay = CreateItem("Metal Payment", "/sales/metal-payment", "CreditCard", 5, salesGroup.Id);
            var sFix = CreateItem("Metal Sales Fixing", "/sales/metal-sales-fixing", "SlidersHorizontal", 6, salesGroup.Id);
            var sHdg = CreateItem("Metal Sales Hedging", "/sales/metal-sales-hedging", "ShieldAlert", 7, salesGroup.Id);
            AssignRoles(sInv, "TenantAdmin", "User");
            AssignRoles(sCn, "TenantAdmin", "User");
            AssignRoles(sMsj, "TenantAdmin", "User");
            AssignRoles(sMsb, "TenantAdmin", "User");
            AssignRoles(sPay, "TenantAdmin");
            AssignRoles(sFix, "TenantAdmin");
            AssignRoles(sHdg, "TenantAdmin");

            // 3.2 Purchases
            var purGroup = CreateItem("Purchases", null, "ShoppingBag", 2, transactions.Id);
            AssignRoles(purGroup, "TenantAdmin", "User");
            var pBills = CreateItem("Bills", "/purchases/bills", "Receipt", 1, purGroup.Id);
            var pDn = CreateItem("Debit Notes", "/purchases/debit-notes", "FileMinus", 2, purGroup.Id);
            var pMpj = CreateItem("Metal Purchase Jewellery", "/purchases/metal-purchase-jewellery", "Sparkles", 3, purGroup.Id);
            var pMpb = CreateItem("Metal Purchase Bullion", "/purchases/metal-purchase-bullion", "Boxes", 4, purGroup.Id);
            var pRec = CreateItem("Metal Receipts", "/purchases/metal-receipts", "ReceiptText", 5, purGroup.Id);
            var pFix = CreateItem("Metal Purchase Fixing", "/purchases/metal-purchase-fixing", "Pin", 6, purGroup.Id);
            var pHdg = CreateItem("Metal Purchase Hedging", "/purchases/metal-purchase-hedging", "ShieldCheck", 7, purGroup.Id);
            AssignRoles(pBills, "TenantAdmin", "User");
            AssignRoles(pDn, "TenantAdmin", "User");
            AssignRoles(pMpj, "TenantAdmin", "User");
            AssignRoles(pMpb, "TenantAdmin", "User");
            AssignRoles(pRec, "TenantAdmin", "User");
            AssignRoles(pFix, "TenantAdmin");
            AssignRoles(pHdg, "TenantAdmin");

            // --- 4. REPORTS (Root) ---
            var reports = CreateItem("REPORTS", null, "FileBarChart", 4);
            AssignRoles(reports, "TenantAdmin", "User");
            var rStock = CreateItem("Stock Balance", "/reports/stock-balance", "Boxes", 1, reports.Id);
            var rGold = CreateItem("Net Gold Position", "/reports/net-gold-position", "Scale", 2, reports.Id);
            var rTb = CreateItem("Trial Balance", "/reports/trial-balance", "ReceiptText", 3, reports.Id);
            AssignRoles(rStock, "TenantAdmin", "User");
            AssignRoles(rGold, "TenantAdmin");
            AssignRoles(rTb, "TenantAdmin");

            // --- 5. MAINTENANCE (Root - Business Level) ---
            var maintenance = CreateItem("MAINTENANCE", null, "Settings", 5);
            AssignRoles(maintenance, "TenantAdmin");
            var mUsers = CreateItem("User & Role Access", "/maintenance/users", "UserCheck", 1, maintenance.Id);
            var mSeq = CreateItem("Document Sequences", "/maintenance/document-series", "Hash", 2, maintenance.Id);
            var mFiscal = CreateItem("Fiscal Periods", "/maintenance/fiscal-periods", "CalendarLock", 3, maintenance.Id);
            var mAudit = CreateItem("System Audit Trail", "/maintenance/audit-trail", "FileSearch", 4, maintenance.Id);
            AssignRoles(mUsers, "TenantAdmin");
            AssignRoles(mSeq, "TenantAdmin");
            AssignRoles(mFiscal, "TenantAdmin");
            AssignRoles(mAudit, "TenantAdmin");

            // --- 6. OTHERS (Root) ---
            var others = CreateItem("OTHERS", null, "MoreHorizontal", 6);
            AssignRoles(others, "TenantAdmin", "User");
            var oRates = CreateItem("Daily Rates Entry", "/others/daily-rates", "LineChart", 1, others.Id);
            var oUtils = CreateItem("Data Utilities", "/others/data-utilities", "Binary", 2, others.Id);
            AssignRoles(oRates, "TenantAdmin", "User");
            AssignRoles(oUtils, "TenantAdmin");

            // ==========================================
            // COMMIT SEED
            // ==========================================
            await context.MenuItems.AddRangeAsync(menuItems);
            await context.RoleMenuItems.AddRangeAsync(roleMenuItems);
            await context.SaveChangesAsync();
        }
    }
}