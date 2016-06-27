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
USE [BusProject]
GO

/****** Object:  Table [dbo].[tblPayment]    Script Date: 06/26/2016 15:55:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON


/****** Object:  Table [dbo].[tblPayment]    Script Date: 06/26/2016 16:09:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
USE [BusProject]
GO

/****** Object:  Table [dbo].[tblPayment]    Script Date: 06/26/2016 16:37:24 ******/
SET ANSI_NULLS ON
GO
drop table [tblPayment]
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblPayment](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[familyId] [int] NULL,
	[parentId] [nvarchar](50) NULL,
	[studentId] [nvarchar](50) NULL,
	[paymentContent] [nvarchar](50) NULL,
	[Paymentamaount] [float] NULL,
	[paymentDueDate] [date] NULL,
	[requestDate] [date] NULL,
	[paymentCompany] [nvarchar](max) NULL,
	[processed] [bit] NULL,
	[referance] [nvarchar](max) NULL,
	[Token] [nvarchar](max) NULL,
	[paymentStatus] [int] NULL,
 CONSTRAINT [PK_tblPayment] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO





ALTER TABLE dbo.[Lines] ADD
	[Sun] [bit] NULL,
	[SunTime] [datetime] NULL,
	[Mon] [bit] NULL,
	[MonTime] [datetime] NULL,
	[Tue] [bit] NULL,
	[TueTime] [datetime] NULL,
	[Wed] [bit] NULL,
	[WedTime] [datetime] NULL,
	[Thu] [bit] NULL,
	[ThuTime] [datetime] NULL,
	[Fri] [bit] NULL,
	[FriTime] [datetime] NULL,
	[Sut] [bit] NULL,
	[SutTime] [datetime] NULL;
GO