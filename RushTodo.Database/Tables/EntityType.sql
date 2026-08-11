-- EntityType table abbreviation: et
CREATE TABLE dbo.EntityType (
    EntityTypeId INT NOT NULL CONSTRAINT PK_EntityType PRIMARY KEY CLUSTERED,
    EntityTypeName NVARCHAR(255) NOT NULL,

    INDEX UX_EntityType_EntityTypeName UNIQUE (EntityTypeName),
)
GO
