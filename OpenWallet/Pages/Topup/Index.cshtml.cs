using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Topup;

public class IndexModel(UserContext dbContext, UserManager<UserCustom> userManager, IWalletService walletService, ISettingsService settings) : PageModel
{
    public Wallet? Wallet { get; set; }
    public decimal MinimumTopUpAmount { get; set; }
    public decimal MaximumTopUpAmount { get; set; }
    [BindProperty] public TopupInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();
        Input.Amount = MinimumTopUpAmount;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadAsync();
        ValidateAmount();
        if (!ModelState.IsValid || Wallet is null) return Page();

        if (Input.PaymentMethod == "BankTransfer")
        {
            return Page();
        }

        return RedirectToPage("Simulate", new { amount = Input.Amount, method = Input.PaymentMethod });
    }

    private async Task LoadAsync()
    {
        MinimumTopUpAmount = await settings.GetDecimalAsync("WalletLimits", "MinimumTopUpAmount", 50);
        MaximumTopUpAmount = await settings.GetDecimalAsync("WalletLimits", "MaximumTopUpAmount", 100000);
        var user = await userManager.GetUserAsync(User) ?? await dbContext.Users.FirstOrDefaultAsync();
        if (user is not null)
        {
            Wallet = await walletService.EnsureWalletAsync(user);
        }
    }

    private void ValidateAmount()
    {
        if (Input.Amount < MinimumTopUpAmount || Input.Amount > MaximumTopUpAmount)
        {
            ModelState.AddModelError("Input.Amount", $"Top-up amount must be between SAR {MinimumTopUpAmount:N2} and SAR {MaximumTopUpAmount:N2}.");
        }
    }

    public class TopupInput
    {
        [Range(0.01, 999999999)] public decimal Amount { get; set; } = 50;
        [Required] public string PaymentMethod { get; set; } = "Card";
    }
}
