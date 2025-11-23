using Microsoft.AspNetCore.Authorization;
using Properly.Models;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Authorization;

public class SystemAdminRequirement : IAuthorizationRequirement
{
    public SystemAdminPermission? Permission { get; }

    public SystemAdminRequirement(SystemAdminPermission? permission = null)
    {
        Permission = permission;
    }
}

public class SystemAdminHandler : AuthorizationHandler<SystemAdminRequirement>
{
    private readonly ISystemAdminRepository _adminRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SystemAdminHandler(ISystemAdminRepository adminRepository, IHttpContextAccessor httpContextAccessor)
    {
        _adminRepository = adminRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SystemAdminRequirement requirement)
    {
        var user = context.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var isAdmin = await _adminRepository.IsSystemAdminAsync(userId);
        if (!isAdmin)
        {
            return;
        }

        if (requirement.Permission.HasValue)
        {
            var hasPermission = await _adminRepository.HasPermissionAsync(userId, requirement.Permission.Value);
            if (!hasPermission)
            {
                return;
            }
        }

        context.Succeed(requirement);
    }
}

public static class SystemAdminPolicies
{
    public const string IsSystemAdmin = "IsSystemAdmin";
    public const string CanManageAdmins = "CanManageAdmins";
    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageCompanies = "CanManageCompanies";
    public const string CanViewReports = "CanViewReports";
    public const string CanManageSystem = "CanManageSystem";
    public const string CanAccessBilling = "CanAccessBilling";
    public const string CanManageSupport = "CanManageSupport";

    public static void AddSystemAdminPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(IsSystemAdmin, policy =>
            policy.Requirements.Add(new SystemAdminRequirement()));

        options.AddPolicy(CanManageAdmins, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.ManageAdmins)));

        options.AddPolicy(CanManageUsers, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.ManageUsers)));

        options.AddPolicy(CanManageCompanies, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.ManageCompanies)));

        options.AddPolicy(CanViewReports, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.ViewReports)));

        options.AddPolicy(CanManageSystem, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.ManageSystem)));

        options.AddPolicy(CanAccessBilling, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.AccessBilling)));

        options.AddPolicy(CanManageSupport, policy =>
            policy.Requirements.Add(new SystemAdminRequirement(SystemAdminPermission.ManageSupport)));
    }
}
