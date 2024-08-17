• Since the storage may be migrated to another type of database in the future,
  I implemented the Repository Pattern so that it doesn’t matter whether it’s interacting
  with SQL Server, Oracle, SQLite, MySQL, or another database.

• To get the DB:

1) Please create the database and ensure that you update the DBConnection connection string with your own.

2) Since I used Entity Framework Code First because it's easy to migrated to another type of DB,
   you can either run the Update-Database command in the Package Manager Console
   or execute the following query in the database:

//***************************************************************************
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Candidates] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Email] nvarchar(450) NOT NULL,
    [PreferredCallStartTime] time NULL,
    [PreferredCallEndTime] time NULL,
    [LinkedInProfileUrl] nvarchar(max) NULL,
    [GitHubProfileUrl] nvarchar(max) NULL,
    [Comment] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Candidates] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Candidates_Email] ON [Candidates] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240816183442_InitialCreate', N'8.0.8');
GO

COMMIT;
GO
//***************************************************************************