-- EntityType table abbreviation: et
CREATE TABLE dbo.EntityType (
    EntityTypeId INT NOT NULL CONSTRAINT PK_EntityType PRIMARY KEY CLUSTERED,
    Name NVARCHAR(255) NOT NULL,
    UpdateDateTime DATETIME2 NOT NULL,

    INDEX UX_EntityType_Name UNIQUE (Name),
)
GO
