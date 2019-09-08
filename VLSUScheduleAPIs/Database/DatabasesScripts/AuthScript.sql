﻿IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Users] (
    [ID] int NOT NULL IDENTITY,
    [FIO] nvarchar(max) NULL,
    [Birthday] datetime2 NOT NULL,
    [LastActiveDateTime] datetime2 NOT NULL,
    [Login] nvarchar(max) NULL,
    [UserType] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([ID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190908152853_Init', N'2.2.6-servicing-10079');

GO

