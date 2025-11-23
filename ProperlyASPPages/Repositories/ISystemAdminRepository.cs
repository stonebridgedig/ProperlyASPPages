using Properly.Models;

namespace ProperlyASPPages.Repositories;

public interface ISystemAdminRepository
{
    Task<SystemAdmin?> GetByIdAsync(int adminId);
    Task<SystemAdmin?> GetByIdentityUserIdAsync(string identityUserId);
    Task<SystemAdmin?> GetByEmailAsync(string email);
    Task<List<SystemAdmin>> GetAllAsync(bool includeInactive = false);
    Task<SystemAdmin> CreateAsync(SystemAdmin admin);
    Task<SystemAdmin> UpdateAsync(SystemAdmin admin);
    Task<bool> DeleteAsync(int adminId);
    Task<bool> IsSystemAdminAsync(string identityUserId);
    Task<bool> HasPermissionAsync(string identityUserId, SystemAdminPermission permission);
    
    Task<List<SystemAdminRole>> GetAllRolesAsync();
    Task<SystemAdminRole?> GetRoleByIdAsync(int roleId);
    Task<SystemAdminRole> CreateRoleAsync(SystemAdminRole role);
    Task<SystemAdminRole> UpdateRoleAsync(SystemAdminRole role);
    
    Task LogActivityAsync(int adminId, string activity, string? description = null, 
        string? entityType = null, string? entityId = null, string? ipAddress = null, string? userAgent = null);
    Task<List<SystemAdminActivityLog>> GetActivityLogsAsync(int adminId, int pageSize = 50, int pageNumber = 1);
    Task<List<SystemAdminActivityLog>> GetRecentActivityAsync(int count = 100);
}
