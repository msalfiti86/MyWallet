using Microsoft.AspNetCore.Identity;

namespace OpenWallet.Areas.Identity.Data;

public class Organization
{
    public Guid Id { get; set; }
    public string LegalNameEn { get; set; } = string.Empty;
    public string LegalNameAr { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string OrganizationType { get; set; } = "Company";
    public string CommercialRegistrationNumber { get; set; } = string.Empty;
    public DateTime? CommercialRegistrationIssueDate { get; set; }
    public DateTime? CommercialRegistrationExpiryDate { get; set; }
    public string UnifiedNationalNumber700 { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;
    public string BusinessActivity { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string OfficialEmail { get; set; } = string.Empty;
    public string OfficialPhone { get; set; } = string.Empty;
    public string ManagerFullName { get; set; } = string.Empty;
    public string ManagerNationalIdOrIqama { get; set; } = string.Empty;
    public string ManagerMobile { get; set; } = string.Empty;
    public string ManagerEmail { get; set; } = string.Empty;
    public string NationalAddressBuildingNumber { get; set; } = string.Empty;
    public string NationalAddressStreet { get; set; } = string.Empty;
    public string NationalAddressDistrict { get; set; } = string.Empty;
    public string NationalAddressCity { get; set; } = string.Empty;
    public string NationalAddressPostalCode { get; set; } = string.Empty;
    public string NationalAddressAdditionalNumber { get; set; } = string.Empty;
    public string NationalAddressUnitNumber { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
    public string KycStatus { get; set; } = "Pending";
    public string ComplianceStatus { get; set; } = "Normal";
    public string RejectionReason { get; set; } = string.Empty;
    public decimal DailyTransferLimit { get; set; } = 50000;
    public decimal MonthlyTransferLimit { get; set; } = 500000;
    public decimal SingleTransactionLimit { get; set; } = 10000;
    public bool IsMainOrganization { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSuspended { get; set; }
    public string SuspensionReason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<UserCustom> Users { get; set; } = new List<UserCustom>();
    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}

public class Wallet
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public string? UserId { get; set; }
    public UserCustom? User { get; set; }
    public string WalletNumber { get; set; } = string.Empty;
    public string WalletType { get; set; } = "Personal";
    public string Currency { get; set; } = "SAR";
    public decimal AvailableBalance { get; set; }
    public decimal HoldBalance { get; set; }
    public string Status { get; set; } = "Active";
    public decimal DailyLimit { get; set; } = 25000;
    public decimal MonthlyLimit { get; set; } = 150000;
    public decimal SingleTransactionLimit { get; set; } = 10000;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}

public class WalletTransaction
{
    public Guid Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public Guid? FromWalletId { get; set; }
    public Wallet? FromWallet { get; set; }
    public Guid? ToWalletId { get; set; }
    public Wallet? ToWallet { get; set; }
    public string TransactionType { get; set; } = "TopUp";
    public string Direction { get; set; } = "Credit";
    public decimal Amount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "SAR";
    public string Status { get; set; } = "Pending";
    public string PaymentMethod { get; set; } = "Wallet";
    public string Category { get; set; } = "Other";
    public string ReferenceNumber { get; set; } = string.Empty;
    public string ExternalProviderReference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FailureReason { get; set; } = string.Empty;
    public bool IsSuspicious { get; set; }
    public string SuspiciousReason { get; set; } = string.Empty;
    public string FlaggedByUserId { get; set; } = string.Empty;
    public DateTime? FlaggedAt { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ApprovedByUserId { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
    public string RejectedByUserId { get; set; } = string.Empty;
    public DateTime? RejectedAt { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool RequiresApproval { get; set; }
    public Guid? OriginalTransactionId { get; set; }
}

public class Beneficiary
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
    public UserCustom? OwnerUser { get; set; }
    public string BeneficiaryType { get; set; } = "InternalWallet";
    public string Nickname { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WalletNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string NationalIdOrIqama { get; set; } = string.Empty;
    public string Country { get; set; } = "Saudi Arabia";
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
}

public class PaymentMethod { public Guid Id { get; set; } public Guid OrganizationId { get; set; } public string UserId { get; set; } = string.Empty; public string Type { get; set; } = "Card"; public string DisplayName { get; set; } = string.Empty; public string CardBrand { get; set; } = string.Empty; public string Last4 { get; set; } = string.Empty; public string CardExpiryMonth { get; set; } = string.Empty; public string CardExpiryYear { get; set; } = string.Empty; public string BankName { get; set; } = string.Empty; public string IbanMasked { get; set; } = string.Empty; public string ProviderToken { get; set; } = string.Empty; public bool IsDefault { get; set; } public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class Permission { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string GroupName { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; }
public class RolePermission { public Guid Id { get; set; } public string RoleId { get; set; } = string.Empty; public IdentityRole? Role { get; set; } public Guid PermissionId { get; set; } public Permission? Permission { get; set; } }
public class UserPermission { public Guid Id { get; set; } public string UserId { get; set; } = string.Empty; public Guid PermissionId { get; set; } public Permission? Permission { get; set; } }
public class UserInvitation { public Guid Id { get; set; } public Guid OrganizationId { get; set; } public string Email { get; set; } = string.Empty; public string MobileNumber { get; set; } = string.Empty; public string RoleName { get; set; } = "User"; public string InvitationToken { get; set; } = string.Empty; public string Status { get; set; } = "Pending"; public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7); public string InvitedByUserId { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class Notification { public Guid Id { get; set; } public string UserId { get; set; } = string.Empty; public Guid? OrganizationId { get; set; } public string Title { get; set; } = string.Empty; public string Body { get; set; } = string.Empty; public string Type { get; set; } = "Info"; public string Channel { get; set; } = "InApp"; public bool IsRead { get; set; } public DateTime? ReadAt { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public string RelatedEntityType { get; set; } = string.Empty; public string RelatedEntityId { get; set; } = string.Empty; }
public class NotificationTemplate { public Guid Id { get; set; } public string EventName { get; set; } = string.Empty; public string Channel { get; set; } = "InApp"; public string Subject { get; set; } = string.Empty; public string Body { get; set; } = string.Empty; public bool IsActive { get; set; } = true; }
public class SystemSetting { public Guid Id { get; set; } public string GroupName { get; set; } = string.Empty; public string Key { get; set; } = string.Empty; public string Value { get; set; } = string.Empty; public bool IsEncrypted { get; set; } public string Description { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? UpdatedAt { get; set; } }
public class AuditLog { public Guid Id { get; set; } public Guid? OrganizationId { get; set; } public string UserId { get; set; } = string.Empty; public string Action { get; set; } = string.Empty; public string EntityName { get; set; } = string.Empty; public string EntityId { get; set; } = string.Empty; public string OldValues { get; set; } = string.Empty; public string NewValues { get; set; } = string.Empty; public string IpAddress { get; set; } = string.Empty; public string UserAgent { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class LoginAudit { public Guid Id { get; set; } public string UserId { get; set; } = string.Empty; public Guid? OrganizationId { get; set; } public string EmailOrMobile { get; set; } = string.Empty; public bool IsSuccess { get; set; } public string FailureReason { get; set; } = string.Empty; public string IpAddress { get; set; } = string.Empty; public string UserAgent { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class DeviceSession { public Guid Id { get; set; } public string UserId { get; set; } = string.Empty; public Guid OrganizationId { get; set; } public string DeviceName { get; set; } = string.Empty; public string Browser { get; set; } = string.Empty; public string OperatingSystem { get; set; } = string.Empty; public string IpAddress { get; set; } = string.Empty; public string UserAgent { get; set; } = string.Empty; public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? LastSeenAt { get; set; } public DateTime? RevokedAt { get; set; } }
public class OtpCode { public Guid Id { get; set; } public string UserId { get; set; } = string.Empty; public string CodeHash { get; set; } = string.Empty; public string Purpose { get; set; } = string.Empty; public DateTime ExpiresAt { get; set; } public bool IsUsed { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class TransactionApproval { public Guid Id { get; set; } public Guid WalletTransactionId { get; set; } public string RequestedByUserId { get; set; } = string.Empty; public string ApprovedByUserId { get; set; } = string.Empty; public string Status { get; set; } = "Pending"; public string Note { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? ResolvedAt { get; set; } }
public class TransactionLimitRule { public Guid Id { get; set; } public Guid? OrganizationId { get; set; } public string WalletType { get; set; } = "Personal"; public decimal SingleLimit { get; set; } public decimal DailyLimit { get; set; } public decimal MonthlyLimit { get; set; } public decimal ApprovalThreshold { get; set; } }
public class OrganizationDocument { public Guid Id { get; set; } public Guid OrganizationId { get; set; } public string DocumentType { get; set; } = string.Empty; public string FileName { get; set; } = string.Empty; public string FilePath { get; set; } = string.Empty; public string Status { get; set; } = "Pending"; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class UserKycDocument { public Guid Id { get; set; } public string UserId { get; set; } = string.Empty; public Guid OrganizationId { get; set; } public string DocumentType { get; set; } = string.Empty; public string FileName { get; set; } = string.Empty; public string FilePath { get; set; } = string.Empty; public string Status { get; set; } = "Pending"; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class WebhookEventLog { public Guid Id { get; set; } public string ProviderName { get; set; } = string.Empty; public string EventType { get; set; } = string.Empty; public string Payload { get; set; } = string.Empty; public string Status { get; set; } = "Received"; public string ErrorMessage { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? ProcessedAt { get; set; } }
public class Complaint { public Guid Id { get; set; } public Guid OrganizationId { get; set; } public string UserId { get; set; } = string.Empty; public Guid? WalletTransactionId { get; set; } public string Title { get; set; } = string.Empty; public string Category { get; set; } = "Other"; public string Status { get; set; } = "Open"; public string Priority { get; set; } = "Medium"; public string AssignedToUserId { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? ClosedAt { get; set; } }
public class ComplaintMessage { public Guid Id { get; set; } public Guid ComplaintId { get; set; } public string UserId { get; set; } = string.Empty; public string Message { get; set; } = string.Empty; public bool IsInternal { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class RefundRequest { public Guid Id { get; set; } public Guid OriginalTransactionId { get; set; } public Guid OrganizationId { get; set; } public decimal Amount { get; set; } public string Reason { get; set; } = string.Empty; public string Status { get; set; } = "Pending"; public string RequestedByUserId { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
public class ComplianceReviewItem { public Guid Id { get; set; } public Guid? OrganizationId { get; set; } public string SourceType { get; set; } = string.Empty; public string SourceId { get; set; } = string.Empty; public string RiskLevel { get; set; } = "Medium"; public string Status { get; set; } = "Open"; public string AssignedToUserId { get; set; } = string.Empty; public string ResolutionNote { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime? ResolvedAt { get; set; } }
public class TransactionCategory { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public bool IsActive { get; set; } = true; }
