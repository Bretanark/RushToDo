/*
    RushTodo uses one known AppUser until authentication and authorization are implemented.
    Replace this development-only identity with authenticated user provisioning at that point.
*/
SET IDENTITY_INSERT dbo.AppUser ON

MERGE dbo.AppUser AS target
USING
(
    VALUES
        (1, N'bretanark@gmail.com', N'Brent Clark', NULL)
) AS source (AppUserId, EmailAddress, Name, GoogleSubject)
    ON target.AppUserId = source.AppUserId
WHEN MATCHED AND (target.EmailAddress <> source.EmailAddress OR target.Name <> source.Name OR target.GoogleSubject IS NOT NULL) THEN
    UPDATE SET
        EmailAddress = source.EmailAddress,
        Name = source.Name,
        GoogleSubject = source.GoogleSubject,
        UpdateDateTime = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (AppUserId, EmailAddress, Name, GoogleSubject, UpdateDateTime)
    VALUES (source.AppUserId, source.EmailAddress, source.Name, source.GoogleSubject, SYSUTCDATETIME());

SET IDENTITY_INSERT dbo.AppUser OFF
GO
