using Microsoft.AspNetCore.Identity;
using Properly.Models;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Services;

public class OnboardingService : IOnboardingService
{
    private readonly IManagementRepository _managementRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OnboardingService> _logger;

    public OnboardingService(
        IManagementRepository managementRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<OnboardingService> logger)
    {
        _managementRepository = managementRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ManagementOnboardingResult> CreateManagementAndUserAsync(string identityUserId, ManagementOnboardingRequest request)
    {
        try
        {
            var managementOrg = new ManagementOrg
            {
                Name = request.ManagementName,
                LegalName = request.LegalName,
                TaxId = request.TaxId,
                Address = request.Address,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                IsActive = true
            };

            var managementOrgId = await _managementRepository.CreateManagementOrgAsync(managementOrg);

            if (managementOrgId <= 0)
            {
                return new ManagementOnboardingResult
                {
                    Success = false,
                    ErrorMessage = "Failed to create management organization."
                };
            }

            var managementUser = new ManagementUser
            {
                ManagementOrgId = managementOrgId,
                IdentityUserId = identityUserId,
                FullName = request.UserFullName,
                Email = request.UserEmail,
                Title = request.UserTitle,
                Phone = request.UserPhone,
                Role = request.UserRole ?? "Admin",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            var managementUserId = await _managementRepository.CreateManagementUserAsync(managementUser);

            if (managementUserId <= 0)
            {
                return new ManagementOnboardingResult
                {
                    Success = false,
                    ErrorMessage = "Failed to create management user."
                };
            }

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                identityUser.DomainTypes |= DomainUserType.Management;
                identityUser.LastManagementUserId = managementUserId;
                identityUser.LastDomainContextSwitchUtc = DateTime.UtcNow;
                await _userManager.UpdateAsync(identityUser);
            }

            _logger.LogInformation("Successfully created management organization {ManagementOrgId} and management user {ManagementUserId} for identity user {IdentityUserId}",
                managementOrgId, managementUserId, identityUserId);

            return new ManagementOnboardingResult
            {
                Success = true,
                ManagementOrgId = managementOrgId,
                ManagementUserId = managementUserId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating management organization and user for identity user {IdentityUserId}", identityUserId);
            return new ManagementOnboardingResult
            {
                Success = false,
                ErrorMessage = "An error occurred during onboarding. Please try again."
            };
        }
    }

    public async Task<ManagementUser?> GetManagementUserByIdentityUserIdAsync(string identityUserId)
    {
        return await _managementRepository.GetManagementUserByIdentityUserIdAsync(identityUserId);
    }

    public async Task<bool> HasCompletedOnboardingAsync(string identityUserId)
    {
        var identityUser = await _userManager.FindByIdAsync(identityUserId);
        if (identityUser == null)
            return false;

        return identityUser.DomainTypes.HasFlag(DomainUserType.Management) && 
               identityUser.LastManagementUserId.HasValue;
    }

    public async Task<ManagementInvitation> CreateInvitationAsync(int managementOrgId, string invitedByUserId, string email, string? fullName, string? role)
    {
        var token = Guid.NewGuid().ToString("N");
        
        var invitation = new ManagementInvitation
        {
            ManagementOrgId = managementOrgId,
            Email = email.ToLowerInvariant(),
            InvitedByUserId = invitedByUserId,
            InvitationToken = token,
            Status = InvitationStatus.Pending,
            Role = role ?? "User",
            InvitedFullName = fullName,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var invitationId = await _managementRepository.CreateInvitationAsync(invitation);
        invitation.InvitationId = invitationId;

        _logger.LogInformation("Created invitation {InvitationId} for {Email} to management organization {ManagementOrgId}",
            invitationId, email, managementOrgId);

        return invitation;
    }

    public async Task<ManagementInvitation?> GetInvitationByTokenAsync(string token)
    {
        return await _managementRepository.GetInvitationByTokenAsync(token);
    }

    public async Task<bool> AcceptInvitationAsync(string token, string identityUserId, string fullName)
    {
        try
        {
            var invitation = await _managementRepository.GetInvitationByTokenAsync(token);
            
            if (invitation == null)
            {
                _logger.LogWarning("Invitation with token {Token} not found", token);
                return false;
            }

            if (invitation.Status != InvitationStatus.Pending)
            {
                _logger.LogWarning("Invitation {InvitationId} is not in pending status", invitation.InvitationId);
                return false;
            }

            if (invitation.ExpiresAt < DateTime.UtcNow)
            {
                await _managementRepository.UpdateInvitationStatusAsync(invitation.InvitationId!.Value, InvitationStatus.Expired);
                _logger.LogWarning("Invitation {InvitationId} has expired", invitation.InvitationId);
                return false;
            }

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser == null)
            {
                _logger.LogError("Identity user {IdentityUserId} not found", identityUserId);
                return false;
            }

            if (!string.Equals(identityUser.Email, invitation.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Email mismatch for invitation {InvitationId}. Expected {Expected}, got {Actual}",
                    invitation.InvitationId, invitation.Email, identityUser.Email);
                return false;
            }

            var managementUser = await JoinManagementViaInvitationAsync(
                identityUserId,
                invitation.ManagementOrgId!.Value,
                fullName,
                identityUser.Email ?? invitation.Email!,
                invitation.Role);

            if (managementUser != null)
            {
                await _managementRepository.UpdateInvitationStatusAsync(
                    invitation.InvitationId!.Value,
                    InvitationStatus.Accepted,
                    identityUserId);

                _logger.LogInformation("User {IdentityUserId} accepted invitation {InvitationId}",
                    identityUserId, invitation.InvitationId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting invitation with token {Token}", token);
            return false;
        }
    }

    public async Task<bool> CanUserInviteToManagementAsync(string identityUserId, int managementOrgId)
    {
        return await _managementRepository.IsUserAdminOfManagementAsync(identityUserId, managementOrgId);
    }

    public async Task<List<ManagementInvitation>> GetPendingInvitationsForManagementAsync(int managementOrgId)
    {
        return await _managementRepository.GetPendingInvitationsByManagementAsync(managementOrgId);
    }

    public async Task<ManagementUser?> JoinManagementViaInvitationAsync(string identityUserId, int managementOrgId, string fullName, string email, string? role)
    {
        try
        {
            var management = await _managementRepository.GetManagementOrgByIdAsync(managementOrgId);
            if (management == null || management.IsActive != true)
            {
                _logger.LogWarning("Cannot join management organization {ManagementOrgId} - organization not found or inactive", managementOrgId);
                return null;
            }

            var existingUser = await _managementRepository.GetManagementUsersByIdentityUserIdAsync(identityUserId);
            if (existingUser.Any(u => u.ManagementOrgId == managementOrgId))
            {
                _logger.LogWarning("User {IdentityUserId} is already a member of management organization {ManagementOrgId}", identityUserId, managementOrgId);
                return existingUser.First(u => u.ManagementOrgId == managementOrgId);
            }

            var managementUser = new ManagementUser
            {
                ManagementOrgId = managementOrgId,
                IdentityUserId = identityUserId,
                FullName = fullName,
                Email = email,
                Role = role ?? "User",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            var managementUserId = await _managementRepository.CreateManagementUserAsync(managementUser);

            if (managementUserId <= 0)
            {
                return null;
            }

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                identityUser.DomainTypes |= DomainUserType.Management;
                identityUser.LastManagementUserId = managementUserId;
                identityUser.LastDomainContextSwitchUtc = DateTime.UtcNow;
                await _userManager.UpdateAsync(identityUser);
            }

            managementUser.ManagementUserId = managementUserId;
            return managementUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining management organization {ManagementOrgId} via invitation for identity user {IdentityUserId}", managementOrgId, identityUserId);
            return null;
        }
    }
}
