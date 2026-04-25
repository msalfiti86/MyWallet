using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Topup;

public class IndexModel(UserContext dbContext, UserManager<UserCustom> userManager, IWalletService walletService, IAuditService auditService, INotificationService notifications) : PageModel
{
    public Wallet? Wallet { get; set; }
    [BindProperty] public TopupInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadWalletAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadWalletAsync();
        if (!ModelState.IsValid || Wallet is null) return Page();

        Wallet.AvailableBalance += Input.Amount;
        var tx = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            TransactionNumber = $"TXN-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Random.Shared.Next(100,999)}",
            OrganizationId = Wallet.OrganizationId,
            ToWalletId = Wallet.Id,
            TransactionType = "TopUp",
            Direction = "Credit",
            Amount = Input.Amount,
            TotalAmount = Input.Amount,
            Currency = Wallet.Currency,
            Status = "Completed",
            PaymentMethod = Input.PaymentMethod,
            Category = "TopUp",
            VirtualIban = Wallet.VirtualIban,
            BankName = Wallet.BankName,
            CreatedByUserId = Wallet.UserId ?? "",
            Description = $"Mock {Input.PaymentMethod} top-up"
        };
        dbContext.WalletTransactions.Add(tx);
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("TopUpCompleted", nameof(WalletTransaction), tx.Id.ToString(), Wallet.OrganizationId, Wallet.UserId ?? "");
        if (!string.IsNullOrWhiteSpace(Wallet.UserId))
        {
            await notifications.CreateInAppAsync(Wallet.UserId, "Top-up completed", $"SAR {Input.Amount:N2} credited to your wallet.", "TopUpCompleted");
        }
        return RedirectToPage();
    }

    private async Task LoadWalletAsync()
    {
        var user = await userManager.GetUserAsync(User) ?? await dbContext.Users.FirstOrDefaultAsync();
        if (user is not null)
        {
            Wallet = await walletService.EnsureWalletAsync(user);
        }
    }

    public class TopupInput
    {
        [Range(1, 999999)] public decimal Amount { get; set; } = 100;
        [Required] public string PaymentMethod { get; set; } = "Card";
    }
}
