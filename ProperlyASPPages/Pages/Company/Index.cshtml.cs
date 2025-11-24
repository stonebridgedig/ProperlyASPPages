using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Pages.Company;

[Authorize]
public class CompanyIndexModel : PageModel
{
    private readonly IOnboardingService _onboardingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyIndexModel(
        IOnboardingService onboardingService,
        UserManager<ApplicationUser> userManager)
    {
        _onboardingService = onboardingService;
        _userManager = userManager;
    }

    public CompanyUser? CurrentUser { get; set; }
    public int PendingInvitations { get; set; }
    public List<CompanyInvitation> RecentInvitations { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        // Get current company user
        CurrentUser = await _onboardingService.GetCompanyUserByIdentityUserIdAsync(user.Id);
        
        if (CurrentUser?.CompanyOrgId != null)
        {
            var companyId = CurrentUser.CompanyOrgId.Value;

            // Get pending invitations
            var allInvitations = await _onboardingService.GetPendingInvitationsForCompanyAsync(companyId);
            PendingInvitations = allInvitations.Count(i => !i.AcceptedAt.HasValue && i.ExpiresAt > DateTime.UtcNow);

            // Get recent invitations for display
            RecentInvitations = allInvitations
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToList();
        }
    }
}
