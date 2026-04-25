using Microsoft.AspNetCore.Identity;

namespace OpenWallet.Areas.Identity.Data;

public class UserCustom : IdentityUser
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public string FirstNameEn { get; set; } = string.Empty;
    public string FatherNameEn { get; set; } = string.Empty;
    public string GrandFatherNameEn { get; set; } = string.Empty;
    public string LastNameEn { get; set; } = string.Empty;
    public string FirstNameAr { get; set; } = string.Empty;
    public string FatherNameAr { get; set; } = string.Empty;
    public string GrandFatherNameAr { get; set; } = string.Empty;
    public string LastNameAr { get; set; } = string.Empty;
    public string Nationality { get; set; } = "Saudi Arabia";
    public string NationalIdOrIqama { get; set; } = string.Empty;
    public string IdType { get; set; } = "SaudiNationalId";
    public DateTime? IdExpiryDateGregorian { get; set; }
    public string IdExpiryDateHijri { get; set; } = string.Empty;
    public DateTime? DateOfBirthGregorian { get; set; }
    public string DateOfBirthHijri { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public bool IsMobileVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string NationalAddressBuildingNumber { get; set; } = string.Empty;
    public string NationalAddressStreet { get; set; } = string.Empty;
    public string NationalAddressDistrict { get; set; } = string.Empty;
    public string NationalAddressCity { get; set; } = string.Empty;
    public string NationalAddressPostalCode { get; set; } = string.Empty;
    public string NationalAddressAdditionalNumber { get; set; } = string.Empty;
    public string NationalAddressUnitNumber { get; set; } = string.Empty;
    public string KycStatus { get; set; } = "Pending";
    public DateTime? KycVerifiedAt { get; set; }
    public string KycRejectionReason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsBlocked { get; set; }
    public string BlockReason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
