using System.ComponentModel.DataAnnotations;

namespace Properly.Models;

// Base types (POCOs for Dapper; all nullable per current convention)
public abstract class BaseEntity
{
    [Key]
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }
}

public interface IOrgScoped
{
    int? CompanyOrgId { get; set; }
}

// Financial -------------------------------------------------
public class Charge : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? LeaseId { get; set; }
    public int? TenantId { get; set; }
    public string? Type { get; set; } // Rent, LateFee, Utility, Other
    public decimal? Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? IsPaid { get; set; }
    public string? Notes { get; set; }
}

public class Payment : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? TenantId { get; set; }
    public int? LeaseId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Method { get; set; } // ACH, Card, Check, Cash
    public string? ReferenceNumber { get; set; }
    public bool? IsVoided { get; set; }
}

public class Invoice : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? LeaseId { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public bool? IsPaid { get; set; }
}

public class LedgerEntry : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? LeaseId { get; set; }
    public int? TenantId { get; set; }
    public DateTime? EntryDate { get; set; }
    public string? EntryType { get; set; } // Debit / Credit
    public decimal? Amount { get; set; }
    public string? Description { get; set; }
}

public class ScheduledRentIncrease : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? LeaseId { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public decimal? NewRent { get; set; }
    public bool? Applied { get; set; }
}

public class RentAdjustment : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? LeaseId { get; set; }
    public DateTime? AdjustmentDate { get; set; }
    public decimal? OldRent { get; set; }
    public decimal? NewRent { get; set; }
    public string? Reason { get; set; }
}

// Maintenance ------------------------------------------------
public class MaintenanceRequest : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public int? UnitId { get; set; }
    public int? TenantId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; } // Low, Medium, High, Emergency
    public string? Status { get; set; } // Open, InProgress, Completed, Cancelled
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? VendorId { get; set; }
}

public class WorkOrder : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? MaintenanceRequestId { get; set; }
    public string? Status { get; set; } // Draft, Assigned, InProgress, Done, Closed
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }
}

public class Inspection : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public int? UnitId { get; set; }
    public string? Type { get; set; } // MoveIn, MoveOut, Annual, Safety
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? ResultSummary { get; set; }
}

public class PreventiveSchedule : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? TaskName { get; set; }
    public string? Frequency { get; set; } // Monthly, Quarterly, Annual
    public DateTime? NextDueDate { get; set; }
    public bool? Active { get; set; }
}

// Amenities --------------------------------------------------
public class Amenity : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? Name { get; set; }
    public bool? RequiresReservation { get; set; }
    public string? Notes { get; set; }
}

public class AmenityReservation : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? AmenityId { get; set; }
    public int? TenantId { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string? Status { get; set; } // Pending, Approved, Cancelled
}

public class ParkingSpace : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? SpaceNumber { get; set; }
    public bool? IsOccupied { get; set; }
    public int? TenantId { get; set; }
}

public class StorageUnit : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? UnitNumber { get; set; }
    public bool? IsOccupied { get; set; }
    public int? TenantId { get; set; }
}

// Utilities --------------------------------------------------
public class UtilityMeter : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? MeterType { get; set; } // Water, Electric, Gas
    public string? SerialNumber { get; set; }
}

public class UtilityReading : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? UtilityMeterId { get; set; }
    public DateTime? ReadingDate { get; set; }
    public decimal? Value { get; set; }
}

// Compliance / Insurance -------------------------------------
public class InsurancePolicy : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? PolicyNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class CertificateOfInsurance : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? InsurancePolicyId { get; set; }
    public string? Url { get; set; }
    public DateTime? UploadedAt { get; set; }
}

public class ComplianceItem : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; } // Pending, Compliant, Overdue
    public DateTime? DueDate { get; set; }
}

// Documents --------------------------------------------------
public class Document : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? FileUrl { get; set; }
    public bool? IsPrivate { get; set; }
}

public class DocumentVersion : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? DocumentId { get; set; }
    public int? VersionNumber { get; set; }
    public string? FileUrl { get; set; }
}

public class ESignatureEnvelope : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Provider { get; set; }
    public string? EnvelopeId { get; set; }
    public string? Status { get; set; } // Sent, Completed, Voided
    public DateTime? SentDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}

// Communication ----------------------------------------------
public class Conversation : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Subject { get; set; }
    public bool? IsClosed { get; set; }
}

public class Message : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? ConversationId { get; set; }
    public string? SenderUserId { get; set; }
    public string? Body { get; set; }
    public DateTime? SentAt { get; set; }
}

public class Notification : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? UserId { get; set; }
    public string? Type { get; set; }
    public string? PayloadJson { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class Announcement : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? PublishAt { get; set; }
    public bool? IsActive { get; set; }
}

// Task / Calendar --------------------------------------------
public class TaskItem : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Title { get; set; }
    public string? Status { get; set; } // Open, InProgress, Done
    public DateTime? DueDate { get; set; }
    public string? AssignedToUserId { get; set; }
}

public class Reminder : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? UserId { get; set; }
    public string? Text { get; set; }
    public DateTime? TriggerAt { get; set; }
    public bool? Sent { get; set; }
}

public class CalendarEvent : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Title { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string? Location { get; set; }
}

// Vendor extensions -----------------------------------------
public class VendorContract : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? VendorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Terms { get; set; }
    public decimal? BaseRate { get; set; }
}

public class RateCard : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? VendorId { get; set; }
    public string? ServiceType { get; set; }
    public decimal? HourlyRate { get; set; }
}

// Budget / Forecast / Capital --------------------------------
public class Budget : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public int? Year { get; set; }
    public decimal? PlannedRevenue { get; set; }
    public decimal? PlannedExpense { get; set; }
}

public class Forecast : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public decimal? ExpectedRevenue { get; set; }
    public decimal? ExpectedExpense { get; set; }
}

public class DepreciationSchedule : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? AssetName { get; set; }
    public decimal? Cost { get; set; }
    public int? LifespanYears { get; set; }
    public DateTime? PlacedInServiceDate { get; set; }
}

// Tagging / Custom Fields ------------------------------------
public class Tag : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Name { get; set; }
    public string? ColorHex { get; set; }
}

public class EntityTag : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? EntityType { get; set; } // e.g. Property, Unit, Lease
    public int? EntityId { get; set; }
    public int? TagId { get; set; }
}

public class CustomFieldDefinition : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? EntityType { get; set; }
    public string? Name { get; set; }
    public string? DataType { get; set; } // String, Number, Date, Bool
    public bool? IsRequired { get; set; }
}

public class CustomFieldValue : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? CustomFieldDefinitionId { get; set; }
    public int? EntityId { get; set; }
    public string? Value { get; set; }
}

// Audit / Integration ----------------------------------------
public class AuditLog : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Action { get; set; } // Create, Update, Delete
    public string? UserId { get; set; }
    public string? ChangesJson { get; set; }
}

public class ChangeSet : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? BatchId { get; set; }
    public string? ChangesJson { get; set; }
}

public class WebhookSubscription : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? TargetUrl { get; set; }
    public string? EventType { get; set; }
    public bool? IsActive { get; set; }
    public string? Secret { get; set; }
}

public class IntegrationMapping : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? ExternalSystem { get; set; }
    public string? InternalEntityType { get; set; }
    public int? InternalEntityId { get; set; }
    public string? ExternalId { get; set; }
}

// Platform / SaaS --------------------------------------------
public class FeatureFlag : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Key { get; set; }
    public bool? Enabled { get; set; }
}

public class SubscriptionPlan : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; } // Owning org or null if catalog
    public string? Name { get; set; }
    public decimal? MonthlyPrice { get; set; }
    public int? MaxProperties { get; set; }
    public int? MaxUsers { get; set; }
}

// SLA / Metrics ----------------------------------------------
public class SLAMetric : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public string? Name { get; set; }
    public decimal? TargetValue { get; set; }
    public string? Unit { get; set; } // Hours, Percentage
}

public class PerformanceSnapshot : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public DateTime? SnapshotDate { get; set; }
    public string? MetricName { get; set; }
    public decimal? MetricValue { get; set; }
}

// IoT / Access ------------------------------------------------
public class SensorReading : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? SensorType { get; set; } // Temperature, Humidity, Leak
    public decimal? Value { get; set; }
    public DateTime? ReadingTime { get; set; }
}

public class AccessControlEvent : BaseEntity, IOrgScoped
{
    public int? CompanyOrgId { get; set; }
    public int? PropertyId { get; set; }
    public string? Door { get; set; }
    public string? UserId { get; set; }
    public DateTime? EventTime { get; set; }
    public string? Result { get; set; } // Granted, Denied
}
