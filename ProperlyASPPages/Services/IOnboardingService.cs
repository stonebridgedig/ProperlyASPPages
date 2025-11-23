using Properly.Models;

namespace ProperlyASPPages.Services;

public interface IOnboardingService
{
    Task<CompanyOnboardingResult> CreateCompanyAndUserAsync(string identityUserId, CompanyOnboardingRequest request);
    Task<CompanyUser?> GetCompanyUserByIdentityUserIdAsync(string identityUserId);
    Task<bool> HasCompletedOnboardingAsync(string identityUserId);
    
    Task<CompanyInvitation> CreateInvitationAsync(int companyOrgId, string invitedByUserId, string email, string? fullName, string? role);
    Task<CompanyInvitation?> GetInvitationByTokenAsync(string token);
    Task<bool> AcceptInvitationAsync(string token, string identityUserId, string fullName);
    Task<bool> CanUserInviteToCompanyAsync(string identityUserId, int companyOrgId);
    Task<List<CompanyInvitation>> GetPendingInvitationsForCompanyAsync(int companyOrgId);
    Task<CompanyUser?> JoinCompanyViaInvitationAsync(string identityUserId, int companyOrgId, string fullName, string email, string? role);
}

public class CompanyOnboardingRequest
{
    public required string CompanyName { get; set; }
    public string? LegalName { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    public required string UserFullName { get; set; }
    public required string UserEmail { get; set; }
    public string? UserTitle { get; set; }
    public string? UserPhone { get; set; }
    public string? UserRole { get; set; }
}

public class CompanyOnboardingResult
{
    public bool Success { get; set; }
    public int? CompanyOrgId { get; set; }
    public int? CompanyUserId { get; set; }
    public string? ErrorMessage { get; set; }
}
