-- AppUser table abbreviation: au
CREATE TABLE dbo.AppUser (
    AppUserId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_AppUser PRIMARY KEY CLUSTERED,
    EmailAddress NVARCHAR(255) NULL,
    Name NVARCHAR(255) NULL,
    GoogleSubject NVARCHAR(255) NULL,
    UpdateDateTime DATETIME2 NOT NULL,

    INDEX UX_AppUser_GoogleSubject UNIQUE (GoogleSubject) WHERE GoogleSubject IS NOT NULL,
)
GO
