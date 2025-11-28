using Properly.Models;

namespace ProperlyASPPages.Repositories;

public interface IPropertyRepository
{
    Task<List<Property>> GetAllPropertiesAsync();
    Task<List<Property>> GetPropertiesByManagementAsync(int managementOrgId);
    Task<Property?> GetPropertyByIdAsync(int propertyId);
    Task<int> CreatePropertyAsync(Property property);
    Task<bool> UpdatePropertyAsync(Property property);
    Task<bool> DeletePropertyAsync(int propertyId);
}
