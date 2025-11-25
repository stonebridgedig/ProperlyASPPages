using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Management;

[Authorize]
public class ManagementIndexModel : PageModel
{
    private readonly IOnboardingService _onboardingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagementIndexModel(
        IOnboardingService onboardingService,
        UserManager<ApplicationUser> userManager)
    {
        _onboardingService = onboardingService;
        _userManager = userManager;
    }

    public ManagementUser? CurrentUser { get; set; }
    public int PendingInvitations { get; set; }
    public List<ManagementInvitation> RecentInvitations { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        // Get current management user
        CurrentUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        
        if (CurrentUser?.ManagementOrgId != null)
        {
            var managementId = CurrentUser.ManagementOrgId.Value;

            // Get pending invitations
            var allInvitations = await _onboardingService.GetPendingInvitationsForManagementAsync(managementId);
            PendingInvitations = allInvitations.Count(i => !i.AcceptedAt.HasValue && i.ExpiresAt > DateTime.UtcNow);

            // Get recent invitations for display
            RecentInvitations = allInvitations
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToList();
        }
    }
}
