using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Tenant
{
    [Authorize]
    public class Tenant2Model : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleContextService _roleContextService;

        public Tenant2Model(UserManager<ApplicationUser> userManager, IRoleContextService roleContextService)
        {
            _userManager = userManager;
            _roleContextService = roleContextService;
        }

        public ApplicationUser CurrentUser { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || _roleContextService.GetCurrentRole() != DomainUserType.Tenant)
            {
                return RedirectToPage("/Dashboard");
            }

            CurrentUser = user;
            return Page();
        }
    }
}
