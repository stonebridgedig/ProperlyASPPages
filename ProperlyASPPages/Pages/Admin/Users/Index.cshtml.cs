using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;

namespace ProperlyASPPages.Pages.Admin.Users;

[Authorize(Policy = SystemAdminPolicies.CanManageUsers)]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public List<ApplicationUser> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        Users = _userManager.Users.OrderByDescending(u => u.CreatedAt).Take(100).ToList();
    }
}
