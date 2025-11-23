using Microsoft.AspNetCore.Mvc;
using ProperlyASPPages.Repositories;
using System.Security.Claims;

namespace ProperlyASPPages.ViewComponents;

public class AdminLinkViewComponent : ViewComponent
{
    private readonly ISystemAdminRepository _adminRepository;

    public AdminLinkViewComponent(ISystemAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!UserClaimsPrincipal.Identity?.IsAuthenticated ?? true)
        {
            return Content(string.Empty);
        }

        var userId = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Content(string.Empty);
        }

        var isAdmin = await _adminRepository.IsSystemAdminAsync(userId);
        if (!isAdmin)
        {
            return Content(string.Empty);
        }

        return View();
    }
}
