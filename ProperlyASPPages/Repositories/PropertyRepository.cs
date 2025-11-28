using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Properly.Models;

namespace ProperlyASPPages.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly IConfiguration _configuration;

    public PropertyRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("ProperlyDataDB"));
    }

    public async Task<List<Property>> GetAllPropertiesAsync()
    {
        const string sql = "SELECT * FROM dbo.Property ORDER BY Name";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<Property>(sql);
        return result.ToList();
    }

    public async Task<List<Property>> GetPropertiesByManagementAsync(int managementOrgId)
    {
        const string sql = @"
            SELECT * FROM dbo.Property 
            WHERE ManagementOrgId = @ManagementOrgId 
            ORDER BY Name";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<Property>(sql, new { ManagementOrgId = managementOrgId });
        return result.ToList();
    }

    public async Task<Property?> GetPropertyByIdAsync(int propertyId)
    {
        const string sql = "SELECT * FROM dbo.Property WHERE PropertyId = @PropertyId";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Property>(sql, new { PropertyId = propertyId });
    }

    public async Task<int> CreatePropertyAsync(Property property)
    {
        const string sql = @"
            INSERT INTO dbo.Property 
            (OwnerOrgId, ManagementOrgId, Name, Type, Address, City, State, PostalCode, 
             Country, CountryCode, Latitude, Longitude, TimeZoneId, UnitsCount, SquareFeet, 
             Status, AcquisitionDate, Notes, CreatedAt)
            VALUES 
            (@OwnerOrgId, @ManagementOrgId, @Name, @Type, @Address, @City, @State, @PostalCode,
             @Country, @CountryCode, @Latitude, @Longitude, @TimeZoneId, @UnitsCount, @SquareFeet,
             @Status, @AcquisitionDate, @Notes, GETUTCDATE());
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, property);
    }

    public async Task<bool> UpdatePropertyAsync(Property property)
    {
        const string sql = @"
            UPDATE dbo.Property 
            SET OwnerOrgId = @OwnerOrgId,
                ManagementOrgId = @ManagementOrgId,
                Name = @Name,
                Type = @Type,
                Address = @Address,
                City = @City,
                State = @State,
                PostalCode = @PostalCode,
                Country = @Country,
                CountryCode = @CountryCode,
                Latitude = @Latitude,
                Longitude = @Longitude,
                TimeZoneId = @TimeZoneId,
                UnitsCount = @UnitsCount,
                SquareFeet = @SquareFeet,
                Status = @Status,
                AcquisitionDate = @AcquisitionDate,
                Notes = @Notes,
                UpdatedAt = GETUTCDATE()
            WHERE PropertyId = @PropertyId";

        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, property);
        return rowsAffected > 0;
    }

    public async Task<bool> DeletePropertyAsync(int propertyId)
    {
        const string sql = "DELETE FROM dbo.Property WHERE PropertyId = @PropertyId";

        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { PropertyId = propertyId });
        return rowsAffected > 0;
    }
}
