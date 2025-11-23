using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Properly.Models;

public class SystemAdminRole
{
    [Key]
    public int RoleId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool CanManageAdmins { get; set; }
    public bool CanManageUsers { get; set; }
    public bool CanManageCompanies { get; set; }
    public bool CanViewReports { get; set; }
    public bool CanManageSystem { get; set; }
    public bool CanAccessBilling { get; set; }
    public bool CanManageSupport { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<SystemAdmin> Admins { get; set; } = new List<SystemAdmin>();
}

public class SystemAdmin
{
    [Key]
    public int SystemAdminId { get; set; }
    
    [Required]
    [MaxLength(450)]
    public string IdentityUserId { get; set; } = string.Empty;
    
    public int RoleId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    [MaxLength(100)]
    public string? Title { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsSuperAdmin { get; set; }
    
    public DateTime? HireDate { get; set; }
    public DateTime? LastAccessAt { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedByAdminId { get; set; }
    
    public SystemAdminRole Role { get; set; } = null!;
    public ApplicationUser IdentityUser { get; set; } = null!;
    
    public ICollection<SystemAdminActivityLog> ActivityLogs { get; set; } = new List<SystemAdminActivityLog>();
    
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}

public class SystemAdminActivityLog
{
    [Key]
    public int LogId { get; set; }
    
    public int SystemAdminId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Activity { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? EntityType { get; set; }
    
    [MaxLength(100)]
    public string? EntityId { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public SystemAdmin Admin { get; set; } = null!;
}

public enum SystemAdminPermission
{
    ManageAdmins,
    ManageUsers,
    ManageCompanies,
    ViewReports,
    ManageSystem,
    AccessBilling,
    ManageSupport
}
