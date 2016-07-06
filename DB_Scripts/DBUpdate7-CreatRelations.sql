ALTER TABLE [dbo].[tblBusCompany] ADD CONSTRAINT [PK_tblBusCompany] PRIMARY KEY CLUSTERED 
(
 [pk] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [dbo].[Buses] ADD CONSTRAINT [FK_OwnerBusCompany_PK] FOREIGN KEY ([Owner]) REFERENCES [dbo].[tblBusCompany]([pk]);

ALTER TABLE [dbo].[tblSchedule] ADD CONSTRAINT [FK_Schedule_Buses] FOREIGN KEY ([BusId]) REFERENCES [dbo].[Buses]([Id]);

ALTER TABLE [dbo].[tblSchedule] ADD CONSTRAINT [FK_Schedule_Drivers] FOREIGN KEY ([DriverId]) REFERENCES [dbo].[Drivers]([Id]);

ALTER TABLE [dbo].[tblSchedule] ADD CONSTRAINT [FK_Schedule_Lines] FOREIGN KEY ([LineId]) REFERENCES [dbo].[Lines]([Id]);

