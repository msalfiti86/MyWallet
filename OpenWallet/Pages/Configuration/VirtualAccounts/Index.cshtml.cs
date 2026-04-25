using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Configuration.VirtualAccounts;

public class IndexModel(UserContext dbContext) : PageModel
{
    public List<VirtualAccount> Accounts { get; set; } = [];
    public int Total { get; set; }
    public int Available { get; set; }
    public int Used { get; set; }

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostSeedAsync()
    {
        var mainOrgId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var existing = await dbContext.VirtualAccounts.Select(v => v.Iban).ToListAsync();
        for (var i = existing.Count; i < 100; i++)
        {
            string iban;
            do { iban = VirtualAccountGenerator.GenerateIban(); } while (existing.Contains(iban));
            existing.Add(iban);
            dbContext.VirtualAccounts.Add(new VirtualAccount
            {
                Id = Guid.NewGuid(),
                OrganizationId = mainOrgId,
                Iban = iban,
                AccountNumber = VirtualAccountGenerator.GenerateAccountNumber(),
                BankName = "BSF Bank",
                CreatedBy = User.Identity?.Name ?? "System"
            });
        }

        await dbContext.SaveChangesAsync();
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Accounts = await dbContext.VirtualAccounts
            .OrderBy(v => v.IsUsed)
            .ThenBy(v => v.Iban)
            .Take(200)
            .ToListAsync();
        Total = await dbContext.VirtualAccounts.CountAsync();
        Used = await dbContext.VirtualAccounts.CountAsync(v => v.IsUsed);
        Available = Total - Used;
    }
}
