using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Properly.Models;
using ProperlyASPPages.Services;

namespace ProperlyASPPages.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ManagementController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ManagementController> _logger;

    public ManagementController(
        IOnboardingService onboardingService,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<ManagementController> logger)
    {
        _onboardingService = onboardingService;
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var managementUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        if (managementUser == null)
        {
            return NotFound("Management user not found");
        }

        return Ok(managementUser);
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var managementUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        if (managementUser?.ManagementOrgId == null)
        {
            return NotFound("Management organization not found");
        }

        var managementId = managementUser.ManagementOrgId.Value;
        var allInvitations = await _onboardingService.GetPendingInvitationsForManagementAsync(managementId);
        
        var stats = new
        {
            PendingInvitationsCount = allInvitations.Count(i => !i.AcceptedAt.HasValue && i.ExpiresAt > DateTime.UtcNow),
            RecentInvitations = allInvitations.OrderByDescending(i => i.CreatedAt).Take(5).ToList()
        };

        return Ok(stats);
    }

    [HttpGet("invitations")]
    public async Task<IActionResult> GetInvitations()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var managementUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        if (managementUser?.ManagementOrgId == null)
        {
            return NotFound("Management organization not found");
        }

        var invitations = await _onboardingService.GetPendingInvitationsForManagementAsync(managementUser.ManagementOrgId.Value);
        return Ok(invitations);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var managementUser = await _onboardingService.GetManagementUserByIdentityUserIdAsync(user.Id);
        if (managementUser?.ManagementOrgId == null)
        {
            return BadRequest("User is not part of a management organization");
        }

        var managementId = managementUser.ManagementOrgId.Value;
        var canInvite = await _onboardingService.CanUserInviteToManagementAsync(user.Id, managementId);
        
        if (!canInvite)
        {
            return Forbid("You do not have permission to invite users.");
        }

        try
        {
            var invitation = await _onboardingService.CreateInvitationAsync(
                managementId,
                user.Id,
                model.Email,
                model.FullName,
                model.Role);

            // Construct the invitation URL manually or pass it from the client if needed.
            // Since this is an API, we might need to know the base URL.
            // For now, I'll assume the client handles the redirect or we construct it using Request.Scheme/Host
            
            var inviteUrl = Url.Page(
                "/Onboarding/AcceptInvitation",
                pageHandler: null,
                values: new { token = invitation.InvitationToken },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "You're invited to join a management organization on Properly",
                $"<h2>You've been invited!</h2>" +
                $"<p>You've been invited to join a management organization on Properly.</p>" +
                $"<p><a href='{inviteUrl}'>Click here to accept the invitation</a></p>" +
                $"<p>This invitation will expire in 7 days.</p>");

            return Ok(new { message = "Invitation sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending invitation to {Email}", model.Email);
            return StatusCode(500, "An error occurred while sending the invitation.");
        }
    }

    public class InviteUserModel
    {
        public required string Email { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = "User";
    }
}
