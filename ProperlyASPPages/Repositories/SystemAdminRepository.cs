using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Properly.Models;
using System.Data;

namespace ProperlyASPPages.Repositories;

public class SystemAdminRepository : ISystemAdminRepository
{
    private readonly string _connectionString;

    public SystemAdminRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ProperlyDataDB") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    public async Task<SystemAdmin?> GetByIdAsync(int adminId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT sa.*, sar.* 
            FROM SystemAdmin sa
            INNER JOIN SystemAdminRole sar ON sa.RoleId = sar.RoleId
            WHERE sa.SystemAdminId = @AdminId";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@AdminId", adminId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapSystemAdmin(reader);
        }

        return null;
    }

    public async Task<SystemAdmin?> GetByIdentityUserIdAsync(string identityUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT sa.*, sar.* 
            FROM SystemAdmin sa
            INNER JOIN SystemAdminRole sar ON sa.RoleId = sar.RoleId
            WHERE sa.IdentityUserId = @IdentityUserId AND sa.IsActive = 1";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdentityUserId", identityUserId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapSystemAdmin(reader);
        }

        return null;
    }

    public async Task<SystemAdmin?> GetByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT sa.*, sar.* 
            FROM SystemAdmin sa
            INNER JOIN SystemAdminRole sar ON sa.RoleId = sar.RoleId
            WHERE sa.Email = @Email";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapSystemAdmin(reader);
        }

        return null;
    }

    public async Task<List<SystemAdmin>> GetAllAsync(bool includeInactive = false)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT sa.*, sar.* 
            FROM SystemAdmin sa
            INNER JOIN SystemAdminRole sar ON sa.RoleId = sar.RoleId";

        if (!includeInactive)
        {
            query += " WHERE sa.IsActive = 1";
        }

        query += " ORDER BY sa.FirstName, sa.LastName";

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        var admins = new List<SystemAdmin>();
        while (await reader.ReadAsync())
        {
            admins.Add(MapSystemAdmin(reader));
        }

        return admins;
    }

    public async Task<SystemAdmin> CreateAsync(SystemAdmin admin)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO SystemAdmin 
            (IdentityUserId, RoleId, FirstName, LastName, Email, Phone, Department, Title, 
             IsActive, IsSuperAdmin, HireDate, Notes, CreatedAt, CreatedByAdminId)
            VALUES 
            (@IdentityUserId, @RoleId, @FirstName, @LastName, @Email, @Phone, @Department, @Title,
             @IsActive, @IsSuperAdmin, @HireDate, @Notes, @CreatedAt, @CreatedByAdminId);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdentityUserId", admin.IdentityUserId);
        command.Parameters.AddWithValue("@RoleId", admin.RoleId);
        command.Parameters.AddWithValue("@FirstName", admin.FirstName);
        command.Parameters.AddWithValue("@LastName", admin.LastName);
        command.Parameters.AddWithValue("@Email", admin.Email);
        command.Parameters.AddWithValue("@Phone", (object?)admin.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Department", (object?)admin.Department ?? DBNull.Value);
        command.Parameters.AddWithValue("@Title", (object?)admin.Title ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", admin.IsActive);
        command.Parameters.AddWithValue("@IsSuperAdmin", admin.IsSuperAdmin);
        command.Parameters.AddWithValue("@HireDate", (object?)admin.HireDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@Notes", (object?)admin.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", admin.CreatedAt);
        command.Parameters.AddWithValue("@CreatedByAdminId", (object?)admin.CreatedByAdminId ?? DBNull.Value);

        admin.SystemAdminId = (int)await command.ExecuteScalarAsync();
        return admin;
    }

    public async Task<SystemAdmin> UpdateAsync(SystemAdmin admin)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            UPDATE SystemAdmin 
            SET RoleId = @RoleId,
                FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email,
                Phone = @Phone,
                Department = @Department,
                Title = @Title,
                IsActive = @IsActive,
                IsSuperAdmin = @IsSuperAdmin,
                HireDate = @HireDate,
                LastAccessAt = @LastAccessAt,
                Notes = @Notes,
                UpdatedAt = @UpdatedAt
            WHERE SystemAdminId = @SystemAdminId";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SystemAdminId", admin.SystemAdminId);
        command.Parameters.AddWithValue("@RoleId", admin.RoleId);
        command.Parameters.AddWithValue("@FirstName", admin.FirstName);
        command.Parameters.AddWithValue("@LastName", admin.LastName);
        command.Parameters.AddWithValue("@Email", admin.Email);
        command.Parameters.AddWithValue("@Phone", (object?)admin.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Department", (object?)admin.Department ?? DBNull.Value);
        command.Parameters.AddWithValue("@Title", (object?)admin.Title ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", admin.IsActive);
        command.Parameters.AddWithValue("@IsSuperAdmin", admin.IsSuperAdmin);
        command.Parameters.AddWithValue("@HireDate", (object?)admin.HireDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@LastAccessAt", (object?)admin.LastAccessAt ?? DBNull.Value);
        command.Parameters.AddWithValue("@Notes", (object?)admin.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync();
        return admin;
    }

    public async Task<bool> DeleteAsync(int adminId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "UPDATE SystemAdmin SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE SystemAdminId = @AdminId";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@AdminId", adminId);
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> IsSystemAdminAsync(string identityUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT COUNT(1) FROM SystemAdmin WHERE IdentityUserId = @IdentityUserId AND IsActive = 1";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdentityUserId", identityUserId);

        var count = (int)await command.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<bool> HasPermissionAsync(string identityUserId, SystemAdminPermission permission)
    {
        var admin = await GetByIdentityUserIdAsync(identityUserId);
        if (admin == null || !admin.IsActive) return false;

        if (admin.IsSuperAdmin) return true;

        return permission switch
        {
            SystemAdminPermission.ManageAdmins => admin.Role.CanManageAdmins,
            SystemAdminPermission.ManageUsers => admin.Role.CanManageUsers,
            SystemAdminPermission.ManageCompanies => admin.Role.CanManageCompanies,
            SystemAdminPermission.ViewReports => admin.Role.CanViewReports,
            SystemAdminPermission.ManageSystem => admin.Role.CanManageSystem,
            SystemAdminPermission.AccessBilling => admin.Role.CanAccessBilling,
            SystemAdminPermission.ManageSupport => admin.Role.CanManageSupport,
            _ => false
        };
    }

    public async Task<List<SystemAdminRole>> GetAllRolesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT * FROM SystemAdminRole WHERE IsActive = 1 ORDER BY RoleName";

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        var roles = new List<SystemAdminRole>();
        while (await reader.ReadAsync())
        {
            roles.Add(MapSystemAdminRole(reader));
        }

        return roles;
    }

    public async Task<SystemAdminRole?> GetRoleByIdAsync(int roleId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT * FROM SystemAdminRole WHERE RoleId = @RoleId";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@RoleId", roleId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapSystemAdminRole(reader);
        }

        return null;
    }

    public async Task<SystemAdminRole> CreateRoleAsync(SystemAdminRole role)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO SystemAdminRole 
            (RoleName, Description, CanManageAdmins, CanManageUsers, CanManageCompanies, 
             CanViewReports, CanManageSystem, CanAccessBilling, CanManageSupport, IsActive, CreatedAt)
            VALUES 
            (@RoleName, @Description, @CanManageAdmins, @CanManageUsers, @CanManageCompanies,
             @CanViewReports, @CanManageSystem, @CanAccessBilling, @CanManageSupport, @IsActive, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@RoleName", role.RoleName);
        command.Parameters.AddWithValue("@Description", (object?)role.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@CanManageAdmins", role.CanManageAdmins);
        command.Parameters.AddWithValue("@CanManageUsers", role.CanManageUsers);
        command.Parameters.AddWithValue("@CanManageCompanies", role.CanManageCompanies);
        command.Parameters.AddWithValue("@CanViewReports", role.CanViewReports);
        command.Parameters.AddWithValue("@CanManageSystem", role.CanManageSystem);
        command.Parameters.AddWithValue("@CanAccessBilling", role.CanAccessBilling);
        command.Parameters.AddWithValue("@CanManageSupport", role.CanManageSupport);
        command.Parameters.AddWithValue("@IsActive", role.IsActive);
        command.Parameters.AddWithValue("@CreatedAt", role.CreatedAt);

        role.RoleId = (int)await command.ExecuteScalarAsync();
        return role;
    }

    public async Task<SystemAdminRole> UpdateRoleAsync(SystemAdminRole role)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            UPDATE SystemAdminRole 
            SET RoleName = @RoleName,
                Description = @Description,
                CanManageAdmins = @CanManageAdmins,
                CanManageUsers = @CanManageUsers,
                CanManageCompanies = @CanManageCompanies,
                CanViewReports = @CanViewReports,
                CanManageSystem = @CanManageSystem,
                CanAccessBilling = @CanAccessBilling,
                CanManageSupport = @CanManageSupport,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE RoleId = @RoleId";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@RoleId", role.RoleId);
        command.Parameters.AddWithValue("@RoleName", role.RoleName);
        command.Parameters.AddWithValue("@Description", (object?)role.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@CanManageAdmins", role.CanManageAdmins);
        command.Parameters.AddWithValue("@CanManageUsers", role.CanManageUsers);
        command.Parameters.AddWithValue("@CanManageCompanies", role.CanManageCompanies);
        command.Parameters.AddWithValue("@CanViewReports", role.CanViewReports);
        command.Parameters.AddWithValue("@CanManageSystem", role.CanManageSystem);
        command.Parameters.AddWithValue("@CanAccessBilling", role.CanAccessBilling);
        command.Parameters.AddWithValue("@CanManageSupport", role.CanManageSupport);
        command.Parameters.AddWithValue("@IsActive", role.IsActive);
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync();
        return role;
    }

    public async Task LogActivityAsync(int adminId, string activity, string? description = null,
        string? entityType = null, string? entityId = null, string? ipAddress = null, string? userAgent = null)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO SystemAdminActivityLog 
            (SystemAdminId, Activity, Description, EntityType, EntityId, IpAddress, UserAgent, Timestamp)
            VALUES 
            (@AdminId, @Activity, @Description, @EntityType, @EntityId, @IpAddress, @UserAgent, @Timestamp)";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@AdminId", adminId);
        command.Parameters.AddWithValue("@Activity", activity);
        command.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
        command.Parameters.AddWithValue("@EntityType", (object?)entityType ?? DBNull.Value);
        command.Parameters.AddWithValue("@EntityId", (object?)entityId ?? DBNull.Value);
        command.Parameters.AddWithValue("@IpAddress", (object?)ipAddress ?? DBNull.Value);
        command.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);
        command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<SystemAdminActivityLog>> GetActivityLogsAsync(int adminId, int pageSize = 50, int pageNumber = 1)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var offset = (pageNumber - 1) * pageSize;
        var query = @"
            SELECT * FROM SystemAdminActivityLog
            WHERE SystemAdminId = @AdminId
            ORDER BY Timestamp DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@AdminId", adminId);
        command.Parameters.AddWithValue("@Offset", offset);
        command.Parameters.AddWithValue("@PageSize", pageSize);

        using var reader = await command.ExecuteReaderAsync();

        var logs = new List<SystemAdminActivityLog>();
        while (await reader.ReadAsync())
        {
            logs.Add(MapActivityLog(reader));
        }

        return logs;
    }

    public async Task<List<SystemAdminActivityLog>> GetRecentActivityAsync(int count = 100)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT TOP (@Count) * FROM SystemAdminActivityLog
            ORDER BY Timestamp DESC";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Count", count);

        using var reader = await command.ExecuteReaderAsync();

        var logs = new List<SystemAdminActivityLog>();
        while (await reader.ReadAsync())
        {
            logs.Add(MapActivityLog(reader));
        }

        return logs;
    }

    private SystemAdmin MapSystemAdmin(SqlDataReader reader)
    {
        return new SystemAdmin
        {
            SystemAdminId = reader.GetInt32("SystemAdminId"),
            IdentityUserId = reader.GetString("IdentityUserId"),
            RoleId = reader.GetInt32("RoleId"),
            FirstName = reader.GetString("FirstName"),
            LastName = reader.GetString("LastName"),
            Email = reader.GetString("Email"),
            Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
            Department = reader.IsDBNull("Department") ? null : reader.GetString("Department"),
            Title = reader.IsDBNull("Title") ? null : reader.GetString("Title"),
            IsActive = reader.GetBoolean("IsActive"),
            IsSuperAdmin = reader.GetBoolean("IsSuperAdmin"),
            HireDate = reader.IsDBNull("HireDate") ? null : reader.GetDateTime("HireDate"),
            LastAccessAt = reader.IsDBNull("LastAccessAt") ? null : reader.GetDateTime("LastAccessAt"),
            Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt"),
            CreatedByAdminId = reader.IsDBNull("CreatedByAdminId") ? null : reader.GetInt32("CreatedByAdminId"),
            Role = MapSystemAdminRole(reader)
        };
    }

    private SystemAdminRole MapSystemAdminRole(SqlDataReader reader)
    {
        return new SystemAdminRole
        {
            RoleId = reader.GetInt32("RoleId"),
            RoleName = reader.GetString("RoleName"),
            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
            CanManageAdmins = reader.GetBoolean("CanManageAdmins"),
            CanManageUsers = reader.GetBoolean("CanManageUsers"),
            CanManageCompanies = reader.GetBoolean("CanManageCompanies"),
            CanViewReports = reader.GetBoolean("CanViewReports"),
            CanManageSystem = reader.GetBoolean("CanManageSystem"),
            CanAccessBilling = reader.GetBoolean("CanAccessBilling"),
            CanManageSupport = reader.GetBoolean("CanManageSupport"),
            IsActive = reader.GetBoolean("IsActive"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt")
        };
    }

    private SystemAdminActivityLog MapActivityLog(SqlDataReader reader)
    {
        return new SystemAdminActivityLog
        {
            LogId = reader.GetInt32("LogId"),
            SystemAdminId = reader.GetInt32("SystemAdminId"),
            Activity = reader.GetString("Activity"),
            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
            EntityType = reader.IsDBNull("EntityType") ? null : reader.GetString("EntityType"),
            EntityId = reader.IsDBNull("EntityId") ? null : reader.GetString("EntityId"),
            IpAddress = reader.IsDBNull("IpAddress") ? null : reader.GetString("IpAddress"),
            UserAgent = reader.IsDBNull("UserAgent") ? null : reader.GetString("UserAgent"),
            Timestamp = reader.GetDateTime("Timestamp")
        };
    }
}
