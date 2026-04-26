using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;

namespace OpenWallet.Pages.Transactions;

public class IndexModel(UserContext dbContext) : PageModel
{
    public List<WalletTransaction> Transactions { get; set; } = [];
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
    public int FailedCount { get; set; }
    public decimal TodayVolume { get; set; }

    public async Task OnGetAsync()
    {
        var today = DateTime.UtcNow.Date;
        Transactions = await dbContext.WalletTransactions
            .Include(t => t.FromWallet)
            .Include(t => t.ToWallet)
            .OrderByDescending(t => t.CreatedAt)
            .Take(100)
            .ToListAsync();

        CompletedCount = await dbContext.WalletTransactions.CountAsync(t => t.Status == "Completed");
        PendingCount = await dbContext.WalletTransactions.CountAsync(t => t.Status == "Pending");
        FailedCount = await dbContext.WalletTransactions.CountAsync(t => t.Status == "Failed");
        TodayVolume = await dbContext.WalletTransactions
            .Where(t => t.CreatedAt >= today && t.Direction == "Debit")
            .SumAsync(t => t.TotalAmount);
    }
}
