using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Pages.Management;

[Authorize]
public class ManagementIndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IManagementRepository _managementRepository;

    public ManagementIndexModel(
        UserManager<ApplicationUser> userManager,
        IManagementRepository managementRepository)
    {
        _userManager = userManager;
        _managementRepository = managementRepository;
    }

    public ManagementUser? CurrentUser { get; set; }
    public int PendingInvitations { get; set; }
    public List<ManagementInvitation> RecentInvitations { get; set; } = new();

    public async Task OnGetAsync()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        if (identityUser == null)
            return;

        CurrentUser = await _managementRepository.GetManagementUserByIdentityUserIdAsync(identityUser.Id);
        
        if (CurrentUser?.ManagementOrgId.HasValue == true)
        {
            var allInvitations = await _managementRepository.GetPendingInvitationsByManagementAsync(CurrentUser.ManagementOrgId.Value);
            PendingInvitations = allInvitations.Count;
            RecentInvitations = allInvitations.Take(10).ToList();
        }
    }
}
