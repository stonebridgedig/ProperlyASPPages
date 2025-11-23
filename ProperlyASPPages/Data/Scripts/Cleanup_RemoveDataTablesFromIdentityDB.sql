-- Cleanup Script: Remove data tables from ProperlyIdentityDB
-- These tables were incorrectly created in ProperlyIdentityDB
-- They should only exist in ProperlyDataDB

USE ProperlyIdentityDB;
GO

PRINT 'Starting cleanup of ProperlyIdentityDB...';
GO

-- Drop stored procedures first
IF OBJECT_ID('dbo.sp_GetPropertyUnits', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_GetPropertyUnits;
    PRINT 'Dropped procedure: sp_GetPropertyUnits';
END
GO

IF OBJECT_ID('dbo.sp_GetLeaseTenants', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_GetLeaseTenants;
    PRINT 'Dropped procedure: sp_GetLeaseTenants';
END
GO

IF OBJECT_ID('dbo.sp_GetCompanyProperties', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_GetCompanyProperties;
    PRINT 'Dropped procedure: sp_GetCompanyProperties';
END
GO

-- Drop SystemAdmin tables first (in correct order for foreign keys)
IF OBJECT_ID('dbo.SystemAdminActivityLog', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.SystemAdminActivityLog;
    PRINT 'Dropped table: SystemAdminActivityLog';
END
GO

IF OBJECT_ID('dbo.SystemAdmin', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.SystemAdmin;
    PRINT 'Dropped table: SystemAdmin';
END
GO

IF OBJECT_ID('dbo.SystemAdminRole', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.SystemAdminRole;
    PRINT 'Dropped table: SystemAdminRole';
END
GO

-- Drop tables in correct order (respecting foreign key constraints)
IF OBJECT_ID('dbo.LeaseTenant', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.LeaseTenant;
    PRINT 'Dropped table: LeaseTenant';
END
GO

IF OBJECT_ID('dbo.Lease', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Lease;
    PRINT 'Dropped table: Lease';
END
GO

IF OBJECT_ID('dbo.Tenant', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Tenant;
    PRINT 'Dropped table: Tenant';
END
GO

IF OBJECT_ID('dbo.Unit', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Unit;
    PRINT 'Dropped table: Unit';
END
GO

IF OBJECT_ID('dbo.Building', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Building;
    PRINT 'Dropped table: Building';
END
GO

IF OBJECT_ID('dbo.Property', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Property;
    PRINT 'Dropped table: Property';
END
GO

IF OBJECT_ID('dbo.Vendor', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Vendor;
    PRINT 'Dropped table: Vendor';
END
GO

IF OBJECT_ID('dbo.ServiceUser', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ServiceUser;
    PRINT 'Dropped table: ServiceUser';
END
GO

IF OBJECT_ID('dbo.ServiceOrg', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ServiceOrg;
    PRINT 'Dropped table: ServiceOrg';
END
GO

IF OBJECT_ID('dbo.OwnerUser', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.OwnerUser;
    PRINT 'Dropped table: OwnerUser';
END
GO

IF OBJECT_ID('dbo.OwnerOrg', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.OwnerOrg;
    PRINT 'Dropped table: OwnerOrg';
END
GO

IF OBJECT_ID('dbo.CompanyInvitation', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.CompanyInvitation;
    PRINT 'Dropped table: CompanyInvitation';
END
GO

IF OBJECT_ID('dbo.CompanyUser', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.CompanyUser;
    PRINT 'Dropped table: CompanyUser';
END
GO

IF OBJECT_ID('dbo.CompanyOrg', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.CompanyOrg;
    PRINT 'Dropped table: CompanyOrg';
END
GO

PRINT 'Cleanup completed successfully!';
PRINT 'All business data tables removed from ProperlyIdentityDB.';
PRINT 'These tables should only exist in ProperlyDataDB.';
GO
