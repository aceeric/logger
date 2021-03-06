USE [meta]
GO
/****** Object:  Table [dbo].[applog]    Script Date: 3/29/2017 7:28:45 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[applog]') AND type in (N'U'))
DROP TABLE [dbo].[applog]
GO
/****** Object:  Table [dbo].[applog]    Script Date: 3/29/2017 7:28:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[applog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[applog](
	[time] [datetime] NULL,
	[log_id] [int] IDENTITY(1,1) NOT NULL,
	[job_id] [uniqueidentifier] NULL,
	[job_name] [varchar](100) NULL,
	[type] [varchar](100) NULL,
	[component] [varchar](100) NULL,
	[message] [varchar](1000) NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
