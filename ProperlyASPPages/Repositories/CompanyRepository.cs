using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Properly.Models;

namespace ProperlyASPPages.Repositories;

public class ManagementRepository : IManagementRepository
{
    private readonly IConfiguration _configuration;

    public ManagementRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("ProperlyDataDB"));
    }

    public async Task<int> CreateManagementOrgAsync(ManagementOrg managementOrg)
    {
        const string sql = @"
            INSERT INTO dbo.ManagementOrg 
            (Name, LegalName, TaxId, Address, City, State, PostalCode, Country, CountryCode, 
             Phone, Email, Website, IsActive, Notes, CreatedAt)
            VALUES 
            (@Name, @LegalName, @TaxId, @Address, @City, @State, @PostalCode, @Country, @CountryCode,
             @Phone, @Email, @Website, @IsActive, @Notes, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            managementOrg.Name,
            managementOrg.LegalName,
            managementOrg.TaxId,
            managementOrg.Address,
            managementOrg.City,
            managementOrg.State,
            managementOrg.PostalCode,
            managementOrg.Country,
            managementOrg.CountryCode,
            managementOrg.Phone,
            managementOrg.Email,
            managementOrg.Website,
            IsActive = managementOrg.IsActive ?? true,
            managementOrg.Notes
        });
    }

    public async Task<int> CreateManagementUserAsync(ManagementUser managementUser)
    {
        const string sql = @"
            INSERT INTO dbo.ManagementUser 
            (ManagementOrgId, IdentityUserId, FullName, Email, Title, Phone, AlternatePhone, 
             Role, IsActive, LastLoginAt, CreatedAt)
            VALUES 
            (@ManagementOrgId, @IdentityUserId, @FullName, @Email, @Title, @Phone, @AlternatePhone,
             @Role, @IsActive, @LastLoginAt, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            managementUser.ManagementOrgId,
            managementUser.IdentityUserId,
            managementUser.FullName,
            managementUser.Email,
            managementUser.Title,
            managementUser.Phone,
            managementUser.AlternatePhone,
            managementUser.Role,
            IsActive = managementUser.IsActive ?? true,
            managementUser.LastLoginAt
        });
    }

    public async Task<ManagementOrg?> GetManagementOrgByIdAsync(int managementOrgId)
    {
        const string sql = "SELECT * FROM dbo.ManagementOrg WHERE ManagementOrgId = @ManagementOrgId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ManagementOrg>(sql, new { ManagementOrgId = managementOrgId });
    }

    public async Task<ManagementUser?> GetManagementUserByIdAsync(int managementUserId)
    {
        const string sql = "SELECT * FROM dbo.ManagementUser WHERE ManagementUserId = @ManagementUserId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ManagementUser>(sql, new { ManagementUserId = managementUserId });
    }

    public async Task<ManagementUser?> GetManagementUserByIdentityUserIdAsync(string identityUserId)
    {
        const string sql = @"
            SELECT TOP 1 * FROM dbo.ManagementUser 
            WHERE IdentityUserId = @IdentityUserId 
            AND IsActive = 1
            ORDER BY LastLoginAt DESC";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ManagementUser>(sql, new { IdentityUserId = identityUserId });
    }

    public async Task<List<ManagementUser>> GetManagementUsersByIdentityUserIdAsync(string identityUserId)
    {
        const string sql = @"
            SELECT * FROM dbo.ManagementUser 
            WHERE IdentityUserId = @IdentityUserId 
            AND IsActive = 1
            ORDER BY LastLoginAt DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ManagementUser>(sql, new { IdentityUserId = identityUserId });
        return result.ToList();
    }

    public async Task<List<ManagementOrg>> SearchManagementOrgsByNameAsync(string searchTerm)
    {
        const string sql = @"
            SELECT TOP 20 * FROM dbo.ManagementOrg 
            WHERE IsActive = 1 
            AND (Name LIKE @SearchTerm OR LegalName LIKE @SearchTerm)
            ORDER BY Name";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ManagementOrg>(sql, new { SearchTerm = $"%{searchTerm}%" });
        return result.ToList();
    }

    public async Task<bool> UpdateManagementUserLastLoginAsync(int managementUserId, DateTime lastLoginAt)
    {
        const string sql = @"
            UPDATE dbo.ManagementUser 
            SET LastLoginAt = @LastLoginAt, UpdatedAt = GETUTCDATE()
            WHERE ManagementUserId = @ManagementUserId";

        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { ManagementUserId = managementUserId, LastLoginAt = lastLoginAt });
        return rowsAffected > 0;
    }

    public async Task<int> CreateInvitationAsync(ManagementInvitation invitation)
    {
        const string sql = @"
            INSERT INTO dbo.ManagementInvitation 
            (ManagementOrgId, Email, InvitedByUserId, InvitationToken, Status, Role, 
             InvitedFullName, ExpiresAt, CreatedAt)
            VALUES 
            (@ManagementOrgId, @Email, @InvitedByUserId, @InvitationToken, @Status, @Role,
             @InvitedFullName, @ExpiresAt, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            invitation.ManagementOrgId,
            invitation.Email,
            invitation.InvitedByUserId,
            invitation.InvitationToken,
            Status = (int)invitation.Status,
            invitation.Role,
            invitation.InvitedFullName,
            invitation.ExpiresAt
        });
    }

    public async Task<ManagementInvitation?> GetInvitationByTokenAsync(string token)
    {
        const string sql = @"
            SELECT * FROM dbo.ManagementInvitation 
            WHERE InvitationToken = @Token";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ManagementInvitation>(sql, new { Token = token });
    }

    public async Task<ManagementInvitation?> GetInvitationByIdAsync(int invitationId)
    {
        const string sql = @"
            SELECT * FROM dbo.ManagementInvitation 
            WHERE InvitationId = @InvitationId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ManagementInvitation>(sql, new { InvitationId = invitationId });
    }

    public async Task<List<ManagementInvitation>> GetPendingInvitationsByManagementAsync(int managementOrgId)
    {
        const string sql = @"
            SELECT * FROM dbo.ManagementInvitation 
            WHERE ManagementOrgId = @ManagementOrgId 
            AND Status = 0
            AND ExpiresAt > GETUTCDATE()
            ORDER BY CreatedAt DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ManagementInvitation>(sql, new { ManagementOrgId = managementOrgId });
        return result.ToList();
    }

    public async Task<bool> UpdateInvitationStatusAsync(int invitationId, InvitationStatus status, string? acceptedByUserId = null)
    {
        const string sql = @"
            UPDATE dbo.ManagementInvitation 
            SET Status = @Status, 
                AcceptedAt = CASE WHEN @Status = 1 THEN GETUTCDATE() ELSE AcceptedAt END,
                AcceptedByUserId = @AcceptedByUserId,
                UpdatedAt = GETUTCDATE()
            WHERE InvitationId = @InvitationId";

        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            InvitationId = invitationId, 
            Status = (int)status,
            AcceptedByUserId = acceptedByUserId
        });
        return rowsAffected > 0;
    }

    public async Task<bool> IsUserAdminOfManagementAsync(string identityUserId, int managementOrgId)
    {
        const string sql = @"
            SELECT COUNT(1) FROM dbo.ManagementUser 
            WHERE IdentityUserId = @IdentityUserId 
            AND ManagementOrgId = @ManagementOrgId
            AND IsActive = 1
            AND (Role = 'Admin' OR Role = 'Administrator')";

        using var connection = CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { IdentityUserId = identityUserId, ManagementOrgId = managementOrgId });
        return count > 0;
    }

    public async Task<ManagementInvitation?> GetPendingInvitationByEmailAndManagementAsync(string email, int managementOrgId)
    {
        const string sql = @"
            SELECT TOP 1 * FROM dbo.ManagementInvitation 
            WHERE Email = @Email 
            AND ManagementOrgId = @ManagementOrgId
            AND Status = 0
            AND ExpiresAt > GETUTCDATE()
            ORDER BY CreatedAt DESC";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ManagementInvitation>(sql, new { Email = email, ManagementOrgId = managementOrgId });
    }

    public async Task<List<ManagementOrg>> GetAllManagementOrgsAsync()
    {
        const string sql = "SELECT * FROM dbo.ManagementOrg ORDER BY Name";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ManagementOrg>(sql);
        return result.ToList();
    }

    public async Task<List<ManagementUser>> GetManagementUsersAsync(int managementOrgId)
    {
        const string sql = @"
            SELECT * FROM dbo.ManagementUser 
            WHERE ManagementOrgId = @ManagementOrgId
            ORDER BY FullName";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ManagementUser>(sql, new { ManagementOrgId = managementOrgId });
        return result.ToList();
    }
}
