-- Setup Script: Create all business tables in ProperlyDataDB
-- This includes Company, Owner, Service, Property, Tenant, Lease, Vendor, and SystemAdmin tables

USE ProperlyDataDB;
GO

PRINT 'Starting ProperlyDataDB setup...';
GO

-- ============================================================================
-- DROP EXISTING TABLES IN CORRECT ORDER (child to parent)
-- ============================================================================

-- Drop child tables first
IF OBJECT_ID('dbo.LeaseTenant', 'U') IS NOT NULL DROP TABLE dbo.LeaseTenant;
IF OBJECT_ID('dbo.Lease', 'U') IS NOT NULL DROP TABLE dbo.Lease;
IF OBJECT_ID('dbo.Unit', 'U') IS NOT NULL DROP TABLE dbo.Unit;
IF OBJECT_ID('dbo.Building', 'U') IS NOT NULL DROP TABLE dbo.Building;
IF OBJECT_ID('dbo.Property', 'U') IS NOT NULL DROP TABLE dbo.Property;
IF OBJECT_ID('dbo.Tenant', 'U') IS NOT NULL DROP TABLE dbo.Tenant;
IF OBJECT_ID('dbo.Vendor', 'U') IS NOT NULL DROP TABLE dbo.Vendor;
IF OBJECT_ID('dbo.ServiceUser', 'U') IS NOT NULL DROP TABLE dbo.ServiceUser;
IF OBJECT_ID('dbo.ServiceOrg', 'U') IS NOT NULL DROP TABLE dbo.ServiceOrg;
IF OBJECT_ID('dbo.OwnerUser', 'U') IS NOT NULL DROP TABLE dbo.OwnerUser;
IF OBJECT_ID('dbo.OwnerOrg', 'U') IS NOT NULL DROP TABLE dbo.OwnerOrg;
IF OBJECT_ID('dbo.CompanyInvitation', 'U') IS NOT NULL DROP TABLE dbo.CompanyInvitation;
IF OBJECT_ID('dbo.CompanyUser', 'U') IS NOT NULL DROP TABLE dbo.CompanyUser;
IF OBJECT_ID('dbo.CompanyOrg', 'U') IS NOT NULL DROP TABLE dbo.CompanyOrg;
IF OBJECT_ID('dbo.SystemAdminActivityLog', 'U') IS NOT NULL DROP TABLE dbo.SystemAdminActivityLog;
IF OBJECT_ID('dbo.SystemAdmin', 'U') IS NOT NULL DROP TABLE dbo.SystemAdmin;
IF OBJECT_ID('dbo.SystemAdminRole', 'U') IS NOT NULL DROP TABLE dbo.SystemAdminRole;

PRINT 'Existing tables dropped.';
GO

-- ============================================================================
-- SYSTEM ADMIN TABLES
-- ============================================================================

-- System Admin Roles
CREATE TABLE dbo.SystemAdminRole
(
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    
    -- Permissions
    CanManageAdmins BIT DEFAULT 0,
    CanManageUsers BIT DEFAULT 0,
    CanManageCompanies BIT DEFAULT 0,
    CanViewReports BIT DEFAULT 0,
    CanManageSystem BIT DEFAULT 0,
    CanAccessBilling BIT DEFAULT 0,
    CanManageSupport BIT DEFAULT 0,
    
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    
    INDEX IX_SystemAdminRole_RoleName (RoleName),
    INDEX IX_SystemAdminRole_IsActive (IsActive)
);

-- System Administrators
CREATE TABLE dbo.SystemAdmin
(
    SystemAdminId INT PRIMARY KEY IDENTITY(1,1),
    IdentityUserId NVARCHAR(450) NOT NULL,
    RoleId INT NOT NULL,
    
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20),
    
    Department NVARCHAR(100),
    Title NVARCHAR(100),
    
    IsActive BIT DEFAULT 1,
    IsSuperAdmin BIT DEFAULT 0,
    
    HireDate DATETIME2,
    LastAccessAt DATETIME2,
    
    Notes NVARCHAR(MAX),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CreatedByAdminId INT,
    
    CONSTRAINT FK_SystemAdmin_Role FOREIGN KEY (RoleId)
        REFERENCES dbo.SystemAdminRole(RoleId),
    
    CONSTRAINT UQ_SystemAdmin_IdentityUserId UNIQUE (IdentityUserId),
    CONSTRAINT UQ_SystemAdmin_Email UNIQUE (Email),
    
    INDEX IX_SystemAdmin_IdentityUserId (IdentityUserId),
    INDEX IX_SystemAdmin_Email (Email),
    INDEX IX_SystemAdmin_IsActive (IsActive),
    INDEX IX_SystemAdmin_RoleId (RoleId)
);

-- Activity Log
CREATE TABLE dbo.SystemAdminActivityLog
(
    LogId INT PRIMARY KEY IDENTITY(1,1),
    SystemAdminId INT NOT NULL,
    
    Activity NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    EntityType NVARCHAR(100),
    EntityId NVARCHAR(100),
    
    IpAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    
    Timestamp DATETIME2 DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_SystemAdminActivityLog_Admin FOREIGN KEY (SystemAdminId)
        REFERENCES dbo.SystemAdmin(SystemAdminId),
    
    INDEX IX_SystemAdminActivityLog_AdminId (SystemAdminId),
    INDEX IX_SystemAdminActivityLog_Timestamp (Timestamp),
    INDEX IX_SystemAdminActivityLog_EntityType (EntityType)
);

PRINT 'SystemAdmin tables created.';
GO

-- ============================================================================
-- COMPANY ORGANIZATION TABLES
-- ============================================================================

-- CompanyOrg table
CREATE TABLE dbo.CompanyOrg
(
    CompanyOrgId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    LegalName NVARCHAR(200),
    TaxId NVARCHAR(50),
    
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100),
    CountryCode NVARCHAR(3),
    
    Phone NVARCHAR(20),
    Email NVARCHAR(256),
    Website NVARCHAR(500),
    
    IsActive BIT DEFAULT 1,
    Notes NVARCHAR(MAX),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    INDEX IX_CompanyOrg_Name (Name),
    INDEX IX_CompanyOrg_IsActive (IsActive),
    INDEX IX_CompanyOrg_Email (Email)
);

-- CompanyUser table
CREATE TABLE dbo.CompanyUser
(
    CompanyUserId INT PRIMARY KEY IDENTITY(1,1),
    CompanyOrgId INT NOT NULL,
    
    IdentityUserId NVARCHAR(450),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Title NVARCHAR(100),
    Phone NVARCHAR(20),
    AlternatePhone NVARCHAR(20),
    
    Role NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    LastLoginAt DATETIME2,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_CompanyUser_CompanyOrg FOREIGN KEY (CompanyOrgId)
        REFERENCES dbo.CompanyOrg(CompanyOrgId),
    
    INDEX IX_CompanyUser_CompanyOrgId (CompanyOrgId),
    INDEX IX_CompanyUser_IdentityUserId (IdentityUserId),
    INDEX IX_CompanyUser_Email (Email),
    INDEX IX_CompanyUser_IsActive (IsActive)
);

-- CompanyInvitation table
CREATE TABLE dbo.CompanyInvitation
(
    InvitationId INT PRIMARY KEY IDENTITY(1,1),
    CompanyOrgId INT NOT NULL,
    
    Email NVARCHAR(256) NOT NULL,
    InvitedByUserId NVARCHAR(450) NOT NULL,
    InvitationToken NVARCHAR(100) NOT NULL,
    
    Status INT DEFAULT 0, -- 0=Pending, 1=Accepted, 2=Declined, 3=Expired, 4=Cancelled
    Role NVARCHAR(50),
    InvitedFullName NVARCHAR(200),
    
    ExpiresAt DATETIME2 NOT NULL,
    AcceptedAt DATETIME2,
    AcceptedByUserId NVARCHAR(450),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    
    CONSTRAINT FK_CompanyInvitation_CompanyOrg FOREIGN KEY (CompanyOrgId)
        REFERENCES dbo.CompanyOrg(CompanyOrgId),
    
    CONSTRAINT UQ_CompanyInvitation_Token UNIQUE (InvitationToken),
    
    INDEX IX_CompanyInvitation_CompanyOrgId (CompanyOrgId),
    INDEX IX_CompanyInvitation_Email (Email),
    INDEX IX_CompanyInvitation_Status (Status),
    INDEX IX_CompanyInvitation_Token (InvitationToken),
    INDEX IX_CompanyInvitation_ExpiresAt (ExpiresAt)
);

PRINT 'Company tables created.';
GO

-- ============================================================================
-- OWNER ORGANIZATION TABLES
-- ============================================================================

-- OwnerOrg table
CREATE TABLE dbo.OwnerOrg
(
    OwnerOrgId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    LegalName NVARCHAR(200),
    TaxId NVARCHAR(50),
    
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100),
    CountryCode NVARCHAR(3),
    
    Phone NVARCHAR(20),
    Email NVARCHAR(256),
    
    IsActive BIT DEFAULT 1,
    Notes NVARCHAR(MAX),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    INDEX IX_OwnerOrg_Name (Name),
    INDEX IX_OwnerOrg_IsActive (IsActive),
    INDEX IX_OwnerOrg_Email (Email)
);

-- OwnerUser table
CREATE TABLE dbo.OwnerUser
(
    OwnerUserId INT PRIMARY KEY IDENTITY(1,1),
    OwnerOrgId INT NOT NULL,
    
    IdentityUserId NVARCHAR(450),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Title NVARCHAR(100),
    Phone NVARCHAR(20),
    AlternatePhone NVARCHAR(20),
    
    IsPrimaryContact BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_OwnerUser_OwnerOrg FOREIGN KEY (OwnerOrgId)
        REFERENCES dbo.OwnerOrg(OwnerOrgId),
    
    INDEX IX_OwnerUser_OwnerOrgId (OwnerOrgId),
    INDEX IX_OwnerUser_IdentityUserId (IdentityUserId),
    INDEX IX_OwnerUser_Email (Email),
    INDEX IX_OwnerUser_IsActive (IsActive)
);

PRINT 'Owner tables created.';
GO

-- ============================================================================
-- SERVICE ORGANIZATION TABLES
-- ============================================================================

-- ServiceOrg table
CREATE TABLE dbo.ServiceOrg
(
    ServiceOrgId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    LegalName NVARCHAR(200),
    TaxId NVARCHAR(50),
    
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100),
    CountryCode NVARCHAR(3),
    
    Phone NVARCHAR(20),
    Email NVARCHAR(256),
    Website NVARCHAR(500),
    
    IsActive BIT DEFAULT 1,
    Notes NVARCHAR(MAX),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    INDEX IX_ServiceOrg_Name (Name),
    INDEX IX_ServiceOrg_IsActive (IsActive),
    INDEX IX_ServiceOrg_Email (Email)
);

-- ServiceUser table
CREATE TABLE dbo.ServiceUser
(
    ServiceUserId INT PRIMARY KEY IDENTITY(1,1),
    ServiceOrgId INT NOT NULL,
    
    IdentityUserId NVARCHAR(450),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Title NVARCHAR(100),
    Role NVARCHAR(50),
    Phone NVARCHAR(20),
    AlternatePhone NVARCHAR(20),
    
    IsPrimaryContact BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    LastLoginAt DATETIME2,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_ServiceUser_ServiceOrg FOREIGN KEY (ServiceOrgId)
        REFERENCES dbo.ServiceOrg(ServiceOrgId),
    
    INDEX IX_ServiceUser_ServiceOrgId (ServiceOrgId),
    INDEX IX_ServiceUser_IdentityUserId (IdentityUserId),
    INDEX IX_ServiceUser_Email (Email),
    INDEX IX_ServiceUser_IsActive (IsActive)
);

PRINT 'Service tables created.';
GO

-- ============================================================================
-- VENDOR TABLES
-- ============================================================================

-- Vendor table
CREATE TABLE dbo.Vendor
(
    VendorId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    CompanyName NVARCHAR(100),
    Email NVARCHAR(256),
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    Specialty NVARCHAR(100),
    HourlyRate DECIMAL(10, 2),
    Status INT DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended
    Notes NVARCHAR(MAX),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    INDEX IX_Vendor_Name (Name),
    INDEX IX_Vendor_Status (Status),
    INDEX IX_Vendor_Email (Email),
    INDEX IX_Vendor_Specialty (Specialty)
);

PRINT 'Vendor table created.';
GO

-- ============================================================================
-- PROPERTY TABLES
-- ============================================================================

-- Property table
CREATE TABLE dbo.Property
(
    PropertyId INT PRIMARY KEY IDENTITY(1,1),
    OwnerOrgId INT NOT NULL,
    CompanyOrgId INT NOT NULL,
    
    Name NVARCHAR(200) NOT NULL,
    Type NVARCHAR(50),
    
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100),
    CountryCode NVARCHAR(3),
    
    Latitude FLOAT,
    Longitude FLOAT,
    TimeZoneId NVARCHAR(50),
    
    UnitsCount INT,
    SquareFeet INT,
    Status INT DEFAULT 0, -- 0=Active, 1=Inactive, 2=UnderRenovation, 3=Planned, 4=Sold
    AcquisitionDate DATETIME2,
    
    Notes NVARCHAR(MAX),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Property_OwnerOrg FOREIGN KEY (OwnerOrgId)
        REFERENCES dbo.OwnerOrg(OwnerOrgId),
    CONSTRAINT FK_Property_CompanyOrg FOREIGN KEY (CompanyOrgId)
        REFERENCES dbo.CompanyOrg(CompanyOrgId),
    
    INDEX IX_Property_OwnerOrgId (OwnerOrgId),
    INDEX IX_Property_CompanyOrgId (CompanyOrgId),
    INDEX IX_Property_Name (Name),
    INDEX IX_Property_Status (Status),
    INDEX IX_Property_Location (City, State)
);

-- Building table
CREATE TABLE dbo.Building
(
    BuildingId INT PRIMARY KEY IDENTITY(1,1),
    PropertyId INT NOT NULL,
    
    Name NVARCHAR(200),
    Code NVARCHAR(50),
    
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100),
    CountryCode NVARCHAR(3),
    
    Latitude FLOAT,
    Longitude FLOAT,
    
    Floors INT,
    YearBuilt INT,
    UnitsCount INT,
    Status INT DEFAULT 0, -- 0=Active, 1=Inactive, 2=UnderRenovation
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Building_Property FOREIGN KEY (PropertyId)
        REFERENCES dbo.Property(PropertyId),
    
    INDEX IX_Building_PropertyId (PropertyId),
    INDEX IX_Building_Name (Name),
    INDEX IX_Building_Status (Status)
);

-- Unit table
CREATE TABLE dbo.Unit
(
    UnitId INT PRIMARY KEY IDENTITY(1,1),
    PropertyId INT NOT NULL,
    BuildingId INT,
    
    UnitNumber NVARCHAR(50) NOT NULL,
    Floor INT,
    
    Bedrooms INT,
    Bathrooms DECIMAL(3, 1),
    SquareFeet INT,
    
    Status INT DEFAULT 0, -- 0=Vacant, 1=Occupied, 2=Turnover, 3=Inactive
    IsOccupied BIT DEFAULT 0,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Unit_Property FOREIGN KEY (PropertyId)
        REFERENCES dbo.Property(PropertyId),
    CONSTRAINT FK_Unit_Building FOREIGN KEY (BuildingId)
        REFERENCES dbo.Building(BuildingId),
    
    INDEX IX_Unit_PropertyId (PropertyId),
    INDEX IX_Unit_BuildingId (BuildingId),
    INDEX IX_Unit_UnitNumber (UnitNumber),
    INDEX IX_Unit_Status (Status),
    INDEX IX_Unit_IsOccupied (IsOccupied)
);

PRINT 'Property tables created.';
GO

-- ============================================================================
-- TENANT TABLES
-- ============================================================================

-- Tenant table
CREATE TABLE dbo.Tenant
(
    TenantId INT PRIMARY KEY IDENTITY(1,1),
    
    IdentityUserId NVARCHAR(450),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Title NVARCHAR(100),
    
    Phone NVARCHAR(20),
    AlternatePhone NVARCHAR(20),
    PreferredContactMethod NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    INDEX IX_Tenant_IdentityUserId (IdentityUserId),
    INDEX IX_Tenant_Email (Email),
    INDEX IX_Tenant_FullName (FullName),
    INDEX IX_Tenant_IsActive (IsActive)
);

PRINT 'Tenant table created.';
GO

-- ============================================================================
-- LEASE TABLES
-- ============================================================================

-- Lease table
CREATE TABLE dbo.Lease
(
    LeaseId INT PRIMARY KEY IDENTITY(1,1),
    PropertyId INT NOT NULL,
    UnitId INT NOT NULL,
    
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2,
    MonthlyRent DECIMAL(10, 2),
    DepositAmount DECIMAL(10, 2),
    LateFee DECIMAL(10, 2),
    GracePeriodDays TINYINT,
    BillingDayOfMonth TINYINT,
    AutoRenew BIT DEFAULT 0,
    
    IsActive BIT DEFAULT 1,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Lease_Property FOREIGN KEY (PropertyId)
        REFERENCES dbo.Property(PropertyId),
    CONSTRAINT FK_Lease_Unit FOREIGN KEY (UnitId)
        REFERENCES dbo.Unit(UnitId),
    
    INDEX IX_Lease_PropertyId (PropertyId),
    INDEX IX_Lease_UnitId (UnitId),
    INDEX IX_Lease_IsActive (IsActive),
    INDEX IX_Lease_StartDate (StartDate),
    INDEX IX_Lease_EndDate (EndDate)
);

-- LeaseTenant table (join table for many-to-many relationship)
CREATE TABLE dbo.LeaseTenant
(
    LeaseId INT NOT NULL,
    TenantId INT NOT NULL,
    AddedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    PRIMARY KEY (LeaseId, TenantId),
    
    CONSTRAINT FK_LeaseTenant_Lease FOREIGN KEY (LeaseId)
        REFERENCES dbo.Lease(LeaseId) ON DELETE CASCADE,
    CONSTRAINT FK_LeaseTenant_Tenant FOREIGN KEY (TenantId)
        REFERENCES dbo.Tenant(TenantId) ON DELETE CASCADE,
    
    INDEX IX_LeaseTenant_TenantId (TenantId)
);

PRINT 'Lease tables created.';
GO

-- ============================================================================
-- INSERT DEFAULT DATA
-- ============================================================================

-- Insert default SystemAdmin roles
INSERT INTO dbo.SystemAdminRole (RoleName, Description, CanManageAdmins, CanManageUsers, CanManageCompanies, CanViewReports, CanManageSystem, CanAccessBilling, CanManageSupport, IsActive)
VALUES 
    ('Super Administrator', 'Full system access with all permissions', 1, 1, 1, 1, 1, 1, 1, 1),
    ('Administrator', 'Standard admin with most permissions', 0, 1, 1, 1, 0, 0, 1, 1),
    ('Support Manager', 'Customer support and user management', 0, 1, 1, 1, 0, 0, 1, 1),
    ('Support Agent', 'Basic customer support', 0, 0, 0, 1, 0, 0, 1, 1),
    ('Billing Manager', 'Billing and financial access', 0, 0, 1, 1, 0, 1, 0, 1),
    ('Developer', 'System configuration and development', 0, 0, 0, 1, 1, 0, 0, 1),
    ('Analyst', 'View-only access for reporting', 0, 0, 0, 1, 0, 0, 0, 1);

PRINT 'Default SystemAdmin roles inserted.';
GO

-- ============================================================================
-- CREATE STORED PROCEDURES
-- ============================================================================

-- Procedure to get active properties for a company
IF OBJECT_ID('dbo.sp_GetCompanyProperties', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetCompanyProperties;
GO

CREATE PROCEDURE dbo.sp_GetCompanyProperties
    @CompanyOrgId INT
AS
BEGIN
    SELECT * FROM dbo.Property
    WHERE CompanyOrgId = @CompanyOrgId
    AND Status = 0 -- Active
    ORDER BY Name;
END;
GO

-- Procedure to get lease tenants
IF OBJECT_ID('dbo.sp_GetLeaseTenants', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetLeaseTenants;
GO

CREATE PROCEDURE dbo.sp_GetLeaseTenants
    @LeaseId INT
AS
BEGIN
    SELECT t.* FROM dbo.Tenant t
    INNER JOIN dbo.LeaseTenant lt ON t.TenantId = lt.TenantId
    WHERE lt.LeaseId = @LeaseId;
END;
GO

-- Procedure to get active units in a property
IF OBJECT_ID('dbo.sp_GetPropertyUnits', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetPropertyUnits;
GO

CREATE PROCEDURE dbo.sp_GetPropertyUnits
    @PropertyId INT
AS
BEGIN
    SELECT * FROM dbo.Unit
    WHERE PropertyId = @PropertyId
    ORDER BY UnitNumber;
END;
GO

PRINT 'Stored procedures created.';
GO

PRINT '========================================';
PRINT 'ProperlyDataDB setup completed successfully!';
PRINT '========================================';
PRINT 'Created:';
PRINT '  - 3 SystemAdmin tables';
PRINT '  - 3 Company tables (CompanyOrg, CompanyUser, CompanyInvitation)';
PRINT '  - 2 Owner tables (OwnerOrg, OwnerUser)';
PRINT '  - 2 Service tables (ServiceOrg, ServiceUser)';
PRINT '  - 1 Vendor table';
PRINT '  - 3 Property tables (Property, Building, Unit)';
PRINT '  - 1 Tenant table';
PRINT '  - 2 Lease tables (Lease, LeaseTenant)';
PRINT '  - 3 Stored procedures';
PRINT '  - 7 Default SystemAdmin roles';
PRINT '========================================';
GO
