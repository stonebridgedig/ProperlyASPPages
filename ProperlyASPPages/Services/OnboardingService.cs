using Microsoft.AspNetCore.Identity;
using Properly.Models;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Services;

public class OnboardingService : IOnboardingService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OnboardingService> _logger;

    public OnboardingService(
        ICompanyRepository companyRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<OnboardingService> logger)
    {
        _companyRepository = companyRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<CompanyOnboardingResult> CreateCompanyAndUserAsync(string identityUserId, CompanyOnboardingRequest request)
    {
        try
        {
            var companyOrg = new CompanyOrg
            {
                Name = request.CompanyName,
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

            var companyOrgId = await _companyRepository.CreateCompanyOrgAsync(companyOrg);

            if (companyOrgId <= 0)
            {
                return new CompanyOnboardingResult
                {
                    Success = false,
                    ErrorMessage = "Failed to create company organization."
                };
            }

            var companyUser = new CompanyUser
            {
                CompanyOrgId = companyOrgId,
                IdentityUserId = identityUserId,
                FullName = request.UserFullName,
                Email = request.UserEmail,
                Title = request.UserTitle,
                Phone = request.UserPhone,
                Role = request.UserRole ?? "Admin",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            var companyUserId = await _companyRepository.CreateCompanyUserAsync(companyUser);

            if (companyUserId <= 0)
            {
                return new CompanyOnboardingResult
                {
                    Success = false,
                    ErrorMessage = "Failed to create company user."
                };
            }

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                identityUser.DomainTypes |= DomainUserType.Company;
                identityUser.LastCompanyUserId = companyUserId;
                identityUser.LastDomainContextSwitchUtc = DateTime.UtcNow;
                await _userManager.UpdateAsync(identityUser);
            }

            _logger.LogInformation("Successfully created company {CompanyOrgId} and company user {CompanyUserId} for identity user {IdentityUserId}",
                companyOrgId, companyUserId, identityUserId);

            return new CompanyOnboardingResult
            {
                Success = true,
                CompanyOrgId = companyOrgId,
                CompanyUserId = companyUserId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company and user for identity user {IdentityUserId}", identityUserId);
            return new CompanyOnboardingResult
            {
                Success = false,
                ErrorMessage = "An error occurred during onboarding. Please try again."
            };
        }
    }

    public async Task<CompanyUser?> GetCompanyUserByIdentityUserIdAsync(string identityUserId)
    {
        return await _companyRepository.GetCompanyUserByIdentityUserIdAsync(identityUserId);
    }

    public async Task<bool> HasCompletedOnboardingAsync(string identityUserId)
    {
        var identityUser = await _userManager.FindByIdAsync(identityUserId);
        if (identityUser == null)
            return false;

        return identityUser.DomainTypes.HasFlag(DomainUserType.Company) && 
               identityUser.LastCompanyUserId.HasValue;
    }

    public async Task<CompanyInvitation> CreateInvitationAsync(int companyOrgId, string invitedByUserId, string email, string? fullName, string? role)
    {
        var token = Guid.NewGuid().ToString("N");
        
        var invitation = new CompanyInvitation
        {
            CompanyOrgId = companyOrgId,
            Email = email.ToLowerInvariant(),
            InvitedByUserId = invitedByUserId,
            InvitationToken = token,
            Status = InvitationStatus.Pending,
            Role = role ?? "User",
            InvitedFullName = fullName,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var invitationId = await _companyRepository.CreateInvitationAsync(invitation);
        invitation.InvitationId = invitationId;

        _logger.LogInformation("Created invitation {InvitationId} for {Email} to company {CompanyOrgId}",
            invitationId, email, companyOrgId);

        return invitation;
    }

    public async Task<CompanyInvitation?> GetInvitationByTokenAsync(string token)
    {
        return await _companyRepository.GetInvitationByTokenAsync(token);
    }

    public async Task<bool> AcceptInvitationAsync(string token, string identityUserId, string fullName)
    {
        try
        {
            var invitation = await _companyRepository.GetInvitationByTokenAsync(token);
            
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
                await _companyRepository.UpdateInvitationStatusAsync(invitation.InvitationId!.Value, InvitationStatus.Expired);
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

            var companyUser = await JoinCompanyViaInvitationAsync(
                identityUserId,
                invitation.CompanyOrgId!.Value,
                fullName,
                identityUser.Email ?? invitation.Email!,
                invitation.Role);

            if (companyUser != null)
            {
                await _companyRepository.UpdateInvitationStatusAsync(
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

    public async Task<bool> CanUserInviteToCompanyAsync(string identityUserId, int companyOrgId)
    {
        return await _companyRepository.IsUserAdminOfCompanyAsync(identityUserId, companyOrgId);
    }

    public async Task<List<CompanyInvitation>> GetPendingInvitationsForCompanyAsync(int companyOrgId)
    {
        return await _companyRepository.GetPendingInvitationsByCompanyAsync(companyOrgId);
    }

    public async Task<CompanyUser?> JoinCompanyViaInvitationAsync(string identityUserId, int companyOrgId, string fullName, string email, string? role)
    {
        try
        {
            var company = await _companyRepository.GetCompanyOrgByIdAsync(companyOrgId);
            if (company == null || company.IsActive != true)
            {
                _logger.LogWarning("Cannot join company {CompanyOrgId} - company not found or inactive", companyOrgId);
                return null;
            }

            var existingUser = await _companyRepository.GetCompanyUsersByIdentityUserIdAsync(identityUserId);
            if (existingUser.Any(u => u.CompanyOrgId == companyOrgId))
            {
                _logger.LogWarning("User {IdentityUserId} is already a member of company {CompanyOrgId}", identityUserId, companyOrgId);
                return existingUser.First(u => u.CompanyOrgId == companyOrgId);
            }

            var companyUser = new CompanyUser
            {
                CompanyOrgId = companyOrgId,
                IdentityUserId = identityUserId,
                FullName = fullName,
                Email = email,
                Role = role ?? "User",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            var companyUserId = await _companyRepository.CreateCompanyUserAsync(companyUser);

            if (companyUserId <= 0)
            {
                return null;
            }

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                identityUser.DomainTypes |= DomainUserType.Company;
                identityUser.LastCompanyUserId = companyUserId;
                identityUser.LastDomainContextSwitchUtc = DateTime.UtcNow;
                await _userManager.UpdateAsync(identityUser);
            }

            companyUser.CompanyUserId = companyUserId;
            return companyUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining company {CompanyOrgId} via invitation for identity user {IdentityUserId}", companyOrgId, identityUserId);
            return null;
        }
    }

    // Kept for backward compatibility but should not be exposed publicly
    private async Task<List<CompanyOrg>> SearchCompaniesAsync(string searchTerm)
    {
        return await _companyRepository.SearchCompanyOrgsByNameAsync(searchTerm);
    }

    // Deprecated - Use JoinCompanyViaInvitationAsync instead
    [Obsolete("Direct company joining is not allowed. Use invitation-based joining instead.")]
    public async Task<CompanyUser?> JoinExistingCompanyAsync(string identityUserId, int companyOrgId, string fullName, string email)
    {
        try
        {
            var company = await _companyRepository.GetCompanyOrgByIdAsync(companyOrgId);
            if (company == null || company.IsActive != true)
            {
                _logger.LogWarning("Cannot join company {CompanyOrgId} - company not found or inactive", companyOrgId);
                return null;
            }

            var companyUser = new CompanyUser
            {
                CompanyOrgId = companyOrgId,
                IdentityUserId = identityUserId,
                FullName = fullName,
                Email = email,
                Role = "User",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            var companyUserId = await _companyRepository.CreateCompanyUserAsync(companyUser);

            if (companyUserId <= 0)
            {
                return null;
            }

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                identityUser.DomainTypes |= DomainUserType.Company;
                identityUser.LastCompanyUserId = companyUserId;
                identityUser.LastDomainContextSwitchUtc = DateTime.UtcNow;
                await _userManager.UpdateAsync(identityUser);
            }

            companyUser.CompanyUserId = companyUserId;
            return companyUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining company {CompanyOrgId} for identity user {IdentityUserId}", companyOrgId, identityUserId);
            return null;
        }
    }
}
