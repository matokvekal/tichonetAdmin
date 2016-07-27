CREATE TABLE [dbo].[tblLinesPlan](
[Id] [int] IDENTITY(1,1) NOT NULL,
[LineId] [int] NOT NULL,
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
[SutTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO