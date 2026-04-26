using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;

namespace OpenWallet.Pages.Transfer;

public class SuccessModel(UserContext dbContext) : PageModel
{
    public string Reference { get; set; } = string.Empty;
    public List<WalletTransaction> Transactions { get; set; } = [];
    public WalletTransaction? MainDebit { get; set; }
    public WalletTransaction? MainCredit { get; set; }
    public decimal Amount => MainDebit?.Amount ?? 0;
    public decimal Fee => Transactions.Where(t => t.TransactionType == "Fee" && t.Direction == "Debit").Sum(t => t.Amount);
    public decimal Vat => Transactions.Where(t => t.Description.Contains("VAT") && t.Direction == "Debit").Sum(t => t.Amount);
    public decimal Total => Amount + Fee + Vat;
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";

    public async Task<IActionResult> OnGetAsync(string reference)
    {
        Reference = reference;
        Transactions = await dbContext.WalletTransactions
            .Include(t => t.FromWallet).ThenInclude(w => w!.User)
            .Include(t => t.ToWallet).ThenInclude(w => w!.User)
            .Where(t => t.ReferenceNumber == reference)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        MainDebit = Transactions.FirstOrDefault(t => t.Direction == "Debit" && t.TransactionType is "InternalTransfer" or "ExternalBankTransfer");
        MainCredit = Transactions.FirstOrDefault(t => t.Direction == "Credit" && t.TransactionType is "InternalTransfer" or "ExternalBankTransfer");

        return Transactions.Count == 0 ? RedirectToPage("Index") : Page();
    }

    public static string WalletLabel(Wallet? wallet)
    {
        if (wallet is null)
        {
            return "External beneficiary";
        }

        var userName = wallet.User is null ? "" : $"{wallet.User.FirstNameEn} {wallet.User.LastNameEn}".Trim();
        return string.IsNullOrWhiteSpace(userName) ? wallet.WalletNumber : $"{userName} - {wallet.WalletNumber}";
    }
}
