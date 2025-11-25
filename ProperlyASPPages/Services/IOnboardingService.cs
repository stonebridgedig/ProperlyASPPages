using Properly.Models;

namespace ProperlyASPPages.Services;

public interface IOnboardingService
{
    Task<ManagementOnboardingResult> CreateManagementAndUserAsync(string identityUserId, ManagementOnboardingRequest request);
    Task<ManagementUser?> GetManagementUserByIdentityUserIdAsync(string identityUserId);
    Task<bool> HasCompletedOnboardingAsync(string identityUserId);
    
    Task<ManagementInvitation> CreateInvitationAsync(int managementOrgId, string invitedByUserId, string email, string? fullName, string? role);
    Task<ManagementInvitation?> GetInvitationByTokenAsync(string token);
    Task<bool> AcceptInvitationAsync(string token, string identityUserId, string fullName);
    Task<bool> CanUserInviteToManagementAsync(string identityUserId, int managementOrgId);
    Task<List<ManagementInvitation>> GetPendingInvitationsForManagementAsync(int managementOrgId);
    Task<ManagementUser?> JoinManagementViaInvitationAsync(string identityUserId, int managementOrgId, string fullName, string email, string? role);
}

public class ManagementOnboardingRequest
{
    public required string ManagementName { get; set; }
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

public class ManagementOnboardingResult
{
    public bool Success { get; set; }
    public int? ManagementOrgId { get; set; }
    public int? ManagementUserId { get; set; }
    public string? ErrorMessage { get; set; }
}
