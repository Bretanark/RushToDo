MERGE dbo.WorkItemStatus AS target
USING
(
    VALUES
        (1, N'New'),
        (2, N'Scheduled'),
        (3, N'Done'),
        (4, N'Cancelled')
) AS source (WorkItemStatusId, WorkItemStatusName)
    ON target.WorkItemStatusId = source.WorkItemStatusId
WHEN MATCHED AND target.WorkItemStatusName <> source.WorkItemStatusName THEN
    UPDATE SET
        WorkItemStatusName = source.WorkItemStatusName
WHEN NOT MATCHED BY TARGET THEN
    INSERT (WorkItemStatusId, WorkItemStatusName)
    VALUES (source.WorkItemStatusId, source.WorkItemStatusName);
GO
