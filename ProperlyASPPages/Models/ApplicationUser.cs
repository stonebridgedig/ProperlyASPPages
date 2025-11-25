using System;
using Microsoft.AspNetCore.Identity;

namespace Properly.Models;

[Flags]
public enum DomainUserType
{
    None = 0,
    Management = 1,
    Owner = 2,
    Tenant = 4,
    Service = 8
}

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Cached domain membership flags
    public DomainUserType DomainTypes { get; set; } = DomainUserType.None;

    // Last used contextual entity IDs (nullable => never used)
    public int? LastManagementUserId { get; set; }
    public int? LastOwnerUserId { get; set; }
    public int? LastTenantId { get; set; }
    public int? LastServiceUserId { get; set; }

    public DateTime? LastDomainContextSwitchUtc { get; set; }


}
