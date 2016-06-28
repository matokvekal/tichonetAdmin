ALTER TABLE [dbo].[Stations]
ADD StationType int NOT NULL default(0)

ALTER TABLE [dbo].[Stations]
ADD Address nvarchar(512) NULL