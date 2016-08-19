CREATE VIEW [dbo].[vStudents]
AS
SELECT        dbo.tblStudent.pk, dbo.tblStudent.studentId, dbo.tblStudent.firstName, dbo.tblStudent.lastName, dbo.tblStudent.Email, dbo.tblStudent.CellPhone, dbo.tblFamily.parent1Type, dbo.tblFamily.parent1FirstName, 
                         dbo.tblFamily.parent1LastName, dbo.tblFamily.parent1Email, dbo.tblFamily.parent1CellPhone, dbo.tblFamily.parent2Type, dbo.tblFamily.parent2FirstName, dbo.tblFamily.parent2LastName, 
                         dbo.tblFamily.parent2Email, dbo.tblFamily.parent2CellPhone, dbo.tblStudent.city, dbo.tblStudent.Active
FROM            dbo.tblStudent INNER JOIN
                         dbo.tblFamily ON dbo.tblStudent.familyId = dbo.tblFamily.familyId

GO