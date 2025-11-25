using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Management
{
    [Authorize]
    public class Manager1Model : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleContextService _roleContextService;

        public Manager1Model(UserManager<ApplicationUser> userManager, IRoleContextService roleContextService)
        {
            _userManager = userManager;
            _roleContextService = roleContextService;
        }

        public ApplicationUser CurrentUser { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || _roleContextService.GetCurrentRole() != DomainUserType.Management)
            {
                return RedirectToPage("/Dashboard");
            }

            CurrentUser = user;
            return Page();
        }
    }
}
