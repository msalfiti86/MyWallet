using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Beneficiaries;

public class IndexModel(UserContext dbContext, ILookupService lookups, IAuditService auditService) : PageModel
{
    public List<Beneficiary> Beneficiaries { get; set; } = [];
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";
    public async Task OnGetAsync() => Beneficiaries = await dbContext.Beneficiaries.OrderBy(b => b.Nickname).ToListAsync();
    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var item = await dbContext.Beneficiaries.FirstOrDefaultAsync(b => b.Id == id);
        if (item is not null)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = User.Identity?.Name ?? "System";
            await dbContext.SaveChangesAsync();
            await auditService.LogAsync("BeneficiarySoftDeleted", nameof(Beneficiary), id.ToString(), item.OrganizationId, item.DeletedBy);
        }
        return RedirectToPage();
    }
    public Task<string> LookupText(string name, string code) => lookups.TextAsync(name, code, Ar ? "ar-SA" : "en-US");
}
