using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Pages.Admin;

[Authorize(Policy = SystemAdminPolicies.IsSystemAdmin)]
public class IndexModel : PageModel
{
    private readonly ISystemAdminRepository _adminRepository;
    private readonly ICompanyRepository _companyRepository;

    public IndexModel(ISystemAdminRepository adminRepository, ICompanyRepository companyRepository)
    {
        _adminRepository = adminRepository;
        _companyRepository = companyRepository;
    }

    public SystemAdmin CurrentAdmin { get; set; } = null!;
    public List<SystemAdminActivityLog> RecentActivity { get; set; } = new();
    public DashboardStats Stats { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var admin = await _adminRepository.GetByIdentityUserIdAsync(userId);
        if (admin == null)
        {
            return Forbid();
        }

        CurrentAdmin = admin;

        await _adminRepository.UpdateAsync(new SystemAdmin
        {
            SystemAdminId = CurrentAdmin.SystemAdminId,
            IdentityUserId = CurrentAdmin.IdentityUserId,
            RoleId = CurrentAdmin.RoleId,
            FirstName = CurrentAdmin.FirstName,
            LastName = CurrentAdmin.LastName,
            Email = CurrentAdmin.Email,
            Phone = CurrentAdmin.Phone,
            Department = CurrentAdmin.Department,
            Title = CurrentAdmin.Title,
            IsActive = CurrentAdmin.IsActive,
            IsSuperAdmin = CurrentAdmin.IsSuperAdmin,
            HireDate = CurrentAdmin.HireDate,
            LastAccessAt = DateTime.UtcNow,
            Notes = CurrentAdmin.Notes,
            CreatedAt = CurrentAdmin.CreatedAt,
            CreatedByAdminId = CurrentAdmin.CreatedByAdminId
        });

        RecentActivity = await _adminRepository.GetRecentActivityAsync(20);
        
        await LoadStatsAsync();

        return Page();
    }

    private async Task LoadStatsAsync()
    {
        var companies = await _companyRepository.GetAllCompaniesAsync();
        Stats.TotalCompanies = companies.Count;
        Stats.ActiveCompanies = companies.Count(c => c.IsActive ?? false);
        
        var admins = await _adminRepository.GetAllAsync(includeInactive: true);
        Stats.TotalAdmins = admins.Count;
        Stats.ActiveAdmins = admins.Count(a => a.IsActive);
    }

    public class DashboardStats
    {
        public int TotalCompanies { get; set; }
        public int ActiveCompanies { get; set; }
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
    }
}
