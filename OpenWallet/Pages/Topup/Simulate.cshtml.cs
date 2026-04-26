using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenWallet.Services;

namespace OpenWallet.Pages.Topup;

public class SimulateModel(ISettingsService settings) : PageModel
{
    [BindProperty] public PaymentInput Input { get; set; } = new();
    public decimal Amount => Input.Amount;
    public string Method => Input.Method;

    public async Task<IActionResult> OnGetAsync(decimal amount, string method)
    {
        Input.Amount = amount;
        Input.Method = method == "PayPal" ? "PayPal" : "Card";
        await ValidateLimitAsync();
        return ModelState.IsValid ? Page() : RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await ValidateLimitAsync();
        if (Input.Method == "PayPal")
        {
            if (string.IsNullOrWhiteSpace(Input.PayPalEmail))
            {
                ModelState.AddModelError("Input.PayPalEmail", "PayPal email is required.");
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(Input.CardholderName)) ModelState.AddModelError("Input.CardholderName", "Cardholder name is required.");
            if (!IsValidCardNumber(Input.CardNumber)) ModelState.AddModelError("Input.CardNumber", "Enter a valid test card number.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(Input.Expiry ?? "", @"^(0[1-9]|1[0-2])\/\d{2}$")) ModelState.AddModelError("Input.Expiry", "Use MM/YY format.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(Input.Cvv ?? "", @"^\d{3,4}$")) ModelState.AddModelError("Input.Cvv", "CVV must be 3 or 4 digits.");
        }

        if (!ModelState.IsValid) return Page();

        var reference = $"AUTH-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        return RedirectToPage("Authorize", new { amount = Input.Amount, method = Input.Method, reference });
    }

    private async Task ValidateLimitAsync()
    {
        var min = await settings.GetDecimalAsync("WalletLimits", "MinimumTopUpAmount", 50);
        var max = await settings.GetDecimalAsync("WalletLimits", "MaximumTopUpAmount", 100000);
        if (Input.Amount < min || Input.Amount > max)
        {
            ModelState.AddModelError("Input.Amount", $"Top-up amount must be between SAR {min:N2} and SAR {max:N2}.");
        }
    }

    private static bool IsValidCardNumber(string? value)
    {
        var digits = new string((value ?? "").Where(char.IsDigit).ToArray());
        return digits.Length is >= 13 and <= 19;
    }

    public class PaymentInput
    {
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Card";
        [Display(Name = "Cardholder name")] public string? CardholderName { get; set; }
        [Display(Name = "Card number")] public string? CardNumber { get; set; }
        public string? Expiry { get; set; }
        public string? Cvv { get; set; }
        [EmailAddress, Display(Name = "PayPal email")] public string? PayPalEmail { get; set; }
    }
}
