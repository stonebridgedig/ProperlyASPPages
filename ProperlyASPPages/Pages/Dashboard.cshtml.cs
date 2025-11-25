using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleContextService _roleContextService;

        public DashboardModel(UserManager<ApplicationUser> userManager, IRoleContextService roleContextService)
        {
            _userManager = userManager;
            _roleContextService = roleContextService;
        }

        public ApplicationUser CurrentUser { get; set; } = null!;
        public bool HasManagementRole { get; set; }
        public bool HasOwnerRole { get; set; }
        public bool HasTenantRole { get; set; }
        public bool HasServiceRole { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return;
            }

            CurrentUser = user;
            HasManagementRole = user.DomainTypes.HasFlag(DomainUserType.Management);
            HasOwnerRole = user.DomainTypes.HasFlag(DomainUserType.Owner);
            HasTenantRole = user.DomainTypes.HasFlag(DomainUserType.Tenant);
            HasServiceRole = user.DomainTypes.HasFlag(DomainUserType.Service);
        }

        public async Task<IActionResult> OnPostSelectRoleAsync(string role)
        {
            var domainType = role switch
            {
                "Management" => DomainUserType.Management,
                "Owner" => DomainUserType.Owner,
                "Tenant" => DomainUserType.Tenant,
                "Service" => DomainUserType.Service,
                _ => DomainUserType.None
            };

            _roleContextService.SetCurrentRole(domainType);

            // Redirect to the appropriate role dashboard
            return role switch
            {
                "Management" => RedirectToPage("/Management/Dashboard"),
                "Owner" => RedirectToPage("/Owner/Dashboard"),
                "Tenant" => RedirectToPage("/Tenant/Dashboard"),
                "Service" => RedirectToPage("/Service/Dashboard"),
                _ => RedirectToPage()
            };
        }
    }
}
