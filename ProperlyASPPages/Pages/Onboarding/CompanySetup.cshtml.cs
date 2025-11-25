using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;
using System.ComponentModel.DataAnnotations;

namespace ProperlyASPPages.Pages.Onboarding;

[Authorize]
public class ManagementSetupModel : PageModel
{
    private readonly IOnboardingService _onboardingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagementSetupModel(IOnboardingService onboardingService, UserManager<ApplicationUser> userManager)
    {
        _onboardingService = onboardingService;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public string ReturnUrl { get; set; } = "~/";

    public class InputModel
    {
        [Required]
        [Display(Name = "Management Name")]
        public string ManagementName { get; set; } = string.Empty;

        [Display(Name = "Legal Name")]
        public string? LegalName { get; set; }

        [Display(Name = "Tax ID")]
        public string? TaxId { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "State")]
        public string? State { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Phone]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [EmailAddress]
        [Display(Name = "Management Email")]
        public string? Email { get; set; }

        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Required]
        [Display(Name = "Your Full Name")]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Your Title")]
        public string? UserTitle { get; set; }

        [Phone]
        [Display(Name = "Your Phone")]
        public string? UserPhone { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var hasCompletedOnboarding = await _onboardingService.HasCompletedOnboardingAsync(user.Id);
        if (hasCompletedOnboarding)
        {
            return Redirect(returnUrl ?? "~/Dashboard");
        }

        ReturnUrl = returnUrl ?? "~/Dashboard";

        Input = new InputModel
        {
            UserFullName = $"{user.FirstName} {user.LastName}".Trim(),
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var request = new ManagementOnboardingRequest
        {
            ManagementName = Input.ManagementName,
            LegalName = Input.LegalName,
            TaxId = Input.TaxId,
            Address = Input.Address,
            City = Input.City,
            State = Input.State,
            PostalCode = Input.PostalCode,
            Country = Input.Country,
            Phone = Input.Phone,
            Email = Input.Email,
            Website = Input.Website,
            UserFullName = Input.UserFullName,
            UserEmail = user.Email ?? string.Empty,
            UserTitle = Input.UserTitle,
            UserPhone = Input.UserPhone,
            UserRole = "Admin"
        };

        var result = await _onboardingService.CreateManagementAndUserAsync(user.Id, request);

        if (result.Success)
        {
            StatusMessage = "Management setup completed successfully!";
            return LocalRedirect(returnUrl);
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "An error occurred during setup.");
            return Page();
        }
    }
}
