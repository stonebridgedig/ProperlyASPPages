using System.ComponentModel.DataAnnotations;

namespace Properly.Models;

public enum InvitationStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2,
    Expired = 3,
    Cancelled = 4
}

public class CompanyInvitation
{
    [Key]
    public int? InvitationId { get; set; }
    
    public int? CompanyOrgId { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? InvitedByUserId { get; set; }
    
    public string? InvitationToken { get; set; }
    
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    
    public string? Role { get; set; }
    
    public string? InvitedFullName { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    
    public DateTime? AcceptedAt { get; set; }
    
    public string? AcceptedByUserId { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
