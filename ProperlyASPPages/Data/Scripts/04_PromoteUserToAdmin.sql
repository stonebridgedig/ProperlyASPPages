-- Helper Script: Promote Existing User to System Administrator
-- 
-- This script helps you promote an existing ApplicationUser to a System Administrator
-- Replace the email address with the user you want to promote

DECLARE @Email NVARCHAR(256) = 'your-email@example.com'; -- CHANGE THIS
DECLARE @FirstName NVARCHAR(100) = 'Your First Name';    -- CHANGE THIS
DECLARE @LastName NVARCHAR(100) = 'Your Last Name';      -- CHANGE THIS
DECLARE @RoleId INT = 1; -- 1 = Super Administrator, see below for other roles
DECLARE @IsSuperAdmin BIT = 1; -- Set to 1 for full access

-- Find the user
DECLARE @IdentityUserId NVARCHAR(450);

SELECT @IdentityUserId = Id 
FROM AspNetUsers 
WHERE Email = @Email;

IF @IdentityUserId IS NULL
BEGIN
    PRINT 'ERROR: User with email ' + @Email + ' not found in AspNetUsers table.';
    PRINT 'Please create the user account first through the Register page.';
    RETURN;
END

-- Check if already an admin
IF EXISTS (SELECT 1 FROM SystemAdmin WHERE IdentityUserId = @IdentityUserId)
BEGIN
    PRINT 'User is already a System Administrator.';
    
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

-- Create System Admin record
INSERT INTO SystemAdmin 
(IdentityUserId, RoleId, FirstName, LastName, Email, IsSuperAdmin, IsActive, CreatedAt, HireDate)
VALUES 
(@IdentityUserId, @RoleId, @FirstName, @LastName, @Email, @IsSuperAdmin, 1, GETUTCDATE(), GETUTCDATE());

PRINT 'Successfully created System Administrator account!';
PRINT '';
PRINT 'Details:';
PRINT '  Name: ' + @FirstName + ' ' + @LastName;
PRINT '  Email: ' + @Email;
PRINT '  Identity User ID: ' + @IdentityUserId;
PRINT '  Super Admin: ' + CASE WHEN @IsSuperAdmin = 1 THEN 'Yes' ELSE 'No' END;
PRINT '';
PRINT 'You can now access the admin panel at: /Admin';

GO

-- REFERENCE: Available System Admin Roles
-- Run this to see all available roles:
/*
SELECT 
    RoleId,
    RoleName,
    Description,
    CanManageAdmins,
    CanManageUsers,
    CanManageCompanies,
    CanViewReports,
    CanManageSystem,
    CanAccessBilling,
    CanManageSupport
FROM SystemAdminRole
WHERE IsActive = 1
ORDER BY RoleId;
*/

-- ROLE IDs:
-- 1 = Super Administrator (Full access)
-- 2 = Administrator (Standard admin)
-- 3 = Support Manager (User & company management)
-- 4 = Support Agent (Basic support)
-- 5 = Billing Manager (Billing access)
-- 6 = Developer (System configuration)
-- 7 = Analyst (Read-only reports)
