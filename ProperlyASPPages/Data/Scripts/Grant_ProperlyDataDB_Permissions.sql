-- Grant permissions to ProperlyAdmin user on ProperlyDataDB
-- This script grants SELECT, INSERT, UPDATE, DELETE permissions on all tables

USE ProperlyDataDB;
GO

PRINT 'Granting permissions to ProperlyAdmin...';
GO

-- Grant database level permissions
GRANT CONNECT TO ProperlyAdmin;
GO

-- Grant permissions on all tables
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.SystemAdminRole TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.SystemAdmin TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.SystemAdminActivityLog TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.CompanyOrg TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.CompanyUser TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.CompanyInvitation TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.OwnerOrg TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.OwnerUser TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.ServiceOrg TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.ServiceUser TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Vendor TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Property TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Building TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Unit TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Tenant TO ProperlyAdmin;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Lease TO ProperlyAdmin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.LeaseTenant TO ProperlyAdmin;

-- Grant EXECUTE permission on stored procedures
GRANT EXECUTE ON dbo.sp_GetCompanyProperties TO ProperlyAdmin;
GRANT EXECUTE ON dbo.sp_GetLeaseTenants TO ProperlyAdmin;
GRANT EXECUTE ON dbo.sp_GetPropertyUnits TO ProperlyAdmin;

PRINT 'Permissions granted successfully!';
PRINT 'ProperlyAdmin can now SELECT, INSERT, UPDATE, DELETE on all tables';
PRINT 'ProperlyAdmin can EXECUTE all stored procedures';
GO
