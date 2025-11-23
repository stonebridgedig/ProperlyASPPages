-- Add CompanyInvitation table to existing schema

IF OBJECT_ID('dbo.CompanyInvitation', 'U') IS NOT NULL
    DROP TABLE dbo.CompanyInvitation;

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

PRINT 'CompanyInvitation table created successfully.';
GO
