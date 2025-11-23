using System.ComponentModel.DataAnnotations;

namespace Properly.Models;

public class ServiceOrg
{
    [Key]
    public int? ServiceOrgId { get; set; }

    public string? Name { get; set; }
    public string? LegalName { get; set; }
    public string? TaxId { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public bool? IsActive { get; set; }
    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public byte[]? RowVersion { get; set; }
}

public class ServiceUser
{
    [Key]
    public int? ServiceUserId { get; set; }
    public int? ServiceOrgId { get; set; }

    public string? IdentityUserId { get; set; }

    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Title { get; set; }
    public string? Role { get; set; }
    public string? Phone { get; set; }
    public string? AlternatePhone { get; set; }

    public bool? IsPrimaryContact { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public byte[]? RowVersion { get; set; }
}

