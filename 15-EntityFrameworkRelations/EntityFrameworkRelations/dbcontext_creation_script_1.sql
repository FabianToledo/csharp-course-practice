-- Creado a partir el dbContext del video

CREATE TABLE [Bricks] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(250) NOT NULL,
    [BrickColor] int NULL,
    [Discriminator] nvarchar(max) NOT NULL,
    [Length] int NULL,
    [Width] int NULL,
    [IsDualSided] bit NULL,
    CONSTRAINT [PK_Bricks] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(250) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Vendors] (
    [Id] int NOT NULL IDENTITY,
    [VendorName] nvarchar(250) NOT NULL,
    CONSTRAINT [PK_Vendors] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [BrickTag] (
    [BricksId] int NOT NULL,
    [TagsId] int NOT NULL,
    CONSTRAINT [PK_BrickTag] PRIMARY KEY ([BricksId], [TagsId]),
    CONSTRAINT [FK_BrickTag_Bricks_BricksId] FOREIGN KEY ([BricksId]) REFERENCES [Bricks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BrickTag_Tags_TagsId] FOREIGN KEY ([TagsId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [BrickAvailabilities] (
    [Id] int NOT NULL IDENTITY,
    [VendorId] int NOT NULL,
    [BrickId] int NOT NULL,
    [AvailableAmount] int NOT NULL,
    [PriceEur] decimal(8,2) NOT NULL,
    CONSTRAINT [PK_BrickAvailabilities] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BrickAvailabilities_Bricks_BrickId] FOREIGN KEY ([BrickId]) REFERENCES [Bricks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BrickAvailabilities_Vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [Vendors] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_BrickAvailabilities_BrickId] ON [BrickAvailabilities] ([BrickId]);
GO


CREATE INDEX [IX_BrickAvailabilities_VendorId] ON [BrickAvailabilities] ([VendorId]);
GO


CREATE INDEX [IX_BrickTag_TagsId] ON [BrickTag] ([TagsId]);
GO
