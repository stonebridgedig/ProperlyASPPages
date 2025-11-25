using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Services;
using ProperlyASPPages.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ProperlyASPPages.Pages.Onboarding;

[Authorize]
public class AcceptInvitationModel : PageModel
{
    private readonly IOnboardingService _onboardingService;
    private readonly IManagementRepository _managementRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AcceptInvitationModel> _logger;

    public AcceptInvitationModel(
        IOnboardingService onboardingService,
        IManagementRepository managementRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<AcceptInvitationModel> logger)
    {
        _onboardingService = onboardingService;
        _managementRepository = managementRepository;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public ManagementInvitation? Invitation { get; set; }
    public ManagementOrg? Management { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Your Full Name")]
        public string FullName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Token))
        {
            ErrorMessage = "Invalid invitation link.";
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        Invitation = await _onboardingService.GetInvitationByTokenAsync(Token);

        if (Invitation == null)
        {
            ErrorMessage = "Invitation not found.";
            return Page();
        }

        if (Invitation.Status != InvitationStatus.Pending)
        {
            ErrorMessage = "This invitation has already been used or cancelled.";
            return Page();
        }

        if (Invitation.ExpiresAt < DateTime.UtcNow)
        {
            ErrorMessage = "This invitation has expired.";
            return Page();
        }

        if (!string.Equals(user.Email, Invitation.Email, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = $"This invitation is for {Invitation.Email}. Please sign in with that email address.";
            return Page();
        }

        Management = await _managementRepository.GetManagementOrgByIdAsync(Invitation.ManagementOrgId!.Value);
        
        if (Management == null)
        {
            ErrorMessage = "Management organization not found.";
            return Page();
        }

        IsValid = true;

        Input = new InputModel
        {
            FullName = Invitation.InvitedFullName ?? $"{user.FirstName} {user.LastName}".Trim()
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Token))
        {
            ErrorMessage = "Invalid invitation link.";
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            Invitation = await _onboardingService.GetInvitationByTokenAsync(Token);
            if (Invitation?.ManagementOrgId != null)
            {
                Management = await _managementRepository.GetManagementOrgByIdAsync(Invitation.ManagementOrgId.Value);
            }
            return Page();
        }

        var success = await _onboardingService.AcceptInvitationAsync(Token, user.Id, Input.FullName);

        if (success)
        {
            StatusMessage = "You have successfully joined the management organization!";
            return RedirectToPage("/Index");
        }
        else
        {
            ErrorMessage = "Unable to accept invitation. Please try again or contact support.";
            Invitation = await _onboardingService.GetInvitationByTokenAsync(Token);
            if (Invitation?.ManagementOrgId != null)
            {
                Management = await _managementRepository.GetManagementOrgByIdAsync(Invitation.ManagementOrgId.Value);
            }
            return Page();
        }
    }
}
