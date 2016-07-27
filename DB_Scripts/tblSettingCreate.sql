CREATE TABLE [dbo].[tblSetting](
	[Id] [int] NOT NULL,
	[PopulateLinesIsActive] [bit] NULL,
	[PopulateLinesLastRun] [datetime] NULL,
 CONSTRAINT [PK__tblSetti__3214EC07ED3B3617] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblSetting] ADD  CONSTRAINT [DF_tblSetting_Id]  DEFAULT ((0)) FOR [Id]
GO

ALTER TABLE [dbo].[tblSetting] ADD  CONSTRAINT [DF_tblSetting_PopulateLinesIsActive]  DEFAULT ((0)) FOR [PopulateLinesIsActive]
GO
