using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Pages.Admin.Admins;

[Authorize(Policy = SystemAdminPolicies.CanManageAdmins)]
public class IndexModel : PageModel
{
    private readonly ISystemAdminRepository _adminRepository;

    public IndexModel(ISystemAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public List<SystemAdmin> Admins { get; set; } = new();
    public List<SystemAdminRole> Roles { get; set; } = new();
    
    [TempData]
    public string? SuccessMessage { get; set; }
    
    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Admins = await _adminRepository.GetAllAsync(includeInactive: true);
        Roles = await _adminRepository.GetAllRolesAsync();
        
        return Page();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(int adminId)
    {
        try
        {
            var success = await _adminRepository.DeleteAsync(adminId);
            if (success)
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var currentAdmin = await _adminRepository.GetByIdentityUserIdAsync(currentUserId!);
                
                await _adminRepository.LogActivityAsync(
                    currentAdmin!.SystemAdminId,
                    "Deactivate Admin",
                    $"Deactivated admin ID: {adminId}",
                    "SystemAdmin",
                    adminId.ToString(),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                SuccessMessage = "Admin deactivated successfully.";
            }
            else
            {
                ErrorMessage = "Failed to deactivate admin.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }

        return RedirectToPage();
    }
}
