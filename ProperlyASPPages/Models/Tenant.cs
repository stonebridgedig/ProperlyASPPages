using System.ComponentModel.DataAnnotations;

namespace Properly.Models;

public class Tenant
{
    [Key]
    public int? TenantId { get; set; }

    public string? IdentityUserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Title { get; set; }

    public string? Phone { get; set; }
    public string? AlternatePhone { get; set; }
    public string? PreferredContactMethod { get; set; }
    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public byte[]? RowVersion { get; set; }
}
