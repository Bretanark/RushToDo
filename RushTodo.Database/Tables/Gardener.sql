-- Gardener table abbreviation: gd
CREATE TABLE dbo.Gardener (
    GardenerId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Gardener PRIMARY KEY CLUSTERED,
    Name NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(50) NOT NULL,
    EmailAddress NVARCHAR(255) NULL,
    UpdateDateTime DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL,
)
GO
