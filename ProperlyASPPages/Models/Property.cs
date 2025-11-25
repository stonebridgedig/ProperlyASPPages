using System.ComponentModel.DataAnnotations;

namespace Properly.Models;

public enum PropertyStatus
{
    Active,
    Inactive,
    UnderRenovation,
    Planned,
    Sold
}

public enum BuildingStatus
{
    Active,
    Inactive,
    UnderRenovation
}

public enum UnitStatus
{
    Vacant,
    Occupied,
    Turnover,
    Inactive
}

public class Property
{
    [Key]
    public int? PropertyId { get; set; }

    public int? OwnerOrgId { get; set; }
    public int? ManagementOrgId { get; set; }

    public string? Name { get; set; }
    public string? Type { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? TimeZoneId { get; set; }

    public int? UnitsCount { get; set; }
    public int? SquareFeet { get; set; }
    public PropertyStatus? Status { get; set; }
    public DateTime? AcquisitionDate { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public byte[]? RowVersion { get; set; }
}

public class Building
{
    [Key]
    public int? BuildingId { get; set; }
    public int? PropertyId { get; set; }

    public string? Name { get; set; }
    public string? Code { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int? Floors { get; set; }
    public int? YearBuilt { get; set; }

    public int? UnitsCount { get; set; }
    public BuildingStatus? Status { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }
}

public class Unit
{
    [Key]
    public int? UnitId { get; set; }
    public int? PropertyId { get; set; }
    public int? BuildingId { get; set; }

    public string? UnitNumber { get; set; }
    public int? Floor { get; set; }

    public int? Bedrooms { get; set; }
    public decimal? Bathrooms { get; set; }
    public int? SquareFeet { get; set; }

    public UnitStatus? Status { get; set; }
    public bool? IsOccupied { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }
}

