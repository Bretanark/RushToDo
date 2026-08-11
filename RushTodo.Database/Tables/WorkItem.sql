-- WorkItem table abbreviation: wi
CREATE TABLE dbo.WorkItem (
    WorkItemId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_WorkItem PRIMARY KEY CLUSTERED,
    Title NVARCHAR(255) NOT NULL,
    Description VARCHAR(MAX) NULL,
    StatusId INT NOT NULL CONSTRAINT FK_WorkItem_Status REFERENCES dbo.WorkItemStatus (WorkItemStatusId),
    Address NVARCHAR(255) NOT NULL,
    GardenerId INT NULL CONSTRAINT FK_WorkItem_Gardener REFERENCES dbo.Gardener (GardenerId),
    ScheduledDate DATE NULL,
    CompletionDate DATE NULL,
    CancellationDate DATE NULL,
    UpdateDateTime DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL,

    INDEX IX_WorkItem_GardenerId (GardenerId),
)
GO
