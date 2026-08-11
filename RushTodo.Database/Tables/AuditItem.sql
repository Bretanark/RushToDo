-- AuditItem table abbreviation: ai
CREATE TABLE dbo.AuditItem (
    AuditItemId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_AuditItem PRIMARY KEY NONCLUSTERED,
    AuditEventId INT NOT NULL CONSTRAINT FK_AuditItem_AuditEvent REFERENCES dbo.AuditEvent (AuditEventId),
    PropertyName NVARCHAR(255) NOT NULL,
    OldValue NVARCHAR(MAX) NULL,
    NewValue NVARCHAR(MAX) NULL,
    UpdateDateTime DATETIME2 NOT NULL,

    INDEX CX_AuditItem CLUSTERED (UpdateDateTime, AuditEventId, AuditItemId),
    INDEX UX_AuditItem_Event UNIQUE (AuditEventId, AuditItemId),
)
GO
