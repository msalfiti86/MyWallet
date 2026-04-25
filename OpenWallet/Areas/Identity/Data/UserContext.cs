using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OpenWallet.Areas.Identity.Data;

public class UserContext : IdentityDbContext<UserCustom, IdentityRole, string>
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<LoginAudit> LoginAudits => Set<LoginAudit>();
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<TransactionApproval> TransactionApprovals => Set<TransactionApproval>();
    public DbSet<TransactionLimitRule> TransactionLimitRules => Set<TransactionLimitRule>();
    public DbSet<OrganizationDocument> OrganizationDocuments => Set<OrganizationDocument>();
    public DbSet<UserKycDocument> UserKycDocuments => Set<UserKycDocument>();
    public DbSet<WebhookEventLog> WebhookEventLogs => Set<WebhookEventLog>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<ComplaintMessage> ComplaintMessages => Set<ComplaintMessage>();
    public DbSet<RefundRequest> RefundRequests => Set<RefundRequest>();
    public DbSet<ComplianceReviewItem> ComplianceReviewItems => Set<ComplianceReviewItem>();
    public DbSet<TransactionCategory> TransactionCategories => Set<TransactionCategory>();
    public DbSet<Lookup> Lookups => Set<Lookup>();
    public DbSet<LookupDetail> LookupDetails => Set<LookupDetail>();
    public DbSet<VirtualAccount> VirtualAccounts => Set<VirtualAccount>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserCustom>()
            .HasQueryFilter(u => !u.IsDeleted);

        builder.Entity<UserCustom>()
            .HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Wallet>()
            .HasQueryFilter(w => !w.IsDeleted);

        builder.Entity<Organization>()
            .HasQueryFilter(o => !o.IsDeleted);

        builder.Entity<Beneficiary>()
            .HasQueryFilter(b => !b.IsDeleted);

        builder.Entity<WalletTransaction>()
            .HasQueryFilter(t => !t.IsDeleted);

        builder.Entity<Lookup>()
            .HasQueryFilter(l => !l.IsDeleted);

        builder.Entity<LookupDetail>()
            .HasQueryFilter(d => !d.IsDeleted);

        builder.Entity<VirtualAccount>()
            .HasQueryFilter(v => !v.IsDeleted);

        builder.Entity<Lookup>()
            .HasIndex(l => l.Name)
            .IsUnique();

        builder.Entity<LookupDetail>()
            .HasIndex(d => new { d.LookupId, d.Code })
            .IsUnique();

        builder.Entity<VirtualAccount>()
            .HasIndex(v => v.Iban)
            .IsUnique();

        builder.Entity<VirtualAccount>()
            .HasIndex(v => v.AccountNumber)
            .IsUnique();

        builder.Entity<LookupDetail>()
            .HasOne(d => d.Lookup)
            .WithMany(l => l.Details)
            .HasForeignKey(d => d.LookupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Wallet>()
            .HasIndex(w => w.WalletNumber)
            .IsUnique();

        builder.Entity<Wallet>()
            .Property(w => w.RowVersion)
            .IsRowVersion();

        builder.Entity<Wallet>()
            .HasOne(w => w.Organization)
            .WithMany(o => o.Wallets)
            .HasForeignKey(w => w.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WalletTransaction>()
            .HasIndex(t => t.TransactionNumber)
            .IsUnique();

        builder.Entity<SystemSetting>()
            .HasIndex(s => new { s.GroupName, s.Key })
            .IsUnique();

        builder.Entity<Permission>()
            .HasIndex(p => p.Name)
            .IsUnique();

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }
    }
}
