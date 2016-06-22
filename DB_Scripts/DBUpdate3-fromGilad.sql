ALTER TABLE [dbo].[tblStudent]
ADD  	[distanceFromSchool] [float] NULL,
	[siblingAtSchool] [bit] NULL,
	[specialRequest] [bit] NULL,
	[request] [nvarchar](250) NULL


GO

ALTER TABLE dbo.tblFamily
ADD  
	[oneParentOnly][bit]NOT NULL
GO

USE [BusProject]
GO

/****** Object:  Table [dbo].[Schedule]    Script Date: 06/22/2016 21:08:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Schedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[Direction] [int] NOT NULL,
	[LineId] [int] NOT NULL,
	[DriverId] [int] NOT NULL,
	[BusId] [int] NOT NULL,
	[leaveTime] [datetime2](7) NOT NULL,
	[arriveTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__Schedule__3214EC0732E0915F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_ToBuses] FOREIGN KEY([BusId])
REFERENCES [dbo].[Buses] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_ToBuses]
GO

ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_ToDrivers] FOREIGN KEY([DriverId])
REFERENCES [dbo].[Drivers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_ToDrivers]
GO

ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_ToLines] FOREIGN KEY([LineId])
REFERENCES [dbo].[Lines] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_ToLines]
GO

ALTER TABLE [dbo].[Schedule] ADD  CONSTRAINT [DF__Schedule__Direct__44FF419A]  DEFAULT ((0)) FOR [Direction]
GO


