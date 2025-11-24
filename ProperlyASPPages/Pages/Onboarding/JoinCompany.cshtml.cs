using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Onboarding;

[Authorize]
public class JoinCompanyModel : PageModel
{
    private readonly IOnboardingService _onboardingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public JoinCompanyModel(IOnboardingService onboardingService, UserManager<ApplicationUser> userManager)
    {
        _onboardingService = onboardingService;
        _userManager = userManager;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public string ReturnUrl { get; set; } = "~/";


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

        return Page();
    }
}
