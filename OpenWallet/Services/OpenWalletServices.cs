using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;

namespace OpenWallet.Services;

public static class OpenWalletConstants
{
    public static readonly string[] Roles =
    [
        "SuperAdmin", "SystemAdmin", "OrganizationAdmin", "OrganizationManager", "FinanceManager",
        "ComplianceOfficer", "SupportAgent", "Auditor", "User"
    ];

    public static readonly string[] Permissions =
    [
        "Organizations.View", "Organizations.Create", "Organizations.Edit", "Organizations.Delete", "Organizations.Suspend", "Organizations.KycReview",
        "Users.View", "Users.Create", "Users.Edit", "Users.Delete", "Users.Invite", "Users.Block", "Users.ResetTwoFactor",
        "Roles.View", "Roles.Create", "Roles.Edit", "Roles.Delete", "Permissions.Manage",
        "Wallets.View", "Wallets.Create", "Wallets.Freeze", "Wallets.Unfreeze", "Wallets.AdjustBalance", "Wallets.Statement",
        "Transactions.ViewAll", "Transactions.ViewOrganization", "Transactions.ViewOwn", "Transactions.Approve", "Transactions.Reject",
        "Transactions.Export", "Transactions.Refund", "Transactions.Reverse", "Transactions.FlagSuspicious",
        "Topup.Create", "Topup.Approve", "Topup.Reject", "Transfer.Internal", "Transfer.External", "Transfer.ApproveHighValue",
        "Beneficiaries.View", "Beneficiaries.Create", "Beneficiaries.Edit", "Beneficiaries.Delete",
        "Settings.View", "Settings.Edit", "Settings.Fees", "Settings.Security", "Settings.Notifications",
        "Notifications.View", "Notifications.ManageTemplates", "AuditLogs.View", "Reports.View",
        "Complaints.View", "Complaints.Manage", "Compliance.View", "Compliance.Manage", "WebhookLogs.View", "Devices.View", "Devices.Manage"
    ];
}

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public sealed class PermissionAuthorizationHandler(
    UserManager<UserCustom> userManager,
    UserContext dbContext) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.IsInRole("SuperAdmin"))
        {
            context.Succeed(requirement);
            return;
        }

        var user = await userManager.GetUserAsync(context.User);
        if (user is null)
        {
            return;
        }

        var roleNames = await userManager.GetRolesAsync(user);
        var hasRolePermission = await dbContext.RolePermissions
            .Include(rp => rp.Permission)
            .AnyAsync(rp => rp.Permission != null && rp.Permission.Name == requirement.Permission && roleNames.Contains(rp.Role!.Name!));

        var hasUserPermission = await dbContext.UserPermissions
            .Include(up => up.Permission)
            .AnyAsync(up => up.UserId == user.Id && up.Permission != null && up.Permission.Name == requirement.Permission);

        if (hasRolePermission || hasUserPermission)
        {
            context.Succeed(requirement);
        }
    }
}

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string entityId, Guid? organizationId = null, string userId = "");
}

public class AuditService(UserContext dbContext, IHttpContextAccessor httpContextAccessor) : IAuditService
{
    public async Task LogAsync(string action, string entityName, string entityId, Guid? organizationId = null, string userId = "")
    {
        var http = httpContextAccessor.HttpContext;
        dbContext.AuditLogs.Add(new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OrganizationId = organizationId,
            UserId = userId,
            IpAddress = http?.Connection.RemoteIpAddress?.ToString() ?? "",
            UserAgent = http?.Request.Headers.UserAgent.ToString() ?? ""
        });
        await dbContext.SaveChangesAsync();
    }
}

public interface INotificationService
{
    Task SendAsync(Notification request);
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSmsAsync(string mobile, string message);
    Task SendPushAsync(string userId, string title, string body);
    Task CreateInAppAsync(string userId, string title, string body, string type);
}

public class NotificationService(UserContext dbContext, IEmailSender emailSender) : INotificationService
{
    public async Task SendAsync(Notification request)
    {
        dbContext.Notifications.Add(request);
        await dbContext.SaveChangesAsync();
    }

    public Task SendEmailAsync(string to, string subject, string body) => emailSender.SendEmailAsync(to, subject, body);
    public Task SendSmsAsync(string mobile, string message) => Task.CompletedTask;
    public Task SendPushAsync(string userId, string title, string body) => Task.CompletedTask;
    public Task CreateInAppAsync(string userId, string title, string body, string type) => SendAsync(new Notification { UserId = userId, Title = title, Body = body, Type = type });
}

public interface IWalletService
{
    string GenerateWalletNumber();
    Task<Wallet> EnsureWalletAsync(UserCustom user);
}

public class WalletService(UserContext dbContext, IAuditService auditService) : IWalletService
{
    public string GenerateWalletNumber() => $"OWKSA{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(100000, 999999)}";

    public async Task<Wallet> EnsureWalletAsync(UserCustom user)
    {
        var wallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
        if (wallet is not null)
        {
            return wallet;
        }

        wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            OrganizationId = user.OrganizationId,
            UserId = user.Id,
            WalletNumber = GenerateWalletNumber(),
            WalletType = "Personal",
            AvailableBalance = 2500,
            CreatedBy = user.Id
        };
        dbContext.Wallets.Add(wallet);
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("WalletCreated", nameof(Wallet), wallet.Id.ToString(), user.OrganizationId, user.Id);
        return wallet;
    }
}

public class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage) => Task.CompletedTask;
}

public static class OpenWalletSeeder
{
    private static readonly Guid MainOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserCustom>>();

        await db.Database.MigrateAsync();

        foreach (var roleName in OpenWalletConstants.Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        if (!await db.Organizations.AnyAsync())
        {
            db.Organizations.Add(new Organization
            {
                Id = MainOrganizationId,
                LegalNameEn = "OpenWallet Main Organization",
                LegalNameAr = "OpenWallet Main Organization",
                ShortName = "OpenWallet",
                OrganizationType = "Company",
                OfficialEmail = "admin@openwallet.local",
                OfficialPhone = "+966500000000",
                NationalAddressCity = "Riyadh",
                NationalAddressPostalCode = "11564",
                KycStatus = "Verified",
                ComplianceStatus = "Normal",
                IsMainOrganization = true,
                IsActive = true
            });
        }

        foreach (var permissionName in OpenWalletConstants.Permissions)
        {
            if (!await db.Permissions.AnyAsync(p => p.Name == permissionName))
            {
                db.Permissions.Add(new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permissionName,
                    GroupName = permissionName.Split('.')[0],
                    Description = permissionName
                });
            }
        }

        foreach (var category in new[] { "Personal", "Business", "Salary", "SupplierPayment", "Refund", "BillPayment", "Transfer", "TopUp", "Other" })
        {
            if (!await db.TransactionCategories.AnyAsync(c => c.Name == category))
            {
                db.TransactionCategories.Add(new TransactionCategory { Id = Guid.NewGuid(), Name = category });
            }
        }

        await db.SaveChangesAsync();

        var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
        if (superAdminRole is not null)
        {
            var permissionIds = await db.Permissions.Select(p => p.Id).ToListAsync();
            var existing = await db.RolePermissions.Where(rp => rp.RoleId == superAdminRole.Id).Select(rp => rp.PermissionId).ToListAsync();
            db.RolePermissions.AddRange(permissionIds.Except(existing).Select(id => new RolePermission { Id = Guid.NewGuid(), RoleId = superAdminRole.Id, PermissionId = id }));
            await db.SaveChangesAsync();
        }

        var admin = await userManager.FindByEmailAsync("admin@openwallet.local");
        if (admin is null)
        {
            admin = new UserCustom
            {
                UserName = "admin@openwallet.local",
                Email = "admin@openwallet.local",
                EmailConfirmed = true,
                PhoneNumber = "+966500000000",
                OrganizationId = MainOrganizationId,
                FirstNameEn = "OpenWallet",
                LastNameEn = "Admin",
                MobileNumber = "+966500000000",
                NationalIdOrIqama = "1000000000",
                NationalAddressCity = "Riyadh",
                NationalAddressPostalCode = "11564",
                KycStatus = "Verified",
                IsEmailVerified = true,
                IsMobileVerified = true
            };

            await userManager.CreateAsync(admin, "Admin@123456");
            await userManager.AddToRoleAsync(admin, "SuperAdmin");
        }

        if (!await db.Wallets.AnyAsync())
        {
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                OrganizationId = MainOrganizationId,
                UserId = admin.Id,
                WalletNumber = $"OWKSA{DateTime.UtcNow:yyyyMMdd}123456",
                WalletType = "Personal",
                AvailableBalance = 84250,
                CreatedBy = admin.Id
            };
            db.Wallets.Add(wallet);
            db.WalletTransactions.AddRange(
                new WalletTransaction { Id = Guid.NewGuid(), TransactionNumber = $"TXN-{DateTime.UtcNow:yyyyMMdd}-090000-101", OrganizationId = MainOrganizationId, ToWalletId = wallet.Id, TransactionType = "TopUp", Direction = "Credit", Amount = 50000, TotalAmount = 50000, Status = "Completed", PaymentMethod = "Card", Category = "TopUp", CreatedByUserId = admin.Id },
                new WalletTransaction { Id = Guid.NewGuid(), TransactionNumber = $"TXN-{DateTime.UtcNow:yyyyMMdd}-101500-202", OrganizationId = MainOrganizationId, FromWalletId = wallet.Id, TransactionType = "ExternalBankTransfer", Direction = "Debit", Amount = 7400, FeeAmount = 12, VatAmount = 1.8m, TotalAmount = 7413.8m, Status = "Pending", PaymentMethod = "BankTransfer", Category = "SupplierPayment", RequiresApproval = true, CreatedByUserId = admin.Id },
                new WalletTransaction { Id = Guid.NewGuid(), TransactionNumber = $"TXN-{DateTime.UtcNow:yyyyMMdd}-121500-303", OrganizationId = MainOrganizationId, FromWalletId = wallet.Id, TransactionType = "InternalTransfer", Direction = "Debit", Amount = 1250, FeeAmount = 0, VatAmount = 0, TotalAmount = 1250, Status = "Completed", PaymentMethod = "Wallet", Category = "Business", CreatedByUserId = admin.Id });
            await db.SaveChangesAsync();
        }
    }
}
