USE [WirelessSensorNetwork]
GO
/****** Object:  Table [dbo].[WSNUser_WSNCity]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNUser_WSNCity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[CityID] [int] NOT NULL,
	[DFSelected] [bit] NOT NULL,
 CONSTRAINT [PK_WSNUsers_WSNCities] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WSNUser]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNUser](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](200) NOT NULL,
	[UserPassword] [nvarchar](200) NOT NULL,
	[NickName] [nvarchar](200) NOT NULL,
	[Remark] [nvarchar](max) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_WSUsers_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WSNUploadData]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNUploadData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NodeAddress] [nvarchar](50) NOT NULL,
	[UploadTime] [datetime] NOT NULL,
	[AirHumidity] [decimal](6, 2) NULL,
	[AirTemperature] [decimal](6, 2) NULL,
	[SoilHumidity] [decimal](6, 2) NULL,
	[SoilTemperature] [decimal](6, 2) NULL,
	[Rainfall] [decimal](6, 2) NULL,
	[APower] [decimal](6, 2) NULL,
	[BPower] [decimal](6, 2) NULL,
	[SPower] [decimal](6, 2) NULL,
	[CityID] [int] NOT NULL,
	[Lai] [decimal](6, 2) NULL,
 CONSTRAINT [PK_DataTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WSNRouteTable]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNRouteTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UploadTime] [datetime] NOT NULL,
	[SourceNode] [nvarchar](10) NOT NULL,
	[NextNode] [nvarchar](10) NOT NULL,
	[TargetNode] [nvarchar](10) NOT NULL,
	[CityID] [int] NULL,
 CONSTRAINT [PK_RouteTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WSNNodeLocationTable]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WSNNodeLocationTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NodeAddress] [nchar](10) NOT NULL,
	[NodeLocation] [varchar](30) NOT NULL,
	[CityID] [int] NULL,
 CONSTRAINT [PK_WSNNodeLocationTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WSNNeighborTable]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNNeighborTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UploadTime] [datetime] NOT NULL,
	[FatherNode] [nvarchar](10) NOT NULL,
	[ChildNode] [nvarchar](10) NULL,
	[CityID] [int] NULL,
 CONSTRAINT [PK_NeighborTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WSNDrawTopologyTable]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNDrawTopologyTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UploadTime] [datetime] NOT NULL,
	[TopologyString] [nvarchar](max) NULL,
	[CityID] [int] NOT NULL,
 CONSTRAINT [PK_WSNDrawTopologyTable] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WSNCity]    Script Date: 04/24/2015 14:59:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WSNCity](
	[ID] [int] NOT NULL,
	[CityName] [nvarchar](50) NULL,
 CONSTRAINT [PK_WSNCity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[CLEARDATA]    Script Date: 04/24/2015 14:59:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CLEARDATA]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM WSNDrawTopologyTable;
    DELETE FROM WSNNeighborTable;
    DELETE FROM WSNRouteTable;
    DELETE FROM WSNUploadData;

END
GO


SET IDENTITY_INSERT [WSNUser] ON

INSERT [WSNCity] ([ID],[CityName]) VALUES ( 1,N'成都')
INSERT [WSNCity] ([ID],[CityName]) VALUES ( 2,N'北京')
INSERT [WSNCity] ([ID],[CityName]) VALUES ( 3,N'广州')

INSERT [WSNUser] ([ID],[UserName],[UserPassword],[NickName],[Remark],[IsAdmin]) VALUES ( 1,N'admin',N'000',N'管理员',N'管理员',1)
INSERT [WSNUser] ([ID],[UserName],[UserPassword],[NickName],[Remark],[IsAdmin]) VALUES ( 2,N'chengdu',N'000',N'成都',N'成都',0)
INSERT [WSNUser] ([ID],[UserName],[UserPassword],[NickName],[Remark],[IsAdmin]) VALUES ( 3,N'beijing',N'000',N'北京',N'北京',0)
INSERT [WSNUser] ([ID],[UserName],[UserPassword],[NickName],[Remark],[IsAdmin]) VALUES ( 4,N'chengduguangzhou',N'000',N'成都和广州',N'成都和广州',0)

INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 1,1,1,0)
INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 2,1,2,0)
INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 3,1,3,0)
INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 4,2,1,1)
INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 5,4,1,0)
INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 6,4,3,0)
INSERT [WSNUser_WSNCity] ([ID],[UserID],[CityID],[DFSelected]) VALUES ( 7,3,2,1)

SET IDENTITY_INSERT [WSNUser_WSNCity] OFF

GO