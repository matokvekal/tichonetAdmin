Alter table [dbo].[Lines]
Add PathGeometry nvarchar(MAX) NULL

Alter table [dbo].[StudentsToLines]
Add PathGeometry nvarchar(MAX) NULL

Alter table [dbo].[StudentsToLines]
Alter COLUMN  LineId int null

Alter table [dbo].[StudentsToLines] 
Alter Column [distanceFromStation] int NULL
