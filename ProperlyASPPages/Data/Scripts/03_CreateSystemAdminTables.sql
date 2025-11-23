-- Create System Administrator tables

IF OBJECT_ID('dbo.SystemAdminActivityLog', 'U') IS NOT NULL
    DROP TABLE dbo.SystemAdminActivityLog;

IF OBJECT_ID('dbo.SystemAdmin', 'U') IS NOT NULL
    DROP TABLE dbo.SystemAdmin;

IF OBJECT_ID('dbo.SystemAdminRole', 'U') IS NOT NULL
    DROP TABLE dbo.SystemAdminRole;

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

-- Insert default roles
INSERT INTO dbo.SystemAdminRole (RoleName, Description, CanManageAdmins, CanManageUsers, CanManageCompanies, CanViewReports, CanManageSystem, CanAccessBilling, CanManageSupport, IsActive)
VALUES 
    ('Super Administrator', 'Full system access with all permissions', 1, 1, 1, 1, 1, 1, 1, 1),
    ('Administrator', 'Standard admin with most permissions', 0, 1, 1, 1, 0, 0, 1, 1),
    ('Support Manager', 'Customer support and user management', 0, 1, 1, 1, 0, 0, 1, 1),
    ('Support Agent', 'Basic customer support', 0, 0, 0, 1, 0, 0, 1, 1),
    ('Billing Manager', 'Billing and financial access', 0, 0, 1, 1, 0, 1, 0, 1),
    ('Developer', 'System configuration and development', 0, 0, 0, 1, 1, 0, 0, 1),
    ('Analyst', 'View-only access for reporting', 0, 0, 0, 1, 0, 0, 0, 1);

PRINT 'System Administrator tables created successfully.';
GO
