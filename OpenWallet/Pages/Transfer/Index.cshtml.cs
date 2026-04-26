using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Transfer;

public class IndexModel(
    UserContext dbContext,
    UserManager<UserCustom> userManager,
    IWalletService walletService,
    ISettingsService settings,
    IOtpService otpService,
    IAuditService auditService,
    INotificationService notifications) : PageModel
{
    private const string OtpPurpose = "Transfer.Execute";
    [BindProperty] public TransferInput Input { get; set; } = new();
    public Wallet? SenderWallet { get; set; }
    public decimal MinimumTransferAmount { get; set; }
    public decimal MaximumTransferAmount { get; set; }
    public decimal ExternalBankFee { get; set; }
    public decimal OpenWalletFee { get; set; }
    public decimal InternalOrganizationFee { get; set; }
    public decimal VatPercentage { get; set; }
    public List<SelectListItem> BeneficiaryOptions { get; set; } = [];
    public List<SelectListItem> SameOrganizationWalletOptions { get; set; } = [];
    public List<SelectListItem> OpenWalletUserOptions { get; set; } = [];
    public bool Ar => Request.Cookies["openwallet-lang"] == "ar";

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();
        Input.Amount = MinimumTransferAmount;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadAsync();
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        await ValidateInputAsync(user);
        if (!ModelState.IsValid || SenderWallet is null)
        {
            return Page();
        }

        Wallet? receiverWallet = null;
        Beneficiary? beneficiary = null;
        var destinationName = "";
        var destinationAccount = "";
        var transferType = Input.TransferType;
        var fee = GetFeeForTransferType(transferType);
        var vat = Math.Round(fee * VatPercentage / 100m, 2);
        var totalDebit = Input.Amount + fee + vat;

        if (SenderWallet.AvailableBalance < totalDebit)
        {
            ModelState.AddModelError("Input.Amount", $"Insufficient balance. Required SAR {totalDebit:N2}, available SAR {SenderWallet.AvailableBalance:N2}.");
            return Page();
        }

        if (transferType == TransferTypes.ExternalBeneficiary)
        {
            beneficiary = await dbContext.Beneficiaries.FirstOrDefaultAsync(b => b.Id == Input.BeneficiaryId && b.OwnerUserId == SenderWallet.UserId && b.IsActive);
            if (beneficiary is null)
            {
                ModelState.AddModelError("Input.BeneficiaryId", "Choose an active beneficiary.");
                return Page();
            }

            destinationName = Ar && !string.IsNullOrWhiteSpace(beneficiary.FullNameAr) ? beneficiary.FullNameAr : beneficiary.FullNameEn;
            destinationAccount = string.IsNullOrWhiteSpace(beneficiary.Iban) ? beneficiary.AccountNumber : beneficiary.Iban;
        }
        else
        {
            receiverWallet = await ResolveReceiverWalletAsync(transferType);
            if (receiverWallet is null)
            {
                ModelState.AddModelError("", "Choose a valid destination wallet.");
                return Page();
            }

            if (receiverWallet.Id == SenderWallet.Id)
            {
                ModelState.AddModelError("", "You cannot transfer to the same wallet.");
                return Page();
            }

            destinationName = DisplayName(receiverWallet.User);
            destinationAccount = receiverWallet.WalletNumber;
        }

        var adminWallet = await GetAdminWalletAsync();
        var now = DateTime.UtcNow;
        var reference = $"TRF-{now:yyyyMMdd-HHmmss}-{Random.Shared.Next(1000, 9999)}";
        var transactionType = transferType == TransferTypes.ExternalBeneficiary ? "ExternalBankTransfer" : "InternalTransfer";
        var transferLabel = transferType == TransferTypes.ExternalBeneficiary ? "External beneficiary transfer" : "Wallet transfer";

        await using var tx = await dbContext.Database.BeginTransactionAsync();
        SenderWallet.AvailableBalance -= totalDebit;
        SenderWallet.UpdatedAt = now;
        SenderWallet.UpdatedBy = user.Id;

        if (receiverWallet is not null)
        {
            receiverWallet.AvailableBalance += Input.Amount;
            receiverWallet.UpdatedAt = now;
            receiverWallet.UpdatedBy = user.Id;
        }

        var senderDebit = CreateTransaction(reference, "001", SenderWallet.OrganizationId, SenderWallet.Id, receiverWallet?.Id, transactionType, "Debit", Input.Amount, fee, vat, totalDebit, SenderWallet, user.Id, transferLabel, Input.Description);
        dbContext.WalletTransactions.Add(senderDebit);

        if (receiverWallet is not null)
        {
            dbContext.WalletTransactions.Add(CreateTransaction(reference, "002", receiverWallet.OrganizationId, SenderWallet.Id, receiverWallet.Id, transactionType, "Credit", Input.Amount, 0, 0, Input.Amount, receiverWallet, user.Id, $"Received from {DisplayName(user)}", Input.Description));
        }

        if (fee > 0)
        {
            dbContext.WalletTransactions.Add(CreateTransaction(reference, "FEE-D", SenderWallet.OrganizationId, SenderWallet.Id, adminWallet.Id, "Fee", "Debit", fee, 0, 0, fee, SenderWallet, user.Id, "Transfer fee", $"Fee for {reference}"));
            adminWallet.AvailableBalance += fee;
            dbContext.WalletTransactions.Add(CreateTransaction(reference, "FEE-C", adminWallet.OrganizationId, SenderWallet.Id, adminWallet.Id, "Fee", "Credit", fee, 0, 0, fee, adminWallet, user.Id, "Transfer fee collected", $"Fee for {reference}"));
        }

        if (vat > 0)
        {
            dbContext.WalletTransactions.Add(CreateTransaction(reference, "VAT-D", SenderWallet.OrganizationId, SenderWallet.Id, adminWallet.Id, "Adjustment", "Debit", vat, 0, 0, vat, SenderWallet, user.Id, "VAT on transfer fee", $"VAT for {reference}"));
            adminWallet.AvailableBalance += vat;
            dbContext.WalletTransactions.Add(CreateTransaction(reference, "VAT-C", adminWallet.OrganizationId, SenderWallet.Id, adminWallet.Id, "Adjustment", "Credit", vat, 0, 0, vat, adminWallet, user.Id, "VAT collected", $"VAT for {reference}"));
        }

        await dbContext.SaveChangesAsync();
        await tx.CommitAsync();

        await auditService.LogAsync("TransferCompleted", nameof(WalletTransaction), reference, SenderWallet.OrganizationId, user.Id);
        await notifications.CreateInAppAsync(user.Id, "Transfer completed", $"Transfer {reference} completed for SAR {Input.Amount:N2}.", "TransferCompleted");

        return RedirectToPage("Success", new { reference });
    }

    public async Task<IActionResult> OnPostSendOtpAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return BadRequest(new { sent = false });
        var result = await otpService.SendAsync(user, OtpPurpose, Ar ? "ar-SA" : "en-US");
        return new JsonResult(new { sent = result.Sent, maskedDestination = result.MaskedDestination });
    }

    private async Task LoadAsync()
    {
        MinimumTransferAmount = await settings.GetDecimalAsync("WalletLimits", "MinimumTransferAmount", 1);
        MaximumTransferAmount = await settings.GetDecimalAsync("WalletLimits", "MaximumTransferAmount", 100000);
        ExternalBankFee = await settings.GetDecimalAsync("TransferFees", "ExternalBankTransferFeeFixed", 12);
        OpenWalletFee = await settings.GetDecimalAsync("TransferFees", "OpenWalletTransferFeeFixed", 1);
        InternalOrganizationFee = await settings.GetDecimalAsync("TransferFees", "InternalOrganizationTransferFeeFixed", 0);
        VatPercentage = await settings.GetDecimalAsync("TransferFees", "VatPercentage", 15);

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        SenderWallet = await walletService.EnsureWalletAsync(user);

        BeneficiaryOptions = await dbContext.Beneficiaries
            .Where(b => b.OwnerUserId == user.Id && b.IsActive)
            .OrderBy(b => b.Nickname)
            .Select(b => new SelectListItem($"{b.Nickname} - {b.FullNameEn} - {(b.Iban == "" ? b.AccountNumber : b.Iban)}", b.Id.ToString()))
            .ToListAsync();

        SameOrganizationWalletOptions = await dbContext.Wallets
            .Include(w => w.User)
            .Where(w => w.OrganizationId == user.OrganizationId && w.Id != SenderWallet.Id && w.Status == "Active")
            .OrderBy(w => w.User!.FirstNameEn)
            .Select(w => new SelectListItem((w.User!.FirstNameEn + " " + w.User.LastNameEn + " - " + w.WalletNumber).Trim(), w.Id.ToString()))
            .ToListAsync();

        OpenWalletUserOptions = await dbContext.Wallets
            .Include(w => w.User)
            .Where(w => w.Id != SenderWallet.Id && w.Status == "Active")
            .OrderBy(w => w.User!.FirstNameEn)
            .Select(w => new SelectListItem((w.User!.FirstNameEn + " " + w.User.LastNameEn + " - ID " + w.User.NationalIdOrIqama + " - " + w.WalletNumber).Trim(), w.Id.ToString()))
            .ToListAsync();
    }

    private async Task ValidateInputAsync(UserCustom user)
    {
        if (Input.Amount < MinimumTransferAmount || Input.Amount > MaximumTransferAmount)
        {
            ModelState.AddModelError("Input.Amount", $"Transfer amount must be between SAR {MinimumTransferAmount:N2} and SAR {MaximumTransferAmount:N2}.");
        }

        if (SenderWallet is not null)
        {
            if (Input.Amount > SenderWallet.SingleTransactionLimit)
            {
                ModelState.AddModelError("Input.Amount", $"This wallet single transfer limit is SAR {SenderWallet.SingleTransactionLimit:N2}.");
            }

            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var transferTypes = new[] { "InternalTransfer", "ExternalBankTransfer" };
            var dailyUsed = await dbContext.WalletTransactions
                .Where(t => t.FromWalletId == SenderWallet.Id && t.Direction == "Debit" && transferTypes.Contains(t.TransactionType) && t.CreatedAt >= today)
                .SumAsync(t => t.Amount);
            var monthlyUsed = await dbContext.WalletTransactions
                .Where(t => t.FromWalletId == SenderWallet.Id && t.Direction == "Debit" && transferTypes.Contains(t.TransactionType) && t.CreatedAt >= monthStart)
                .SumAsync(t => t.Amount);

            if (dailyUsed + Input.Amount > SenderWallet.DailyLimit)
            {
                ModelState.AddModelError("Input.Amount", $"This wallet daily transfer limit is SAR {SenderWallet.DailyLimit:N2}. Used today: SAR {dailyUsed:N2}.");
            }

            if (monthlyUsed + Input.Amount > SenderWallet.MonthlyLimit)
            {
                ModelState.AddModelError("Input.Amount", $"This wallet monthly transfer limit is SAR {SenderWallet.MonthlyLimit:N2}. Used this month: SAR {monthlyUsed:N2}.");
            }
        }

        if (!await otpService.ValidateAsync(user.Id, OtpPurpose, Input.OtpCode?.Trim() ?? ""))
        {
            ModelState.AddModelError("Input.OtpCode", Ar ? "رمز التحقق غير صحيح. استخدم 00000 للاختبار." : "OTP is not valid. Use 00000 for testing.");
        }
    }

    private async Task<Wallet?> ResolveReceiverWalletAsync(string transferType)
    {
        if (transferType == TransferTypes.SameOrganizationWallet && Input.SameOrganizationWalletId.HasValue)
        {
            return await dbContext.Wallets.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == Input.SameOrganizationWalletId && w.Status == "Active");
        }

        if (transferType != TransferTypes.OpenWalletAccount)
        {
            return null;
        }

        if (Input.OpenWalletUserWalletId.HasValue)
        {
            return await dbContext.Wallets.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == Input.OpenWalletUserWalletId && w.Status == "Active");
        }

        var walletNumber = Input.OpenWalletWalletNumber?.Trim();
        return string.IsNullOrWhiteSpace(walletNumber)
            ? null
            : await dbContext.Wallets.Include(w => w.User).FirstOrDefaultAsync(w => w.WalletNumber == walletNumber && w.Status == "Active");
    }

    private async Task<Wallet> GetAdminWalletAsync()
    {
        var adminWallet = await dbContext.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.User != null && w.User.Email == "admin@openwallet.local");

        return adminWallet ?? await dbContext.Wallets.OrderBy(w => w.CreatedAt).FirstAsync();
    }

    private decimal GetFeeForTransferType(string transferType) => transferType switch
    {
        TransferTypes.ExternalBeneficiary => ExternalBankFee,
        TransferTypes.SameOrganizationWallet => InternalOrganizationFee,
        _ => OpenWalletFee
    };

    private static WalletTransaction CreateTransaction(string reference, string suffix, Guid organizationId, Guid? fromWalletId, Guid? toWalletId, string type, string direction, decimal amount, decimal fee, decimal vat, decimal total, Wallet wallet, string userId, string description, string note)
    {
        return new WalletTransaction
        {
            Id = Guid.NewGuid(),
            TransactionNumber = $"{reference}-{suffix}",
            OrganizationId = organizationId,
            FromWalletId = fromWalletId,
            ToWalletId = toWalletId,
            TransactionType = type,
            Direction = direction,
            Amount = amount,
            FeeAmount = fee,
            VatAmount = vat,
            TotalAmount = total,
            Currency = wallet.Currency,
            Status = "Completed",
            PaymentMethod = "Wallet",
            Category = type == "Fee" ? "Fee" : type == "Adjustment" ? "VAT" : "Transfer",
            ReferenceNumber = reference,
            VirtualIban = wallet.VirtualIban,
            BankName = wallet.BankName,
            Description = string.IsNullOrWhiteSpace(note) ? description : $"{description} - {note}",
            CreatedByUserId = userId
        };
    }

    private static string DisplayName(UserCustom? user)
    {
        if (user is null)
        {
            return "OpenWallet account";
        }

        var name = $"{user.FirstNameEn} {user.LastNameEn}".Trim();
        return string.IsNullOrWhiteSpace(name) ? user.Email ?? user.UserName ?? user.Id : name;
    }

    public decimal PreviewFee => GetFeeForTransferType(Input.TransferType);
    public decimal PreviewVat => Math.Round(PreviewFee * VatPercentage / 100m, 2);
    public decimal PreviewTotal => Input.Amount + PreviewFee + PreviewVat;

    public static class TransferTypes
    {
        public const string ExternalBeneficiary = "ExternalBeneficiary";
        public const string SameOrganizationWallet = "SameOrganizationWallet";
        public const string OpenWalletAccount = "OpenWalletAccount";
    }

    public class TransferInput
    {
        [Required] public string TransferType { get; set; } = TransferTypes.ExternalBeneficiary;
        public Guid? BeneficiaryId { get; set; }
        public Guid? SameOrganizationWalletId { get; set; }
        public Guid? OpenWalletUserWalletId { get; set; }
        [Display(Name = "Wallet number")] public string? OpenWalletWalletNumber { get; set; }
        [Range(0.01, 999999999)] public decimal Amount { get; set; } = 1;
        [StringLength(200)] public string Description { get; set; } = string.Empty;
        [StringLength(6, MinimumLength = 5), Display(Name = "OTP code")] public string OtpCode { get; set; } = string.Empty;
    }
}
