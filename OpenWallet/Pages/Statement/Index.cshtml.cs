using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;

namespace OpenWallet.Pages.Statement;

public class IndexModel(UserContext dbContext, UserManager<UserCustom> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)] public DateTime? FromDate { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? ToDate { get; set; }
    [BindProperty(SupportsGet = true)] public string? Type { get; set; }
    [BindProperty(SupportsGet = true)] public string? Direction { get; set; }
    [BindProperty(SupportsGet = true)] public string? Status { get; set; }
    [BindProperty(SupportsGet = true)] public string? Reference { get; set; }
    public List<WalletTransaction> Transactions { get; set; } = [];
    public List<SelectListItem> TypeOptions { get; set; } = [];
    public decimal OpeningBalance { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal ClosingBalance => OpeningBalance + TotalCredit - TotalDebit;
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var walletIds = await dbContext.Wallets
            .Where(w => w.UserId == user.Id)
            .Select(w => w.Id)
            .ToListAsync();

        var query = dbContext.WalletTransactions
            .Include(t => t.FromWallet)
            .Include(t => t.ToWallet)
            .Where(t => (t.FromWalletId.HasValue && walletIds.Contains(t.FromWalletId.Value)) || (t.ToWalletId.HasValue && walletIds.Contains(t.ToWalletId.Value)));

        if (FromDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= FromDate.Value.Date);
        }

        if (ToDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt < ToDate.Value.Date.AddDays(1));
        }

        if (!string.IsNullOrWhiteSpace(Type))
        {
            query = query.Where(t => t.TransactionType == Type);
        }

        if (!string.IsNullOrWhiteSpace(Direction))
        {
            query = query.Where(t => t.Direction == Direction);
        }

        if (!string.IsNullOrWhiteSpace(Status))
        {
            query = query.Where(t => t.Status == Status);
        }

        if (!string.IsNullOrWhiteSpace(Reference))
        {
            query = query.Where(t => t.ReferenceNumber.Contains(Reference) || t.TransactionNumber.Contains(Reference));
        }

        Transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .Take(500)
            .ToListAsync();

        TotalCredit = Transactions.Where(t => t.Direction == "Credit").Sum(t => t.TotalAmount);
        TotalDebit = Transactions.Where(t => t.Direction == "Debit").Sum(t => t.TotalAmount);

        var firstDate = FromDate?.Date ?? DateTime.MinValue;
        if (firstDate > DateTime.MinValue)
        {
            var before = await dbContext.WalletTransactions
                .Where(t => t.CreatedAt < firstDate && ((t.FromWalletId.HasValue && walletIds.Contains(t.FromWalletId.Value)) || (t.ToWalletId.HasValue && walletIds.Contains(t.ToWalletId.Value))))
                .ToListAsync();
            OpeningBalance = before.Where(t => t.Direction == "Credit").Sum(t => t.TotalAmount) - before.Where(t => t.Direction == "Debit").Sum(t => t.TotalAmount);
        }

        TypeOptions = await dbContext.WalletTransactions
            .Select(t => t.TransactionType)
            .Distinct()
            .OrderBy(t => t)
            .Select(t => new SelectListItem(t, t))
            .ToListAsync();

        return Page();
    }
}
