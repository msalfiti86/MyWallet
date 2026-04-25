// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserCustom> _signInManager;
        private readonly UserManager<UserCustom> _userManager;
        private readonly IUserStore<UserCustom> _userStore;
        private readonly IUserEmailStore<UserCustom> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserContext _dbContext;
        private readonly IWalletService _walletService;

        public RegisterModel(
            UserManager<UserCustom> userManager,
            IUserStore<UserCustom> userStore,
            SignInManager<UserCustom> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            UserContext dbContext,
            IWalletService walletService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _walletService = walletService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Organization legal name")]
            public string OrganizationLegalNameEn { get; set; }

            [Display(Name = "Organization Arabic name")]
            public string OrganizationLegalNameAr { get; set; }

            [Required]
            [Display(Name = "Short name")]
            public string OrganizationShortName { get; set; }

            [Required]
            [Display(Name = "Commercial registration number")]
            public string CommercialRegistrationNumber { get; set; }

            [Required]
            [RegularExpression(@"^(\+9665\d{8}|05\d{8})$", ErrorMessage = "Use Saudi format +9665XXXXXXXX or 05XXXXXXXX.")]
            [Display(Name = "Mobile number")]
            public string MobileNumber { get; set; }

            [Required]
            [Display(Name = "National ID or Iqama")]
            public string NationalIdOrIqama { get; set; }

            [Required]
            [Display(Name = "First name")]
            public string FirstNameEn { get; set; }

            [Required]
            [Display(Name = "Last name")]
            public string LastNameEn { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of birth")]
            public DateTime? DateOfBirthGregorian { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Postal code")]
            public string PostalCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    LegalNameEn = Input.OrganizationLegalNameEn,
                    LegalNameAr = Input.OrganizationLegalNameAr ?? Input.OrganizationLegalNameEn,
                    ShortName = Input.OrganizationShortName,
                    CommercialRegistrationNumber = Input.CommercialRegistrationNumber,
                    OfficialEmail = Input.Email,
                    OfficialPhone = Input.MobileNumber,
                    ManagerFullName = $"{Input.FirstNameEn} {Input.LastNameEn}",
                    ManagerNationalIdOrIqama = Input.NationalIdOrIqama,
                    ManagerMobile = Input.MobileNumber,
                    ManagerEmail = Input.Email,
                    NationalAddressCity = Input.City,
                    NationalAddressPostalCode = Input.PostalCode,
                    KycStatus = "Pending",
                    ComplianceStatus = "Normal",
                    IsActive = true,
                    CreatedBy = Input.Email
                };

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.OrganizationId = organization.Id;
                user.FirstNameEn = Input.FirstNameEn;
                user.LastNameEn = Input.LastNameEn;
                user.MobileNumber = Input.MobileNumber;
                user.PhoneNumber = Input.MobileNumber;
                user.NationalIdOrIqama = Input.NationalIdOrIqama;
                user.DateOfBirthGregorian = Input.DateOfBirthGregorian;
                user.NationalAddressCity = Input.City;
                user.NationalAddressPostalCode = Input.PostalCode;
                user.KycStatus = "Pending";
                user.CreatedBy = Input.Email;

                await using var tx = await _dbContext.Database.BeginTransactionAsync();
                _dbContext.Organizations.Add(organization);
                await _dbContext.SaveChangesAsync();
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "OrganizationAdmin");
                    await _walletService.EnsureWalletAsync(user);
                    await tx.CommitAsync();
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                await tx.RollbackAsync();
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private UserCustom CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserCustom>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserCustom)}'. " +
                    $"Ensure that '{nameof(UserCustom)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<UserCustom> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UserCustom>)_userStore;
        }
    }
}
