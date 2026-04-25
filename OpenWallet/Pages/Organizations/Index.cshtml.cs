using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Organizations;

public class IndexModel(UserContext dbContext, ILookupService lookups, IAuditService auditService) : PageModel
{
    public List<Organization> Organizations { get; set; } = [];
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";
    public async Task OnGetAsync() => Organizations = await dbContext.Organizations.OrderBy(o => o.ShortName).ToListAsync();

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var org = await dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        if (org is not null && !org.IsMainOrganization)
        {
            org.IsDeleted = true;
            org.DeletedAt = DateTime.UtcNow;
            org.DeletedBy = User.Identity?.Name ?? "System";
            await dbContext.SaveChangesAsync();
            await auditService.LogAsync("OrganizationSoftDeleted", nameof(Organization), id.ToString(), org.Id, org.DeletedBy);
        }
        return RedirectToPage();
    }

    public Task<string> LookupText(string name, string code) => lookups.TextAsync(name, code, Ar ? "ar-SA" : "en-US");
}
