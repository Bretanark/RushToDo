MERGE dbo.EntityType AS target
USING
(
    VALUES
        (1, N'AppUser'),
        (2, N'Gardener'),
        (3, N'Todo')
) AS source (EntityTypeId, Name)
    ON target.EntityTypeId = source.EntityTypeId
WHEN MATCHED AND target.Name <> source.Name THEN
    UPDATE SET
        Name = source.Name,
        UpdateDateTime = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (EntityTypeId, Name, UpdateDateTime)
    VALUES (source.EntityTypeId, source.Name, SYSUTCDATETIME());
GO
