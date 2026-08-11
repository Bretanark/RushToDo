MERGE dbo.EntityType AS target
USING
(
    VALUES
        (1, N'AppUser'),
        (2, N'Gardener'),
        (3, N'WorkItem')
) AS source (EntityTypeId, EntityTypeName)
    ON target.EntityTypeId = source.EntityTypeId
WHEN MATCHED AND target.EntityTypeName <> source.EntityTypeName THEN
    UPDATE SET
        EntityTypeName = source.EntityTypeName
WHEN NOT MATCHED BY TARGET THEN
    INSERT (EntityTypeId, EntityTypeName)
    VALUES (source.EntityTypeId, source.EntityTypeName);
GO
