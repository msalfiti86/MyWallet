using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Settings;

public class IndexModel(ISettingsService settings, UserManager<UserCustom> userManager, IAuditService auditService) : PageModel
{
    [BindProperty] public SettingsInput Input { get; set; } = new();
    public bool Saved { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.GetUserAsync(User);
        var userId = user?.Id ?? User.Identity?.Name ?? "System";

        await settings.SetAsync("TransferFees", "ExternalBankTransferFeeFixed", Input.ExternalBankTransferFeeFixed.ToString("0.##"), "Fixed fee for external beneficiary bank transfers", userId);
        await settings.SetAsync("TransferFees", "OpenWalletTransferFeeFixed", Input.OpenWalletTransferFeeFixed.ToString("0.##"), "Fixed fee for transfers to OpenWallet wallet accounts", userId);
        await settings.SetAsync("TransferFees", "InternalOrganizationTransferFeeFixed", Input.InternalOrganizationTransferFeeFixed.ToString("0.##"), "Fixed fee for transfers inside the same organization", userId);
        await settings.SetAsync("TransferFees", "VatPercentage", Input.VatPercentage.ToString("0.##"), "VAT percentage applied to transfer fees", userId);
        await settings.SetAsync("WalletLimits", "MinimumTransferAmount", Input.MinimumTransferAmount.ToString("0.##"), "Minimum transfer amount", userId);
        await settings.SetAsync("WalletLimits", "MaximumTransferAmount", Input.MaximumTransferAmount.ToString("0.##"), "Maximum transfer amount", userId);
        await settings.SetAsync("WalletLimits", "MinimumTopUpAmount", Input.MinimumTopUpAmount.ToString("0.##"), "Minimum wallet top-up amount", userId);
        await settings.SetAsync("WalletLimits", "MaximumTopUpAmount", Input.MaximumTopUpAmount.ToString("0.##"), "Maximum wallet top-up amount", userId);

        await auditService.LogAsync("SettingsUpdated", "SystemSetting", "WalletFeesAndLimits", user?.OrganizationId, userId);
        Saved = true;
        return Page();
    }

    private async Task LoadAsync()
    {
        Input.ExternalBankTransferFeeFixed = await settings.GetDecimalAsync("TransferFees", "ExternalBankTransferFeeFixed", 12);
        Input.OpenWalletTransferFeeFixed = await settings.GetDecimalAsync("TransferFees", "OpenWalletTransferFeeFixed", 1);
        Input.InternalOrganizationTransferFeeFixed = await settings.GetDecimalAsync("TransferFees", "InternalOrganizationTransferFeeFixed", 0);
        Input.VatPercentage = await settings.GetDecimalAsync("TransferFees", "VatPercentage", 15);
        Input.MinimumTransferAmount = await settings.GetDecimalAsync("WalletLimits", "MinimumTransferAmount", 1);
        Input.MaximumTransferAmount = await settings.GetDecimalAsync("WalletLimits", "MaximumTransferAmount", 100000);
        Input.MinimumTopUpAmount = await settings.GetDecimalAsync("WalletLimits", "MinimumTopUpAmount", 50);
        Input.MaximumTopUpAmount = await settings.GetDecimalAsync("WalletLimits", "MaximumTopUpAmount", 100000);
    }

    public class SettingsInput
    {
        [Range(0, 999999)] public decimal ExternalBankTransferFeeFixed { get; set; } = 12;
        [Range(0, 999999)] public decimal OpenWalletTransferFeeFixed { get; set; } = 1;
        [Range(0, 999999)] public decimal InternalOrganizationTransferFeeFixed { get; set; }
        [Range(0, 100)] public decimal VatPercentage { get; set; } = 15;
        [Range(0.01, 999999999)] public decimal MinimumTransferAmount { get; set; } = 1;
        [Range(0.01, 999999999)] public decimal MaximumTransferAmount { get; set; } = 100000;
        [Range(0.01, 999999999)] public decimal MinimumTopUpAmount { get; set; } = 50;
        [Range(0.01, 999999999)] public decimal MaximumTopUpAmount { get; set; } = 100000;
    }
}
