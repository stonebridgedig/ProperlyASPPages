using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Management
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
        public ManagementUser ManagementUserInfo { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.DomainTypes.HasFlag(DomainUserType.Management))
            {
                return RedirectToPage("/Dashboard");
            }

            CurrentUser = user;
            ManagementUserInfo = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id) ?? new ManagementUser();

            return Page();
        }
    }
}
