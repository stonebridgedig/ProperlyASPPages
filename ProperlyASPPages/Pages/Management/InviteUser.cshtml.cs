using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;

namespace ProperlyASPPages.Pages.Management;

[Authorize]
public class InviteUserModel : PageModel
{
    private readonly IOnboardingService _onboardingService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<InviteUserModel> _logger;

    public InviteUserModel(
        IOnboardingService onboardingService,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<InviteUserModel> logger)
    {
        _onboardingService = onboardingService;
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public List<ManagementInvitation> PendingInvitations { get; set; } = new();
    public int CurrentManagementOrgId { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "User";
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (!user.LastManagementUserId.HasValue)
        {
            return RedirectToPage("/Onboarding/ManagementSetup");
        }

        var managementUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        if (managementUser?.ManagementOrgId == null)
        {
            return RedirectToPage("/Onboarding/ManagementSetup");
        }

        CurrentManagementOrgId = managementUser.ManagementOrgId.Value;

        var canInvite = await _onboardingService.CanUserInviteToManagementAsync(user.Id, CurrentManagementOrgId);
        if (!canInvite)
        {
            StatusMessage = "You do not have permission to invite users to this management organization.";
            return Page();
        }

        PendingInvitations = await _onboardingService.GetPendingInvitationsForManagementAsync(CurrentManagementOrgId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (!user.LastManagementUserId.HasValue)
        {
            return RedirectToPage("/Onboarding/ManagementSetup");
        }

        var managementUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        if (managementUser?.ManagementOrgId == null)
        {
            return RedirectToPage("/Onboarding/ManagementSetup");
        }

        CurrentManagementOrgId = managementUser.ManagementOrgId.Value;

        var canInvite = await _onboardingService.CanUserInviteToManagementAsync(user.Id, CurrentManagementOrgId);
        if (!canInvite)
        {
            ModelState.AddModelError(string.Empty, "You do not have permission to invite users to this management organization.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            PendingInvitations = await _onboardingService.GetPendingInvitationsForManagementAsync(CurrentManagementOrgId);
            return Page();
        }

        try
        {
            var invitation = await _onboardingService.CreateInvitationAsync(
                CurrentManagementOrgId,
                user.Id,
                Input.Email,
                Input.FullName,
                Input.Role);

            var inviteUrl = Url.Page(
                "/Onboarding/AcceptInvitation",
                pageHandler: null,
                values: new { token = invitation.InvitationToken },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "You're invited to join a management organization on Properly",
                $"<h2>You've been invited!</h2>" +
                $"<p>You've been invited to join a management organization on Properly.</p>" +
                $"<p><a href='{inviteUrl}'>Click here to accept the invitation</a></p>" +
                $"<p>This invitation will expire in 7 days.</p>");

            StatusMessage = $"Invitation sent successfully to {Input.Email}";
            
            Input = new InputModel { Role = "User" };
            
            PendingInvitations = await _onboardingService.GetPendingInvitationsForManagementAsync(CurrentManagementOrgId);
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending invitation to {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred while sending the invitation. Please try again.");
            PendingInvitations = await _onboardingService.GetPendingInvitationsForManagementAsync(CurrentManagementOrgId);
            return Page();
        }
    }
}
