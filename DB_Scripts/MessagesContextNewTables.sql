CREATE TABLE [dbo].[tblRecepientFilterTableName] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,

	[Name]			NVARCHAR (MAX)	NOT NULL,
	[ReferncedTableName]			NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblRecepientFilter] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
	[tblRecepientFilterTableNameId]	INT		NOT NULL,

	[Name]			NVARCHAR (150)	NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblFilter] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
    [tblRecepientFilterId]			INT				NOT NULL,

	[Key]			NVARCHAR (MAX)	NOT NULL,
	[ValuesJSON]					NVARCHAR (MAX)	NOT NULL,
	[OperatorsJSON]					NVARCHAR (MAX)	NOT NULL,
    [Type]          NVARCHAR (100) NOT NULL,
	[allowUserInput]				BIT            NULL,
    [allowMultipleSelection]		BIT            NULL,
	[Name]          NVARCHAR (150) NULL,
    [autoUpdatedList]				BIT            NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblWildcard] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
    [tblRecepientFilterId]			INT				NOT NULL,

	[Name]			NVARCHAR (150)	NOT NULL,
	[Code]			NVARCHAR (150)	NOT NULL,
	[Key]			NVARCHAR (MAX)	NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblRecepientCard] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
    [tblRecepientFilterId]			INT				NOT NULL,

	[Name]			NVARCHAR (150)	NOT NULL,
	
	[NameKey]			NVARCHAR (MAX)	NOT NULL,
	[EmailKey]			NVARCHAR (MAX)	NOT NULL,
	[PhoneKey]			NVARCHAR (MAX)	NOT NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblTemplate] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
    [tblRecepientFilterId]			INT				NOT NULL,

	[Name]			NVARCHAR (150)	NOT NULL,

	[IsSms]			BIT				NOT NULL, 
	[MsgHeader]		NVARCHAR (500)	NULL,
	[MsgBody]		NVARCHAR (MAX)	NULL,
	[FilterValueContainersJSON] NVARCHAR (MAX) NULL,
    [ChoosenReccardIdsJSON]				NVARCHAR (MAX) NULL,


    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblMessageSchedule] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,

	[Name]			NVARCHAR (150)	NOT NULL,
	[tblTemplateId] INT NOT NULL,
	[ScheduleDate]	DATETIME NULL,
	[RepeatMode]	NVARCHAR (10) NULL,
	[IsActive]		BIT NOT NULL,
	[InArchive]		BIT NULL,

	[IsSms]			BIT				NOT NULL, 
	[MsgHeader]		NVARCHAR (500)	NULL,
	[MsgBody]		NVARCHAR (MAX)	NULL,
	[FilterValueContainersJSON] NVARCHAR (MAX) NULL,
    [ChoosenReccardIdsJSON]				NVARCHAR (MAX) NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblMessageBatch] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
	[tblMessageScheduleId]			INT NOT NULL,
	[CreatedOn]		DATETIME NULL,
	[FinishedOn]	DATETIME NULL,
	[Errors]		NVARCHAR NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblMessage] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
	[Header]		NVARCHAR (MAX)	NULL,
	[Body]			NVARCHAR (MAX)	NULL,	
	[Adress]		NVARCHAR (MAX)	NOT NULL,
	[IsSms]			BIT				NOT NULL,
	[SentOn]		DATETIME NULL,
	[tblMessageBatchId] INT NOT NULL,
    
	PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblPendingMessagesQueue] (
    [Id]           INT NOT NULL,
    [Priority]     INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Id]) REFERENCES [dbo].[tblMessage] ([Id])
);

CREATE TABLE [dbo].[tblEmailSenderDataProvider] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (150) NOT NULL,
    [IsActive]             BIT            NOT NULL,
    [FromEmailAddress]     NVARCHAR (MAX) NOT NULL,
    [FromEmailDisplayName] NVARCHAR (300) NOT NULL,
    [FromEmailPassword]    NVARCHAR (MAX) NOT NULL,
    [SmtpHostName]         NVARCHAR (MAX) NOT NULL,
    [SmtpPort]             INT            NOT NULL,
    [EnableSsl]            BIT            NOT NULL,
    [MaxMessagesInHour]    INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[tblBatchCreationManagerData] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,

	[LastStartTime] DATETIME NOT NULL,
	[LastEndTime]	DATETIME NOT NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)

);