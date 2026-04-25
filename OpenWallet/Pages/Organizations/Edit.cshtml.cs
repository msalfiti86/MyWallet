using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Organizations;

public class EditModel(UserContext dbContext, ILookupService lookups, IAuditService auditService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public List<SelectListItem> OrganizationTypes { get; set; } = [];
    public List<SelectListItem> KycStatuses { get; set; } = [];
    public List<SelectListItem> ComplianceStatuses { get; set; } = [];
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        await LoadLookups();
        if (id is null) return Page();
        var org = await dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        if (org is null) return NotFound();
        Input = new InputModel
        {
            Id = org.Id, LegalNameEn = org.LegalNameEn, LegalNameAr = org.LegalNameAr, ShortName = org.ShortName,
            OrganizationType = org.OrganizationType, CommercialRegistrationNumber = org.CommercialRegistrationNumber,
            KycStatus = org.KycStatus, ComplianceStatus = org.ComplianceStatus, OfficialEmail = org.OfficialEmail,
            OfficialPhone = org.OfficialPhone, ManagerFullName = org.ManagerFullName, ManagerEmail = org.ManagerEmail,
            NationalAddressCity = org.NationalAddressCity, NationalAddressPostalCode = org.NationalAddressPostalCode
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadLookups();
        if (!ModelState.IsValid) return Page();
        var user = User.Identity?.Name ?? "System";
        var org = await dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == Input.Id);
        if (org is null)
        {
            org = new Organization { Id = Guid.NewGuid(), CreatedBy = user, IsActive = true };
            dbContext.Organizations.Add(org);
        }
        else
        {
            org.UpdatedAt = DateTime.UtcNow;
            org.UpdatedBy = user;
        }
        org.LegalNameEn = Input.LegalNameEn;
        org.LegalNameAr = Input.LegalNameAr;
        org.ShortName = Input.ShortName;
        org.OrganizationType = Input.OrganizationType;
        org.CommercialRegistrationNumber = Input.CommercialRegistrationNumber;
        org.KycStatus = Input.KycStatus;
        org.ComplianceStatus = Input.ComplianceStatus;
        org.OfficialEmail = Input.OfficialEmail;
        org.OfficialPhone = Input.OfficialPhone;
        org.ManagerFullName = Input.ManagerFullName;
        org.ManagerEmail = Input.ManagerEmail;
        org.NationalAddressCity = Input.NationalAddressCity;
        org.NationalAddressPostalCode = Input.NationalAddressPostalCode;
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("OrganizationSaved", nameof(Organization), org.Id.ToString(), org.Id, user);
        return RedirectToPage("Index");
    }

    private async Task LoadLookups()
    {
        var culture = Ar ? "ar-SA" : "en-US";
        OrganizationTypes = (await lookups.OptionsAsync("OrganizationType", culture)).Select(o => new SelectListItem(o.Text, o.Code)).ToList();
        KycStatuses = (await lookups.OptionsAsync("KycStatus", culture)).Select(o => new SelectListItem(o.Text, o.Code)).ToList();
        ComplianceStatuses = (await lookups.OptionsAsync("ComplianceStatus", culture)).Select(o => new SelectListItem(o.Text, o.Code)).ToList();
    }

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required] public string LegalNameEn { get; set; } = string.Empty;
        [Required] public string LegalNameAr { get; set; } = string.Empty;
        [Required] public string ShortName { get; set; } = string.Empty;
        [Required] public string OrganizationType { get; set; } = "Company";
        public string CommercialRegistrationNumber { get; set; } = string.Empty;
        public string KycStatus { get; set; } = "Pending";
        public string ComplianceStatus { get; set; } = "Normal";
        public string OfficialEmail { get; set; } = string.Empty;
        public string OfficialPhone { get; set; } = string.Empty;
        public string ManagerFullName { get; set; } = string.Empty;
        public string ManagerEmail { get; set; } = string.Empty;
        public string NationalAddressCity { get; set; } = string.Empty;
        public string NationalAddressPostalCode { get; set; } = string.Empty;
    }
}
