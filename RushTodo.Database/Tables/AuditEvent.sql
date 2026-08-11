-- AuditEvent table abbreviation: ae
CREATE TABLE dbo.AuditEvent (
    AuditEventId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_AuditEvent PRIMARY KEY NONCLUSTERED,
    EntityTypeId INT NOT NULL CONSTRAINT FK_AuditEvent_EntityType REFERENCES dbo.EntityType (EntityTypeId),
    EntityId INT NOT NULL,
    AppUserId INT NOT NULL CONSTRAINT FK_AuditEvent_AppUser REFERENCES dbo.AppUser (AppUserId),
    Description NVARCHAR(255) NOT NULL,
    UpdateDateTime DATETIME2 NOT NULL,

    INDEX CX_AuditEvent CLUSTERED (UpdateDateTime, EntityTypeId, EntityId, AuditEventId),
    INDEX UX_AuditEvent_Entity UNIQUE (EntityTypeId, EntityId, UpdateDateTime, AuditEventId),
)
GO
