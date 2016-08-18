ALTER TABLE [dbo].[tblFilter] 
ADD [allowUserInput] bit NULL, [allowMultipleSelection] bit NULL

ALTER TABLE [dbo].[tblTemplate] ADD FilterValueContainersJSON NVARCHAR(MAX) NULL