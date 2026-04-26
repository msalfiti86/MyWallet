using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace OpenWallet.Services;

public static class OpenWalletConstants
{
    public const string OtpBypassCode = "00000";

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

public record LookupOption(string Code, string Text);

public interface ILookupService
{
    Task<IReadOnlyList<LookupOption>> OptionsAsync(string lookupName, string cultureName);
    Task<string> TextAsync(string lookupName, string code, string cultureName);
}

public class LookupService(UserContext dbContext) : ILookupService
{
    public async Task<IReadOnlyList<LookupOption>> OptionsAsync(string lookupName, string cultureName)
    {
        var isArabic = cultureName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        return await dbContext.LookupDetails
            .Include(d => d.Lookup)
            .Where(d => d.Lookup != null && d.Lookup.Name == lookupName && d.IsActive && d.Lookup.IsActive)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Code)
            .Select(d => new LookupOption(d.Code, isArabic ? d.ValueAr : d.ValueEn))
            .ToListAsync();
    }

    public async Task<string> TextAsync(string lookupName, string code, string cultureName)
    {
        var isArabic = cultureName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        var detail = await dbContext.LookupDetails
            .Include(d => d.Lookup)
            .FirstOrDefaultAsync(d => d.Lookup != null && d.Lookup.Name == lookupName && d.Code == code);
        return detail is null ? code : isArabic ? detail.ValueAr : detail.ValueEn;
    }
}

public interface IOtpService
{
    bool Validate(string code);
    Task<OtpSendResult> SendAsync(UserCustom user, string purpose, string cultureName);
    Task<bool> ValidateAsync(string userId, string purpose, string code);
    string MaskEmail(string email);
}

public record OtpSendResult(bool Sent, string MaskedDestination, DateTime ExpiresAt);

public class OtpService(UserContext dbContext, IEmailSender emailSender, IConfiguration configuration) : IOtpService
{
    public bool Validate(string code) => code is OpenWalletConstants.OtpBypassCode or "000000";

    public async Task<OtpSendResult> SendAsync(UserCustom user, string purpose, string cultureName)
    {
        var email = user.Email ?? "";
        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var expiryMinutes = configuration.GetValue("Security:OtpExpiryMinutes", 5);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        dbContext.OtpCodes.Add(new OtpCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Purpose = purpose,
            CodeHash = Hash(code),
            ExpiresAt = expiresAt
        });
        await dbContext.SaveChangesAsync();

        var isArabic = cultureName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        var subject = isArabic ? "رمز التحقق من OpenWallet" : "OpenWallet verification code";
        var body = isArabic
            ? $"<p>رمز التحقق لإكمال العملية هو <strong>{code}</strong>.</p><p>ينتهي الرمز خلال {expiryMinutes} دقائق.</p>"
            : $"<p>Your verification code to continue the current action is <strong>{code}</strong>.</p><p>This code expires in {expiryMinutes} minutes.</p>";

        if (!string.IsNullOrWhiteSpace(email))
        {
            await emailSender.SendEmailAsync(email, subject, body);
        }

        return new OtpSendResult(true, MaskEmail(email), expiresAt);
    }

    public async Task<bool> ValidateAsync(string userId, string purpose, string code)
    {
        code = code.Trim();
        if (Validate(code))
        {
            return true;
        }

        var hash = Hash(code);
        var otp = await dbContext.OtpCodes
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed && o.ExpiresAt >= DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null || otp.CodeHash != hash)
        {
            return false;
        }

        otp.IsUsed = true;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return "m***@e***.com";
        }

        var parts = email.Split('@', 2);
        var name = parts[0];
        var domainParts = parts[1].Split('.');
        var maskedName = name.Length == 0 ? "m***" : name.Length <= 2 ? $"{name[0]}***" : $"{name[0]}{name[1]}***";
        var maskedDomain = string.Join(".", domainParts.Select(p => p.Length == 0 ? "" : $"{p[0]}**"));
        return $"{maskedName}@{maskedDomain}";
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}

public interface ISettingsService
{
    Task<decimal> GetDecimalAsync(string groupName, string key, decimal defaultValue);
    Task SetAsync(string groupName, string key, string value, string description, string userId);
}

public class SettingsService(UserContext dbContext) : ISettingsService
{
    public async Task<decimal> GetDecimalAsync(string groupName, string key, decimal defaultValue)
    {
        var value = await dbContext.SystemSettings
            .Where(s => s.GroupName == groupName && s.Key == key)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        return decimal.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    public async Task SetAsync(string groupName, string key, string value, string description, string userId)
    {
        var setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.GroupName == groupName && s.Key == key);
        if (setting is null)
        {
            dbContext.SystemSettings.Add(new SystemSetting
            {
                Id = Guid.NewGuid(),
                GroupName = groupName,
                Key = key,
                Value = value,
                Description = description,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            setting.Value = value;
            setting.Description = description;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();
    }
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

        var virtualAccount = await dbContext.VirtualAccounts
            .Where(v => v.IsActive && !v.IsUsed)
            .OrderBy(v => v.CreatedAt)
            .FirstOrDefaultAsync();

        if (virtualAccount is null)
        {
            virtualAccount = new VirtualAccount
            {
                Id = Guid.NewGuid(),
                OrganizationId = user.OrganizationId,
                Iban = VirtualAccountGenerator.GenerateIban(),
                AccountNumber = VirtualAccountGenerator.GenerateAccountNumber(),
                BankName = "BSF Bank",
                CreatedBy = "System"
            };
            dbContext.VirtualAccounts.Add(virtualAccount);
        }

        wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            OrganizationId = user.OrganizationId,
            UserId = user.Id,
            WalletNumber = GenerateWalletNumber(),
            AccountNumber = virtualAccount.AccountNumber,
            VirtualIban = virtualAccount.Iban,
            BankName = virtualAccount.BankName,
            WalletType = "Personal",
            AvailableBalance = 2500,
            CreatedBy = user.Id
        };
        virtualAccount.IsUsed = true;
        virtualAccount.AssignedToUserId = user.Id;
        virtualAccount.AssignedWalletId = wallet.Id;
        virtualAccount.AssignedAt = DateTime.UtcNow;
        dbContext.Wallets.Add(wallet);
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("WalletCreated", nameof(Wallet), wallet.Id.ToString(), user.OrganizationId, user.Id);
        return wallet;
    }
}

public static class VirtualAccountGenerator
{
    public static string GenerateAccountNumber() => Random.Shared.NextInt64(1000000000000000, 9999999999999999).ToString();

    public static string GenerateIban()
    {
        var account = GenerateAccountNumber().PadLeft(18, '0');
        return $"SA{Random.Shared.Next(10, 99)}55{account}";
    }
}

public class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = configuration["EmailSettings:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            return;
        }

        var port = configuration.GetValue("EmailSettings:Port", 587);
        var senderEmail = configuration["EmailSettings:SenderEmail"] ?? "no-reply@openwallet.local";
        var senderName = configuration["EmailSettings:SenderName"] ?? "OpenWallet KSA";
        var username = configuration["EmailSettings:Username"];
        var password = configuration["EmailSettings:Password"];
        var enableSsl = configuration.GetValue("EmailSettings:EnableSsl", true);

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        message.To.Add(email);

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl
        };
        if (!string.IsNullOrWhiteSpace(username))
        {
            client.Credentials = new NetworkCredential(username, password);
        }

        await client.SendMailAsync(message);
    }
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

        foreach (var setting in new[]
        {
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "WalletLimits", Key = "MinimumTopUpAmount", Value = "50", Description = "Minimum wallet top-up amount" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "WalletLimits", Key = "MaximumTopUpAmount", Value = "100000", Description = "Maximum wallet top-up amount" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "TransferFees", Key = "ExternalBankTransferFeeFixed", Value = "12", Description = "Fixed fee for external beneficiary bank transfers" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "TransferFees", Key = "OpenWalletTransferFeeFixed", Value = "1", Description = "Fixed fee for transfers to OpenWallet wallet accounts" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "TransferFees", Key = "InternalOrganizationTransferFeeFixed", Value = "0", Description = "Fixed fee for transfers inside the same organization" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "TransferFees", Key = "VatPercentage", Value = "15", Description = "VAT percentage applied to transfer fees" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "WalletLimits", Key = "MinimumTransferAmount", Value = "1", Description = "Minimum transfer amount" },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "WalletLimits", Key = "MaximumTransferAmount", Value = "100000", Description = "Maximum transfer amount" }
        })
        {
            if (!await db.SystemSettings.AnyAsync(s => s.GroupName == setting.GroupName && s.Key == setting.Key))
            {
                db.SystemSettings.Add(setting);
            }
        }

        await SeedLookupsAsync(db, MainOrganizationId);

        if (await db.VirtualAccounts.CountAsync() < 100)
        {
            var existing = await db.VirtualAccounts.Select(v => v.Iban).ToListAsync();
            var needed = 100 - existing.Count;
            for (var i = 0; i < needed; i++)
            {
                string iban;
                do { iban = VirtualAccountGenerator.GenerateIban(); } while (existing.Contains(iban));
                existing.Add(iban);
                db.VirtualAccounts.Add(new VirtualAccount
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = MainOrganizationId,
                    Iban = iban,
                    AccountNumber = VirtualAccountGenerator.GenerateAccountNumber(),
                    BankName = "BSF Bank",
                    CreatedBy = "Seeder"
                });
            }
            await db.SaveChangesAsync();
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
                AccountNumber = "5555000012345678",
                VirtualIban = "SA44550000000012345678",
                BankName = "BSF Bank",
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

    private static async Task SeedLookupsAsync(UserContext db, Guid organizationId)
    {
        var lookups = new Dictionary<string, (string En, string Ar)[]>
        {
            ["OrganizationType"] =
            [
                ("Company", "شركة"), ("Establishment", "مؤسسة"), ("Government", "حكومي"), ("NonProfit", "غير ربحي"),
                ("Waqf", "وقف"), ("IndividualMerchant", "تاجر فردي")
            ],
            ["KycStatus"] = [("Pending", "قيد الانتظار"), ("Verified", "موثق"), ("Rejected", "مرفوض"), ("Expired", "منتهي")],
            ["ComplianceStatus"] = [("Normal", "طبيعي"), ("ReviewRequired", "يتطلب مراجعة"), ("Suspended", "موقوف")],
            ["WalletType"] = [("Personal", "شخصية"), ("Organization", "منظمة"), ("Merchant", "تاجر")],
            ["WalletStatus"] = [("Active", "نشطة"), ("Frozen", "مجمدة"), ("Closed", "مغلقة"), ("Suspended", "موقوفة")],
            ["TransactionType"] = [("TopUp", "شحن"), ("InternalTransfer", "تحويل داخلي"), ("ExternalBankTransfer", "تحويل بنكي خارجي"), ("Refund", "استرداد"), ("Reversal", "عكس قيد"), ("Fee", "رسوم"), ("Adjustment", "تسوية")],
            ["TransactionDirection"] = [("Credit", "دائن"), ("Debit", "مدين")],
            ["TransactionStatus"] = [("Pending", "قيد الانتظار"), ("Processing", "قيد المعالجة"), ("Completed", "مكتملة"), ("Failed", "فشلت"), ("Cancelled", "ملغاة"), ("Rejected", "مرفوضة"), ("Reversed", "معكوسة"), ("Refunded", "مستردة")],
            ["PaymentMethod"] = [("Card", "بطاقة"), ("PayPal", "باي بال"), ("BankAccount", "حساب بنكي"), ("Wallet", "محفظة"), ("BankTransfer", "تحويل بنكي")],
            ["TransactionCategory"] = [("Personal", "شخصي"), ("Business", "أعمال"), ("Salary", "راتب"), ("SupplierPayment", "دفع مورد"), ("Refund", "استرداد"), ("BillPayment", "سداد فاتورة"), ("Transfer", "تحويل"), ("TopUp", "شحن"), ("Other", "أخرى")],
            ["BeneficiaryType"] = [("InternalWallet", "محفظة داخلية"), ("LocalBank", "بنك محلي"), ("InternationalBank", "بنك دولي")],
            ["ComplaintStatus"] = [("Open", "مفتوحة"), ("InProgress", "قيد المعالجة"), ("WaitingForCustomer", "بانتظار العميل"), ("Resolved", "محلولة"), ("Closed", "مغلقة"), ("Rejected", "مرفوضة")],
            ["ComplaintCategory"] = [("TopUpIssue", "مشكلة شحن"), ("TransferIssue", "مشكلة تحويل"), ("AccountIssue", "مشكلة حساب"), ("BeneficiaryIssue", "مشكلة مستفيد"), ("SecurityIssue", "مشكلة أمنية"), ("Other", "أخرى")],
            ["Priority"] = [("Low", "منخفضة"), ("Medium", "متوسطة"), ("High", "عالية"), ("Critical", "حرجة")],
            ["RiskLevel"] = [("Low", "منخفض"), ("Medium", "متوسط"), ("High", "عال"), ("Critical", "حرج")],
            ["IdType"] = [("SaudiNationalId", "هوية وطنية"), ("Iqama", "إقامة"), ("GCCId", "هوية خليجية"), ("Passport", "جواز سفر")]
        };

        foreach (var (name, details) in lookups)
        {
            var lookup = await db.Lookups.Include(l => l.Details).FirstOrDefaultAsync(l => l.Name == name);
            if (lookup is null)
            {
                lookup = new Lookup { Id = Guid.NewGuid(), Name = name, Description = name, OrganizationId = organizationId, IsSystem = true };
                db.Lookups.Add(lookup);
            }

            for (var i = 0; i < details.Length; i++)
            {
                var code = details[i].En;
                if (lookup.Details.All(d => d.Code != code))
                {
                    lookup.Details.Add(new LookupDetail
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organizationId,
                        Code = code,
                        ValueEn = details[i].En,
                        ValueAr = details[i].Ar,
                        SortOrder = i + 1
                    });
                }
            }
        }

        await db.SaveChangesAsync();
    }
}
