using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Properly.Models;

namespace ProperlyASPPages.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly IConfiguration _configuration;

    public CompanyRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("ProperlyDataDB"));
    }

    public async Task<int> CreateCompanyOrgAsync(CompanyOrg companyOrg)
    {
        const string sql = @"
            INSERT INTO dbo.CompanyOrg 
            (Name, LegalName, TaxId, Address, City, State, PostalCode, Country, CountryCode, 
             Phone, Email, Website, IsActive, Notes, CreatedAt)
            VALUES 
            (@Name, @LegalName, @TaxId, @Address, @City, @State, @PostalCode, @Country, @CountryCode,
             @Phone, @Email, @Website, @IsActive, @Notes, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            companyOrg.Name,
            companyOrg.LegalName,
            companyOrg.TaxId,
            companyOrg.Address,
            companyOrg.City,
            companyOrg.State,
            companyOrg.PostalCode,
            companyOrg.Country,
            companyOrg.CountryCode,
            companyOrg.Phone,
            companyOrg.Email,
            companyOrg.Website,
            IsActive = companyOrg.IsActive ?? true,
            companyOrg.Notes
        });
    }

    public async Task<int> CreateCompanyUserAsync(CompanyUser companyUser)
    {
        const string sql = @"
            INSERT INTO dbo.CompanyUser 
            (CompanyOrgId, IdentityUserId, FullName, Email, Title, Phone, AlternatePhone, 
             Role, IsActive, LastLoginAt, CreatedAt)
            VALUES 
            (@CompanyOrgId, @IdentityUserId, @FullName, @Email, @Title, @Phone, @AlternatePhone,
             @Role, @IsActive, @LastLoginAt, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            companyUser.CompanyOrgId,
            companyUser.IdentityUserId,
            companyUser.FullName,
            companyUser.Email,
            companyUser.Title,
            companyUser.Phone,
            companyUser.AlternatePhone,
            companyUser.Role,
            IsActive = companyUser.IsActive ?? true,
            companyUser.LastLoginAt
        });
    }

    public async Task<CompanyOrg?> GetCompanyOrgByIdAsync(int companyOrgId)
    {
        const string sql = "SELECT * FROM dbo.CompanyOrg WHERE CompanyOrgId = @CompanyOrgId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyOrg>(sql, new { CompanyOrgId = companyOrgId });
    }

    public async Task<CompanyUser?> GetCompanyUserByIdAsync(int companyUserId)
    {
        const string sql = "SELECT * FROM dbo.CompanyUser WHERE CompanyUserId = @CompanyUserId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyUser>(sql, new { CompanyUserId = companyUserId });
    }

    public async Task<CompanyUser?> GetCompanyUserByIdentityUserIdAsync(string identityUserId)
    {
        const string sql = @"
            SELECT TOP 1 * FROM dbo.CompanyUser 
            WHERE IdentityUserId = @IdentityUserId 
            AND IsActive = 1
            ORDER BY LastLoginAt DESC";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyUser>(sql, new { IdentityUserId = identityUserId });
    }

    public async Task<List<CompanyUser>> GetCompanyUsersByIdentityUserIdAsync(string identityUserId)
    {
        const string sql = @"
            SELECT * FROM dbo.CompanyUser 
            WHERE IdentityUserId = @IdentityUserId 
            AND IsActive = 1
            ORDER BY LastLoginAt DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CompanyUser>(sql, new { IdentityUserId = identityUserId });
        return result.ToList();
    }

    public async Task<List<CompanyOrg>> SearchCompanyOrgsByNameAsync(string searchTerm)
    {
        const string sql = @"
            SELECT TOP 20 * FROM dbo.CompanyOrg 
            WHERE IsActive = 1 
            AND (Name LIKE @SearchTerm OR LegalName LIKE @SearchTerm)
            ORDER BY Name";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CompanyOrg>(sql, new { SearchTerm = $"%{searchTerm}%" });
        return result.ToList();
    }

    public async Task<bool> UpdateCompanyUserLastLoginAsync(int companyUserId, DateTime lastLoginAt)
    {
        const string sql = @"
            UPDATE dbo.CompanyUser 
            SET LastLoginAt = @LastLoginAt, UpdatedAt = GETUTCDATE()
            WHERE CompanyUserId = @CompanyUserId";

        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { CompanyUserId = companyUserId, LastLoginAt = lastLoginAt });
        return rowsAffected > 0;
    }

    public async Task<int> CreateInvitationAsync(CompanyInvitation invitation)
    {
        const string sql = @"
            INSERT INTO dbo.CompanyInvitation 
            (CompanyOrgId, Email, InvitedByUserId, InvitationToken, Status, Role, 
             InvitedFullName, ExpiresAt, CreatedAt)
            VALUES 
            (@CompanyOrgId, @Email, @InvitedByUserId, @InvitationToken, @Status, @Role,
             @InvitedFullName, @ExpiresAt, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            invitation.CompanyOrgId,
            invitation.Email,
            invitation.InvitedByUserId,
            invitation.InvitationToken,
            Status = (int)invitation.Status,
            invitation.Role,
            invitation.InvitedFullName,
            invitation.ExpiresAt
        });
    }

    public async Task<CompanyInvitation?> GetInvitationByTokenAsync(string token)
    {
        const string sql = @"
            SELECT * FROM dbo.CompanyInvitation 
            WHERE InvitationToken = @Token";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyInvitation>(sql, new { Token = token });
    }

    public async Task<CompanyInvitation?> GetInvitationByIdAsync(int invitationId)
    {
        const string sql = @"
            SELECT * FROM dbo.CompanyInvitation 
            WHERE InvitationId = @InvitationId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyInvitation>(sql, new { InvitationId = invitationId });
    }

    public async Task<List<CompanyInvitation>> GetPendingInvitationsByCompanyAsync(int companyOrgId)
    {
        const string sql = @"
            SELECT * FROM dbo.CompanyInvitation 
            WHERE CompanyOrgId = @CompanyOrgId 
            AND Status = 0
            AND ExpiresAt > GETUTCDATE()
            ORDER BY CreatedAt DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CompanyInvitation>(sql, new { CompanyOrgId = companyOrgId });
        return result.ToList();
    }

    public async Task<bool> UpdateInvitationStatusAsync(int invitationId, InvitationStatus status, string? acceptedByUserId = null)
    {
        const string sql = @"
            UPDATE dbo.CompanyInvitation 
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

    public async Task<bool> IsUserAdminOfCompanyAsync(string identityUserId, int companyOrgId)
    {
        const string sql = @"
            SELECT COUNT(1) FROM dbo.CompanyUser 
            WHERE IdentityUserId = @IdentityUserId 
            AND CompanyOrgId = @CompanyOrgId
            AND IsActive = 1
            AND (Role = 'Admin' OR Role = 'Administrator')";

        using var connection = CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { IdentityUserId = identityUserId, CompanyOrgId = companyOrgId });
        return count > 0;
    }

    public async Task<CompanyInvitation?> GetPendingInvitationByEmailAndCompanyAsync(string email, int companyOrgId)
    {
        const string sql = @"
            SELECT TOP 1 * FROM dbo.CompanyInvitation 
            WHERE Email = @Email 
            AND CompanyOrgId = @CompanyOrgId
            AND Status = 0
            AND ExpiresAt > GETUTCDATE()
            ORDER BY CreatedAt DESC";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyInvitation>(sql, new { Email = email, CompanyOrgId = companyOrgId });
    }

    public async Task<List<CompanyOrg>> GetAllCompaniesAsync()
    {
        const string sql = "SELECT * FROM dbo.CompanyOrg ORDER BY Name";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CompanyOrg>(sql);
        return result.ToList();
    }

    public async Task<List<CompanyUser>> GetCompanyUsersAsync(int companyOrgId)
    {
        const string sql = @"
            SELECT * FROM dbo.CompanyUser 
            WHERE CompanyOrgId = @CompanyOrgId
            ORDER BY FullName";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CompanyUser>(sql, new { CompanyOrgId = companyOrgId });
        return result.ToList();
    }
}
