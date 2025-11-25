using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Pages.Admin.Management;

[Authorize(Policy = SystemAdminPolicies.CanManageCompanies)]
public class IndexModel : PageModel
{
    private readonly IManagementRepository _managementRepository;

    public IndexModel(IManagementRepository managementRepository)
    {
        _managementRepository = managementRepository;
    }

    public List<ManagementOrg> ManagementOrgs { get; set; } = new();
    public Dictionary<int, int> ManagementUserCounts { get; set; } = new();

    public async Task OnGetAsync()
    {
        ManagementOrgs = await _managementRepository.GetAllManagementOrgsAsync();
        
        foreach (var management in ManagementOrgs)
        {
            if (management.ManagementOrgId.HasValue)
            {
                var users = await _managementRepository.GetManagementUsersAsync(management.ManagementOrgId.Value);
                ManagementUserCounts[management.ManagementOrgId.Value] = users.Count;
            }
        }
    }
}
