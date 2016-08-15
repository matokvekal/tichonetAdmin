CREATE TABLE [dbo].[tblTESTMSGTBL] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,

	[StudentName]			NVARCHAR (MAX)	NULL,
	[StudentEmail]					NVARCHAR (MAX)	NULL,

	[MotherName]			NVARCHAR (MAX)	NULL,
	[MotherEmail]					NVARCHAR (MAX)	NULL,

	[FatherName]			NVARCHAR (MAX)	NULL,
	[FatherEmail]					NVARCHAR (MAX)	NULL,

	[LineName]			NVARCHAR (MAX) NULL,

	[BusInfo]			NVARCHAR (MAX) NULL,
	
	[FirstStationName]			NVARCHAR (MAX) NULL,
	[LastStationName]			NVARCHAR (MAX) NULL,

	[RouteStartTime]			DATETIME NULL,
	[RouteEndTime]			DATETIME NULL,


    PRIMARY KEY CLUSTERED ([Id] ASC)
);