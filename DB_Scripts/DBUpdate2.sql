

ALTER TABLE StudentsToLines   
DROP CONSTRAINT FK_StudentsToLines_ToStudents;   
GO 

ALTER TABLE StudentsToLines   
ALTER COLUMN [LineId] int NULL
GO 

update [dbo].[tblStudent] SET [Lat]=NULL, [Lng]=NULL