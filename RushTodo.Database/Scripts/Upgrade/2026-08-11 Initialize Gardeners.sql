/*
    Manually run this script once after deploying the RushToDo database.
    It supplies useful gardeners until gardener maintenance is implemented.
*/
MERGE dbo.Gardener AS target
USING
(
    VALUES
        (N'Bob Green', N'021 000 0001', N'bob.green@example.com'),
        (N'Mary Bloom', N'021 000 0002', NULL),
        (N'Alice Sprout', N'021 000 0003', N'alice.sprout@example.com')
) AS source (Name, PhoneNumber, EmailAddress)
    ON target.Name = source.Name
WHEN MATCHED AND (target.PhoneNumber <> source.PhoneNumber OR ISNULL(target.EmailAddress, N'') <> ISNULL(source.EmailAddress, N'') OR target.IsDeleted = 1) THEN
    UPDATE SET
        PhoneNumber = source.PhoneNumber,
        EmailAddress = source.EmailAddress,
        UpdateDateTime = SYSUTCDATETIME(),
        IsDeleted = 0
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Name, PhoneNumber, EmailAddress, UpdateDateTime, IsDeleted)
    VALUES (source.Name, source.PhoneNumber, source.EmailAddress, SYSUTCDATETIME(), 0);
GO
