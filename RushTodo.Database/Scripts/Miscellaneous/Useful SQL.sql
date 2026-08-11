-- Set the recent-change window used by the diagnostic queries below.
DECLARE @Since DATETIME2 = DATEADD(HOUR, -1, SYSUTCDATETIME())

-- Show recently changed gardeners.
SELECT * FROM dbo.Gardener gd WHERE gd.UpdateDateTime > @Since ORDER BY gd.UpdateDateTime DESC

-- Show work items with their status and optional gardener names.
SELECT
    wi.WorkItemId,
    wi.Title,
    ws.WorkItemStatusName,
    gd.Name GardenerName,
    wi.Address,
    wi.ScheduledDate,
    wi.CompletionDate,
    wi.CancellationDate,
    wi.IsDeleted,
    wi.UpdateDateTime
FROM dbo.WorkItem wi
JOIN dbo.WorkItemStatus ws ON ws.WorkItemStatusId = wi.StatusId
LEFT JOIN dbo.Gardener gd ON gd.GardenerId = wi.GardenerId
WHERE wi.UpdateDateTime > @Since
ORDER BY wi.ScheduledDate, wi.WorkItemId


-- Show audit events and their property changes.
SELECT
    ae.AuditEventId,
    et.EntityTypeName,
    ae.EntityId,
    au.Name AppUserName,
    ae.UpdateDateTime,
    ae.Description,
    ai.AuditItemId,
    ai.PropertyName,
    ai.OldValue,
    ai.NewValue
FROM dbo.AuditEvent ae
JOIN dbo.EntityType et ON et.EntityTypeId = ae.EntityTypeId
JOIN dbo.AppUser au ON au.AppUserId = ae.AppUserId
LEFT JOIN dbo.AuditItem ai ON ai.AuditEventId = ae.AuditEventId
WHERE ae.UpdateDateTime > @Since
ORDER BY ae.AuditEventId DESC, ai.PropertyName
