using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Users;

public class IndexModel(UserContext dbContext, UserManager<UserCustom> userManager, RoleManager<IdentityRole> roleManager, ILookupService lookups, IAuditService auditService) : PageModel
{
    public List<UserCustom> Users { get; set; } = [];
    public List<SelectListItem> OrganizationOptions { get; set; } = [];
    public List<SelectListItem> RoleOptions { get; set; } = [];
    public List<SelectListItem> IdTypeOptions { get; set; } = [];
    [BindProperty] public InviteInput Invite { get; set; } = new();
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostInviteAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }
        var inviter = User.Identity?.Name ?? "System";
        dbContext.UserInvitations.Add(new UserInvitation
        {
            Id = Guid.NewGuid(),
            OrganizationId = Invite.OrganizationId,
            Email = Invite.Email,
            MobileNumber = Invite.MobileNumber,
            FirstNameEn = Invite.FirstNameEn,
            LastNameEn = Invite.LastNameEn,
            FirstNameAr = Invite.FirstNameAr,
            LastNameAr = Invite.LastNameAr,
            NationalIdOrIqama = Invite.NationalIdOrIqama,
            IdType = Invite.IdType,
            JobTitle = Invite.JobTitle,
            Department = Invite.Department,
            City = Invite.City,
            PostalCode = Invite.PostalCode,
            RoleName = Invite.RoleName,
            InvitationToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()),
            Status = "Pending",
            InvitedByUserId = inviter,
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("UserInvited", nameof(UserInvitation), Invite.Email, Invite.OrganizationId, inviter);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is not null)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = User.Identity?.Name ?? "System";
            user.IsActive = false;
            await userManager.UpdateAsync(user);
            await auditService.LogAsync("UserSoftDeleted", nameof(UserCustom), id, user.OrganizationId, user.DeletedBy);
        }
        return RedirectToPage();
    }

    public Task<string> LookupText(string name, string code) => lookups.TextAsync(name, code, Ar ? "ar-SA" : "en-US");

    private async Task LoadAsync()
    {
        Users = await dbContext.Users.Include(u => u.Organization).OrderBy(u => u.Email).ToListAsync();
        OrganizationOptions = await dbContext.Organizations.OrderBy(o => o.ShortName).Select(o => new SelectListItem(o.ShortName, o.Id.ToString())).ToListAsync();
        RoleOptions = await roleManager.Roles.OrderBy(r => r.Name).Select(r => new SelectListItem(r.Name!, r.Name!)).ToListAsync();
        IdTypeOptions = (await lookups.OptionsAsync("IdType", Ar ? "ar-SA" : "en-US")).Select(o => new SelectListItem(o.Text, o.Code)).ToList();
    }

    public class InviteInput
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string MobileNumber { get; set; } = string.Empty;
        [Required, StringLength(60)] public string FirstNameEn { get; set; } = string.Empty;
        [Required, StringLength(60)] public string LastNameEn { get; set; } = string.Empty;
        [StringLength(60)] public string FirstNameAr { get; set; } = string.Empty;
        [StringLength(60)] public string LastNameAr { get; set; } = string.Empty;
        [Required, RegularExpression(@"^[12]\d{9}$")] public string NationalIdOrIqama { get; set; } = string.Empty;
        [Required] public string IdType { get; set; } = "SaudiNationalId";
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        [RegularExpression(@"^\d{5}$")] public string PostalCode { get; set; } = string.Empty;
        [Required] public Guid OrganizationId { get; set; }
        [Required] public string RoleName { get; set; } = "User";
    }
}
