using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Topup;

public class AuthorizeModel(UserContext dbContext, UserManager<UserCustom> userManager, IWalletService walletService, IAuditService auditService, INotificationService notifications, ISettingsService settings) : PageModel
{
    [BindProperty] public AuthorizationInput Input { get; set; } = new();
    public decimal Amount => Input.Amount;
    public string Method => Input.Method;
    public string Reference => Input.Reference;

    public async Task<IActionResult> OnGetAsync(decimal amount, string method, string reference)
    {
        Input.Amount = amount;
        Input.Method = method == "PayPal" ? "PayPal" : "Card";
        Input.Reference = reference;
        return await ValidateLimitAsync() ? Page() : RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await ValidateLimitAsync()) return Page();
        var user = await userManager.GetUserAsync(User) ?? await dbContext.Users.FirstOrDefaultAsync();
        if (user is null) return RedirectToPage("Index");
        var wallet = await walletService.EnsureWalletAsync(user);

        wallet.AvailableBalance += Input.Amount;
        var tx = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            TransactionNumber = $"TXN-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Random.Shared.Next(100, 999)}",
            OrganizationId = wallet.OrganizationId,
            ToWalletId = wallet.Id,
            TransactionType = "TopUp",
            Direction = "Credit",
            Amount = Input.Amount,
            TotalAmount = Input.Amount,
            Currency = wallet.Currency,
            Status = "Completed",
            PaymentMethod = Input.Method,
            Category = "TopUp",
            ReferenceNumber = Input.Reference,
            ExternalProviderReference = Input.Reference,
            VirtualIban = wallet.VirtualIban,
            BankName = wallet.BankName,
            CreatedByUserId = user.Id,
            Description = $"Mock authorized {Input.Method} top-up"
        };
        dbContext.WalletTransactions.Add(tx);
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("TopUpCompleted", nameof(WalletTransaction), tx.Id.ToString(), wallet.OrganizationId, user.Id);
        await notifications.CreateInAppAsync(user.Id, "Top-up completed", $"SAR {Input.Amount:N2} credited to your wallet.", "TopUpCompleted");
        return RedirectToPage("/Transactions/Index");
    }

    private async Task<bool> ValidateLimitAsync()
    {
        var min = await settings.GetDecimalAsync("WalletLimits", "MinimumTopUpAmount", 50);
        var max = await settings.GetDecimalAsync("WalletLimits", "MaximumTopUpAmount", 100000);
        if (Input.Amount < min || Input.Amount > max)
        {
            ModelState.AddModelError("", $"Top-up amount must be between SAR {min:N2} and SAR {max:N2}.");
            return false;
        }
        return true;
    }

    public class AuthorizationInput
    {
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Card";
        public string Reference { get; set; } = string.Empty;
    }
}
