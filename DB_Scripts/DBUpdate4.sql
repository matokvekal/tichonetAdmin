
/****** Object:  Table [dbo].[tblSystem]    Script Date: 19.06.2016 0:43:38 ******/
DROP TABLE [dbo].[tblSystem]
GO

/****** Object:  Table [dbo].[tblSystem]    Script Date: 19.06.2016 0:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblSystem](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[strKey] [nvarchar](250) NULL,
	[key] [nvarchar](50) NULL,
	[value] [nvarchar](50) NULL,
	[strValue] [nvarchar](max) NULL,
	[LastModify] [datetime] NULL,
	[ModifedBy] [int] NULL,
	[strNamespace] [nchar](10) NULL,
 CONSTRAINT [PK_tblSystems] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


