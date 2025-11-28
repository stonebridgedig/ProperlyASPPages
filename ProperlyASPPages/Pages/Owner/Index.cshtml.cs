using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;

namespace ProperlyASPPages.Pages.Owner
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser CurrentUser { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.DomainTypes.HasFlag(DomainUserType.Owner))
            {
                return RedirectToPage("/Dashboard");
            }

            CurrentUser = user;
            return Page();
        }
    }
}
