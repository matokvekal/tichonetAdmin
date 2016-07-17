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

alter table [Buses]
add


	[seats] [int] NULL,
	[price] [float] NULL,
	[munifacturedate] [date] NULL,
	[LicensingDueDate] [date] NULL,
	[insuranceDueDate] [date] NULL,
	[winterLicenseDueDate] [date] NULL,
	[brakeTesDueDate] [date] NULL
GO
CREATE TABLE [dbo].[tblCalendar](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[date] [date] NULL,
	[month] [nvarchar](50) NULL,
	[HebMonth] [nvarchar](50) NULL,
	[day] [nvarchar](50) NULL,
	[active] [bit] NULL,
	[event] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_tblCalendar] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

drop table tblstreet
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblStreet](
	[cityId] [int] NOT NULL,
	[streetId] [int] NOT NULL,
	[streetName] [nvarchar](50) NOT NULL,
	[cityName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_tblStreet_1] PRIMARY KEY CLUSTERED 
(
	[cityId] ASC,
	[streetId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

alter table tblStudent

Add
	cityId [int] NULL,
	streetId [Int] NULL
	
	go
alter table tblFamily
add 
 	[PaymentPlanID] [nvarchar](MAX) NULL,
 	[PaymentRequestID] [nvarchar](MAX) NULL
 go
 drop table TblPayment

/****** Object:  Table [dbo].[tblPayment]    Script Date: 07/03/2016 10:11:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblPayment](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[familyId] [int] NULL,
	[parentId] [nvarchar](50) NULL,
	[firstNme] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[studentId] [nvarchar](50) NULL,
	[PaymentCode] [int] NULL,
	[paymentName] [nvarchar](max) NULL,
	[PaymentSum] [float] NULL,
	[paymentDay] [datetime] NULL,
	[paymentCompany] [nvarchar](max) NULL,
	[paymentOK] [bit] NULL,
	[PaymentRequestId] [nvarchar](max) NULL,
	[PaymentPlanID] [nvarchar](max) NULL,
	[paymentStatus] [int] NULL,
 CONSTRAINT [PK_tblPayment] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblPaymentCode](
	[paymentCode] [int] NOT NULL,
	[paymentName] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblPaymentCode] PRIMARY KEY CLUSTERED 
(
	[paymentCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
<<<<<<< .mine

ALTER TABLE [dbo].[tblFamily]
Add

	[registrationStatus] [bit] 
GO

update tblfamily 
set [registrationStatus]=0

go
ALTER TABLE [dbo].[tblFamily]ALTER COLUMN [registrationStatus] [bit] NOT NULL
go

drop table tblPayment
go

drop table tblPaymentCode
go

CREATE TABLE [dbo].[tblPaymentOrders](
	[PaymentOrderId] [int] IDENTITY(1,1) NOT NULL,
	[FamilyId] [int] NULL,
	[StudentId] [nvarchar](50) NULL,
	[StudentPk] [int] NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastNeme] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[cellPhone] [nvarchar](100) NULL,
	[schoolId] [int] NULL,
	[AutorisationCode] [nvarchar](100) NULL,
	[Amount] [decimal](18, 2) NULL,
	[CCGateway] [nvarchar](50) NULL,
	[OrderDate] [datetime] NULL,
	[CardToken] [nvarchar](50) NULL,
	[CardExp] [nvarchar](50) NULL,
	[PersonalId] [nvarchar](50) NULL,
	[CardMask] [nvarchar](50) NULL,
	[TxId] [nvarchar](50) NULL,
	[Status] [int] NOT NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentRequestId] [int] NULL,
 CONSTRAINT [PK_tblPaymentOrders] PRIMARY KEY CLUSTERED 
(
	[PaymentOrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [BusProject]
GO

/****** Object:  Table [dbo].[tblBusCompany]    Script Date: 07/06/2016 14:34:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblBusCompany](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[companyName] [nvarchar](50) NULL,
	[manager] [nvarchar](50) NULL,
	[tel] [nvarchar](50) NULL,
	[cell] [nvarchar](50) NULL,
	[email] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblBusCompany] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



drop table tblCalendar

CREATE TABLE [dbo].[tblCalendar](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[date] [date] NULL,
	[month] [nvarchar](50) NULL,
	[HebMonth] [nvarchar](50) NULL,
	[HebDate] [nvarchar](50) NULL,
	[day] [nvarchar](50) NULL,
	[active] [bit] NULL,
	[event] [nvarchar](50)  NULL,
 CONSTRAINT [PK_tblCalander] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


=======

ALTER TABLE [dbo].[tblFamily]
Add

	[registrationStatus] [bit] 
GO

update tblfamily 
set [registrationStatus]=0

go
ALTER TABLE [dbo].[tblFamily]ALTER COLUMN [registrationStatus] [bit] NOT NULL
go

drop table tblPayment
go

drop table tblPaymentCode
go

CREATE TABLE [dbo].[tblPaymentOrders](
	[PaymentOrderId] [int] IDENTITY(1,1) NOT NULL,
	[FamilyId] [int] NULL,
	[StudentId] [nvarchar](50) NULL,
	[StudentPk] [int] NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastNeme] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[cellPhone] [nvarchar](100) NULL,
	[schoolId] [int] NULL,
	[AutorisationCode] [nvarchar](100) NULL,
	[Amount] [decimal](18, 2) NULL,
	[CCGateway] [nvarchar](50) NULL,
	[OrderDate] [datetime] NULL,
	[CardToken] [nvarchar](50) NULL,
	[CardExp] [nvarchar](50) NULL,
	[PersonalId] [nvarchar](50) NULL,
	[CardMask] [nvarchar](50) NULL,
	[TxId] [nvarchar](50) NULL,
	[Status] [int] NOT NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentRequestId] [int] NULL,
 CONSTRAINT [PK_tblPaymentOrders] PRIMARY KEY CLUSTERED 
(
	[PaymentOrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [BusProject]
GO

/****** Object:  Table [dbo].[tblBusCompany]    Script Date: 07/06/2016 14:34:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblBusCompany](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[companyName] [nvarchar](50) NULL,
	[manager] [nvarchar](50) NULL,
	[tel] [nvarchar](50) NULL,
	[cell] [nvarchar](50) NULL,
	[email] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblBusCompany] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


>>>>>>> .r125
