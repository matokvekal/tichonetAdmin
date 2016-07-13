ALTER TABLE StudentsToLines   
DROP CONSTRAINT FK_StudentsToLines_ToStudents;   
GO 

ALTER TABLE StudentsToLines   
DROP CONSTRAINT FK_StudentsToLines_ToLines;   
GO 