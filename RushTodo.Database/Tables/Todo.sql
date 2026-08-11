-- Todo table abbreviation: td
CREATE TABLE dbo.Todo (
    TodoId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Todo PRIMARY KEY CLUSTERED,
    Title NVARCHAR(255) NOT NULL,
    Description VARCHAR(MAX) NULL,
    Status INT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    GardenerId INT NOT NULL CONSTRAINT FK_Todo_Gardener REFERENCES dbo.Gardener (GardenerId),
    ScheduledDate DATE NULL,
    CompletionDate DATE NULL,
    CancellationDate DATE NULL,
    UpdateDateTime DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL,

    INDEX IX_Todo_GardenerId (GardenerId),
)
GO
