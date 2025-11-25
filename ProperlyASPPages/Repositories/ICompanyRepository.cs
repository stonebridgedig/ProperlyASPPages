using Properly.Models;

namespace ProperlyASPPages.Repositories;

public interface IManagementRepository
{
    Task<int> CreateManagementOrgAsync(ManagementOrg managementOrg);
    Task<int> CreateManagementUserAsync(ManagementUser managementUser);
    Task<ManagementOrg?> GetManagementOrgByIdAsync(int managementOrgId);
    Task<ManagementUser?> GetManagementUserByIdAsync(int managementUserId);
    Task<ManagementUser?> GetManagementUserByIdentityUserIdAsync(string identityUserId);
    Task<List<ManagementUser>> GetManagementUsersByIdentityUserIdAsync(string identityUserId);
    Task<List<ManagementOrg>> GetAllManagementOrgsAsync();
    Task<List<ManagementUser>> GetManagementUsersAsync(int managementOrgId);
    Task<List<ManagementOrg>> SearchManagementOrgsByNameAsync(string searchTerm);
    Task<bool> UpdateManagementUserLastLoginAsync(int managementUserId, DateTime lastLoginAt);
    
    Task<int> CreateInvitationAsync(ManagementInvitation invitation);
    Task<ManagementInvitation?> GetInvitationByTokenAsync(string token);
    Task<ManagementInvitation?> GetInvitationByIdAsync(int invitationId);
    Task<List<ManagementInvitation>> GetPendingInvitationsByManagementAsync(int managementOrgId);
    Task<bool> UpdateInvitationStatusAsync(int invitationId, InvitationStatus status, string? acceptedByUserId = null);
    Task<bool> IsUserAdminOfManagementAsync(string identityUserId, int managementOrgId);
    Task<ManagementInvitation?> GetPendingInvitationByEmailAndManagementAsync(string email, int managementOrgId);
}
