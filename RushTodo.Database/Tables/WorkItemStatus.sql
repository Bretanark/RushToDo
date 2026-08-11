-- WorkItemStatus table abbreviation: ws
CREATE TABLE dbo.WorkItemStatus (
    WorkItemStatusId INT NOT NULL CONSTRAINT PK_WorkItemStatus PRIMARY KEY CLUSTERED,
    WorkItemStatusName NVARCHAR(255) NOT NULL,

    INDEX UX_WorkItemStatus_WorkItemStatusName UNIQUE (WorkItemStatusName),
)
GO
