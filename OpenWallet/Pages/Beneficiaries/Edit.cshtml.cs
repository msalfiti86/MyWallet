using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Beneficiaries;

public class EditModel(UserContext dbContext, ILookupService lookups, IOtpService otpService, IAuditService auditService, UserManager<UserCustom> userManager) : PageModel
{
    private const string OtpPurpose = "Beneficiary.Save";
    [BindProperty] public InputModel Input { get; set; } = new();
    public List<SelectListItem> Types { get; set; } = [];
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        await LoadLookups();
        if (id is null) return Page();
        var item = await dbContext.Beneficiaries.FirstOrDefaultAsync(b => b.Id == id);
        if (item is null) return NotFound();
        Input = new InputModel { Id = item.Id, BeneficiaryType = item.BeneficiaryType, Nickname = item.Nickname, FullNameEn = item.FullNameEn, FullNameAr = item.FullNameAr, MobileNumber = item.MobileNumber, Email = item.Email, WalletNumber = item.WalletNumber, BankName = item.BankName, Iban = item.Iban };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        await LoadLookups();
        var user = User.Identity?.Name ?? "System";
        var appUser = await userManager.GetUserAsync(User) ?? await dbContext.Users.FirstOrDefaultAsync();
        if (appUser is null)
        {
            ModelState.AddModelError("", "Create or seed a user before adding beneficiaries.");
            return Page();
        }
        if (!await otpService.ValidateAsync(appUser.Id, OtpPurpose, Input.OtpCode))
        {
            ModelState.AddModelError("Input.OtpCode", Ar ? "رمز التحقق غير صحيح. استخدم 00000 للاختبار." : "Invalid OTP. Use 00000 for testing.");
        }
        if (!ModelState.IsValid) return Page();
        var orgId = appUser.OrganizationId;
        var item = await dbContext.Beneficiaries.FirstOrDefaultAsync(b => b.Id == Input.Id);
        if (item is null) { item = new Beneficiary { Id = Guid.NewGuid(), OrganizationId = orgId, OwnerUserId = appUser.Id, CreatedBy = user }; dbContext.Beneficiaries.Add(item); }
        else { item.UpdatedAt = DateTime.UtcNow; item.UpdatedBy = user; }
        item.BeneficiaryType = Input.BeneficiaryType; item.Nickname = Input.Nickname; item.FullNameEn = Input.FullNameEn; item.FullNameAr = Input.FullNameAr; item.MobileNumber = Input.MobileNumber; item.Email = Input.Email; item.WalletNumber = Input.WalletNumber; item.BankName = Input.BankName; item.Iban = Input.Iban;
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("BeneficiarySaved", nameof(Beneficiary), item.Id.ToString(), item.OrganizationId, user);
        return RedirectToPage("Index");
    }
    public async Task<IActionResult> OnPostSendOtpAsync()
    {
        var appUser = await userManager.GetUserAsync(User) ?? await dbContext.Users.FirstOrDefaultAsync();
        if (appUser is null) return BadRequest(new { sent = false });
        var result = await otpService.SendAsync(appUser, OtpPurpose, Ar ? "ar-SA" : "en-US");
        return new JsonResult(new { sent = result.Sent, maskedDestination = result.MaskedDestination });
    }
    private async Task LoadLookups() => Types = (await lookups.OptionsAsync("BeneficiaryType", Ar ? "ar-SA" : "en-US")).Select(o => new SelectListItem(o.Text, o.Code)).ToList();
    public class InputModel
    {
        public Guid Id { get; set; }
        [Required] public string BeneficiaryType { get; set; } = "InternalWallet";
        [Required] public string Nickname { get; set; } = string.Empty;
        [Required] public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string WalletNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string Iban { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
