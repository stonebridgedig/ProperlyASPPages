using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Company
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOnboardingService _onboardingService;

        public DashboardModel(UserManager<ApplicationUser> userManager, IOnboardingService onboardingService)
        {
            _userManager = userManager;
            _onboardingService = onboardingService;
        }

        public ApplicationUser CurrentUser { get; set; } = null!;
        public CompanyUser CompanyUserInfo { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.DomainTypes.HasFlag(DomainUserType.Company))
            {
                return RedirectToPage("/Dashboard");
            }

            CurrentUser = user;
            CompanyUserInfo = await _onboardingService.GetCompanyUserByIdentityUserIdAsync(user.Id) ?? new CompanyUser();

            return Page();
        }
    }
}
