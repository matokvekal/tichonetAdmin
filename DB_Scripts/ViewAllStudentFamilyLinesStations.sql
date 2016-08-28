USE [BusProject]
GO

/****** Object:  View [dbo].[ViewAllStudentFamilyLinesStations]    Script Date: 28.08.2016 23:51:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ViewAllStudentFamilyLinesStations]
AS
SELECT        dbo.tblStudent.pk, dbo.tblStudent.studentId, dbo.tblStudent.firstName, dbo.tblStudent.lastName, dbo.tblStudent.Shicva, dbo.tblStudent.class, dbo.tblStudent.city, dbo.tblStudent.street, dbo.tblStudent.houseNumber,
                          dbo.tblStudent.zipCode, dbo.tblStudent.CellPhone, dbo.tblStudent.Email, dbo.tblStudent.paymentStatus, dbo.tblSchool.name, dbo.tblFamily.familyId, dbo.tblFamily.ParentId, dbo.tblFamily.parent1Type, 
                         dbo.tblFamily.parent1FirstName, dbo.tblFamily.parent1LastName, dbo.tblFamily.parent1Email, dbo.tblFamily.parent1CellPhone, dbo.tblFamily.parent2Type, dbo.tblFamily.parent2FirstName, 
                         dbo.tblFamily.parent2LastName, dbo.tblFamily.parent2Email, dbo.tblFamily.parent2CellPhone, dbo.Lines.LineName, dbo.Lines.LineNumber, dbo.Lines.IsActive, dbo.Lines.Sun, dbo.Lines.SunTime, 
                         dbo.Lines.Mon, dbo.Lines.MonTime, dbo.Lines.Tue, dbo.Lines.TueTime, dbo.Lines.Wed, dbo.Lines.WedTime, dbo.Lines.Thu, dbo.Lines.ThuTime, dbo.Lines.Fri, dbo.Lines.FriTime, dbo.Lines.Sut, 
                         dbo.Lines.SutTime, dbo.Stations.StationName, dbo.Buses.BusId, dbo.Buses.PlateNumber, dbo.tblBusCompany.companyName, dbo.Lines.BasicArriveTime, dbo.Lines.BasicDepartureTime, 
                         dbo.Lines.Direction
FROM            dbo.tblFamily RIGHT OUTER JOIN
                         dbo.BusesToLines INNER JOIN
                         dbo.Lines ON dbo.BusesToLines.LineId = dbo.Lines.Id INNER JOIN
                         dbo.Buses ON dbo.BusesToLines.BusId = dbo.Buses.Id LEFT OUTER JOIN
                         dbo.StudentsToLines INNER JOIN
                         dbo.Stations ON dbo.StudentsToLines.StationId = dbo.Stations.Id INNER JOIN
                         dbo.tblStudent ON dbo.StudentsToLines.StudentId = dbo.tblStudent.pk ON dbo.Lines.Id = dbo.StudentsToLines.LineId LEFT OUTER JOIN
                         dbo.tblBusCompany ON dbo.Buses.Owner = dbo.tblBusCompany.pk LEFT OUTER JOIN
                         dbo.tblSchool ON dbo.tblStudent.schoolId = dbo.tblSchool.id ON dbo.tblFamily.familyId = dbo.tblStudent.familyId

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[66] 4[5] 2[10] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "BusesToLines"
            Begin Extent = 
               Top = 13
               Left = 361
               Bottom = 126
               Right = 531
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Lines"
            Begin Extent = 
               Top = 16
               Left = 23
               Bottom = 248
               Right = 223
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Buses"
            Begin Extent = 
               Top = 18
               Left = 600
               Bottom = 148
               Right = 806
            End
            DisplayFlags = 280
            TopColumn = 11
         End
         Begin Table = "tblBusCompany"
            Begin Extent = 
               Top = 14
               Left = 884
               Bottom = 144
               Right = 1055
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "tblFamily"
            Begin Extent = 
               Top = 155
               Left = 894
               Bottom = 285
               Right = 1108
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tblSchool"
            Begin Extent = 
               Top = 281
               Left = 908
               Bottom = 411
               Right = 1078
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "StudentsToLines"
            Begin Extent = 
               Top = 174
               Left = 325
               Bottom = 304
               Right = 523
            End
       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewAllStudentFamilyLinesStations'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Stations"
            Begin Extent = 
               Top = 316
               Left = 617
               Bottom = 446
               Right = 787
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "tblStudent"
            Begin Extent = 
               Top = 165
               Left = 614
               Bottom = 295
               Right = 811
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 49
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewAllStudentFamilyLinesStations'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewAllStudentFamilyLinesStations'
GO

