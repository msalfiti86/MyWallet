using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Configuration.Lookups;

public class IndexModel(UserContext dbContext, IAuditService auditService) : PageModel
{
    public List<Lookup> Lookups { get; set; } = [];

    public async Task OnGetAsync()
    {
        Lookups = await dbContext.Lookups.Include(l => l.Details).OrderBy(l => l.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var lookup = await dbContext.Lookups.Include(l => l.Details).FirstOrDefaultAsync(l => l.Id == id);
        if (lookup is not null)
        {
            lookup.IsDeleted = true;
            lookup.DeletedAt = DateTime.UtcNow;
            lookup.DeletedBy = User.Identity?.Name ?? "System";
            foreach (var detail in lookup.Details)
            {
                detail.IsDeleted = true;
                detail.DeletedAt = lookup.DeletedAt;
                detail.DeletedBy = lookup.DeletedBy;
            }
            await dbContext.SaveChangesAsync();
            await auditService.LogAsync("LookupDeleted", nameof(Lookup), id.ToString(), lookup.OrganizationId, lookup.DeletedBy);
        }

        return RedirectToPage();
    }

}
