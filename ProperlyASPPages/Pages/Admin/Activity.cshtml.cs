using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Pages.Admin;

[Authorize(Policy = SystemAdminPolicies.IsSystemAdmin)]
public class ActivityModel : PageModel
{
    private readonly ISystemAdminRepository _adminRepository;

    public ActivityModel(ISystemAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public List<SystemAdminActivityLog> ActivityLogs { get; set; } = new();
    public Dictionary<int, SystemAdmin> AdminLookup { get; set; } = new();

    public async Task OnGetAsync(int count = 100)
    {
        ActivityLogs = await _adminRepository.GetRecentActivityAsync(count);
        
        var admins = await _adminRepository.GetAllAsync(includeInactive: true);
        AdminLookup = admins.ToDictionary(a => a.SystemAdminId, a => a);
    }
}
