USE [BusProject]
GO

/****** Object:  View [dbo].[ViewAlllinesByBusCompanyAndStations]    Script Date: 28.08.2016 23:51:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ViewAlllinesByBusCompanyAndStations]
AS
SELECT        dbo.Lines.LineName, dbo.Lines.LineNumber, dbo.Lines.IsActive, dbo.Lines.Sun, dbo.Lines.SunTime, dbo.Lines.Mon, dbo.Lines.MonTime, dbo.Lines.Tue, dbo.Lines.TueTime, dbo.Lines.Wed, dbo.Lines.WedTime, 
                         dbo.Lines.Thu, dbo.Lines.ThuTime, dbo.Lines.Fri, dbo.Lines.FriTime, dbo.Lines.Sut, dbo.Lines.SutTime, dbo.Buses.BusId, dbo.Buses.PlateNumber, dbo.tblBusCompany.companyName, dbo.tblBusCompany.cell, 
                         dbo.tblBusCompany.email, dbo.ViewStationList.StationList
FROM            dbo.BusesToLines INNER JOIN
                         dbo.Lines ON dbo.BusesToLines.LineId = dbo.Lines.Id INNER JOIN
                         dbo.Buses ON dbo.BusesToLines.BusId = dbo.Buses.Id LEFT OUTER JOIN
                         dbo.ViewStationList ON dbo.Lines.Id = dbo.ViewStationList.LineId LEFT OUTER JOIN
                         dbo.tblBusCompany ON dbo.Buses.Owner = dbo.tblBusCompany.pk

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[52] 4[9] 2[20] 3) )"
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
               Top = 246
               Left = 400
               Bottom = 359
               Right = 570
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Lines"
            Begin Extent = 
               Top = 155
               Left = 55
               Bottom = 285
               Right = 250
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Buses"
            Begin Extent = 
               Top = 274
               Left = 739
               Bottom = 404
               Right = 945
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "tblBusCompany"
            Begin Extent = 
               Top = 285
               Left = 1045
               Bottom = 415
               Right = 1216
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "ViewStationList"
            Begin Extent = 
               Top = 76
               Left = 484
               Bottom = 172
               Right = 654
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
      Begin ColumnWidths = 9
         Width = 284
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
         A' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewAlllinesByBusCompanyAndStations'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'lias = 900
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewAlllinesByBusCompanyAndStations'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewAlllinesByBusCompanyAndStations'
GO

