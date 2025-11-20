-- Add ApplicationUser properties to AspNetUsers table
-- This migration adds the custom properties from the ApplicationUser model

USE [ProperlyIdentityDB]
GO

-- Add FirstName column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'FirstName')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [FirstName] [nvarchar](max) NOT NULL DEFAULT ''
    PRINT 'Added FirstName column'
END
GO

-- Add LastName column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'LastName')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [LastName] [nvarchar](max) NOT NULL DEFAULT ''
    PRINT 'Added LastName column'
END
GO

-- Add ProfileImageUrl column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'ProfileImageUrl')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [ProfileImageUrl] [nvarchar](max) NULL
    PRINT 'Added ProfileImageUrl column'
END
GO

-- Add CreatedAt column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'CreatedAt')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE()
    PRINT 'Added CreatedAt column'
END
GO

-- Add LastLoginAt column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'LastLoginAt')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [LastLoginAt] [datetime2](7) NULL
    PRINT 'Added LastLoginAt column'
END
GO

-- Add IsActive column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [IsActive] [bit] NOT NULL DEFAULT 1
    PRINT 'Added IsActive column'
END
GO

PRINT 'Migration completed successfully'
GO
