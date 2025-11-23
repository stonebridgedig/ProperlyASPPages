using System.ComponentModel.DataAnnotations;

namespace Properly.Models;

public class Lease
{
    [Key]
    public int? LeaseId { get; set; }
    public int? PropertyId { get; set; } // Redundant but useful for queries
    public int? UnitId { get; set; } // Primary target of the lease

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? LateFee { get; set; }
    public byte? GracePeriodDays { get; set; } // days after due date before late fee
    public byte? BillingDayOfMonth { get; set; } // 1..28 typical cycle
    public bool? AutoRenew { get; set; } = false;

    public bool? IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public byte[]? RowVersion { get; set; }
}

public class LeaseTenant
{
    [Key]
    public int? LeaseId { get; set; }
    [Key]
    public int? TenantId { get; set; }
    public DateTime? AddedAt { get; set; } = DateTime.UtcNow;
}

