-- Script to add mmitev62@yahoo.com as System Administrator
-- This script assumes the user already exists in the Identity database

USE [ProperlyDataDB]
GO

DECLARE @Email NVARCHAR(256) = 'mmitev62@yahoo.com';
DECLARE @FirstName NVARCHAR(100) = 'Mitko';
DECLARE @LastName NVARCHAR(100) = 'Mitev';
DECLARE @IdentityUserId NVARCHAR(450);

-- Get the Identity User ID from the Identity database
SELECT @IdentityUserId = Id 
FROM ProperlyIdentityDB.dbo.AspNetUsers 
WHERE NormalizedEmail = UPPER(@Email);

IF @IdentityUserId IS NULL
BEGIN
    PRINT 'ERROR: User ' + @Email + ' not found in Identity database.';
    PRINT 'Please verify the email address is correct.';
    RETURN;
END

PRINT 'Found user in Identity database';
PRINT 'Identity User ID: ' + @IdentityUserId;
PRINT '';

-- Check if already a System Admin
IF EXISTS (SELECT 1 FROM SystemAdmin WHERE IdentityUserId = @IdentityUserId)
BEGIN
    PRINT 'User is already a System Administrator.';
    PRINT '';
    
    -- Show current admin details
    SELECT 
        sa.SystemAdminId,
        sa.FirstName + ' ' + sa.LastName as Name,
        sa.Email,
        sar.RoleName,
        sa.IsSuperAdmin,
        sa.IsActive,
        sa.CreatedAt
    FROM SystemAdmin sa
    INNER JOIN SystemAdminRole sar ON sa.RoleId = sar.RoleId
    WHERE sa.IdentityUserId = @IdentityUserId;
    
    RETURN;
END

-- Get Super Administrator role (RoleId = 1)
DECLARE @SuperAdminRoleId INT = 1;

-- Verify the role exists
IF NOT EXISTS (SELECT 1 FROM SystemAdminRole WHERE RoleId = @SuperAdminRoleId)
BEGIN
    PRINT 'ERROR: Super Administrator role not found.';
    PRINT 'Please run the 03_CreateSystemAdminTables.sql script first to create the admin tables.';
    RETURN;
END

-- Create System Admin record
INSERT INTO SystemAdmin 
(
    IdentityUserId, 
    RoleId, 
    FirstName, 
    LastName, 
    Email, 
    IsSuperAdmin, 
    IsActive, 
    CreatedAt, 
    HireDate
)
VALUES 
(
    @IdentityUserId, 
    @SuperAdminRoleId, 
    @FirstName, 
    @LastName, 
    @Email, 
    1, -- Super Admin
    1, -- Active
    GETUTCDATE(), 
    GETUTCDATE()
);

PRINT '';
PRINT '? Successfully elevated user to System Administrator!';
PRINT '';
PRINT 'Details:';
PRINT '  Name: ' + @FirstName + ' ' + @LastName;
PRINT '  Email: ' + @Email;
PRINT '  Identity User ID: ' + @IdentityUserId;
PRINT '  Role: Super Administrator';
PRINT '  Super Admin: Yes';
PRINT '  Status: Active';
PRINT '';
PRINT 'You can now access the admin panel at: /Admin';
GO
