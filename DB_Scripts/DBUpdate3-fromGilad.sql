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

USE [BusProject]
GO
drop table [tblSchedule]
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NULL,
	[Direction] [int] NULL,
	[LineId] [int] NULL,
	[DriverId] [int] NULL,
	[BusId] [int] NULL,
	[leaveTime] [datetime2](7) NULL,
	[arriveTime] [datetime2](7) NULL,
 CONSTRAINT [PK_tblSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



