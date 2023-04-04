
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/24/2023 15:54:13
-- Generated from EDMX file: E:\zry\505\WSN改\WSNserver20190325修复节点动态图数据库\WSNServer\DB\WSNDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WirelessSensorNetWork];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[sysdiagram]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagram];
GO
IF OBJECT_ID(N'[dbo].[WSNCheckNodeAddress]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNCheckNodeAddress];
GO
IF OBJECT_ID(N'[dbo].[WSNCheckNodeLost]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNCheckNodeLost];
GO
IF OBJECT_ID(N'[dbo].[WSNCity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNCity];
GO
IF OBJECT_ID(N'[dbo].[WSNDrawTopologyTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNDrawTopologyTable];
GO
IF OBJECT_ID(N'[dbo].[WSNEveryDayLai]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNEveryDayLai];
GO
IF OBJECT_ID(N'[dbo].[WSNImagesPath]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNImagesPath];
GO
IF OBJECT_ID(N'[dbo].[WSNLAI]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNLAI];
GO
IF OBJECT_ID(N'[dbo].[WSNMail_Code]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNMail_Code];
GO
IF OBJECT_ID(N'[dbo].[WSNMessageRecevieTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNMessageRecevieTable];
GO
IF OBJECT_ID(N'[dbo].[WSNNeighborTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNNeighborTable];
GO
IF OBJECT_ID(N'[dbo].[WSNNodeLocationTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNNodeLocationTable];
GO
IF OBJECT_ID(N'[dbo].[WSNReadUploadData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNReadUploadData];
GO
IF OBJECT_ID(N'[dbo].[WSNRouteTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNRouteTable];
GO
IF OBJECT_ID(N'[dbo].[WSNRTUploadData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNRTUploadData];
GO
IF OBJECT_ID(N'[dbo].[WSNTimeSetTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNTimeSetTable];
GO
IF OBJECT_ID(N'[dbo].[WSNUploadData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNUploadData];
GO
IF OBJECT_ID(N'[dbo].[WSNUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNUser];
GO
IF OBJECT_ID(N'[dbo].[WSNUser_WSNCity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WSNUser_WSNCity];
GO
IF OBJECT_ID(N'[WirelessSensorNetworkModelStoreContainer].[WSNEveryDayLaiBK20210910]', 'U') IS NOT NULL
    DROP TABLE [WirelessSensorNetworkModelStoreContainer].[WSNEveryDayLaiBK20210910];
GO
IF OBJECT_ID(N'[WirelessSensorNetworkModelStoreContainer].[wsneverydaylaibk20211213]', 'U') IS NOT NULL
    DROP TABLE [WirelessSensorNetworkModelStoreContainer].[wsneverydaylaibk20211213];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(max)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int  NOT NULL,
    [version] int  NULL,
    [definition] binary(50)  NULL
);
GO

-- Creating table 'WSNCheckNodeAddresses'
CREATE TABLE [dbo].[WSNCheckNodeAddresses] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CheckNode] nvarchar(50)  NOT NULL,
    [CityID] int  NOT NULL
);
GO

-- Creating table 'WSNCheckNodeLosts'
CREATE TABLE [dbo].[WSNCheckNodeLosts] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Date_t] datetime  NOT NULL,
    [Last_id] int  NOT NULL
);
GO

-- Creating table 'WSNCities'
CREATE TABLE [dbo].[WSNCities] (
    [ID] int  NOT NULL,
    [CityName] nvarchar(50)  NULL
);
GO

-- Creating table 'WSNDrawTopologyTables'
CREATE TABLE [dbo].[WSNDrawTopologyTables] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UploadTime] datetime  NOT NULL,
    [TopologyString] nvarchar(max)  NULL,
    [CityID] int  NOT NULL
);
GO

-- Creating table 'WSNEveryDayLais'
CREATE TABLE [dbo].[WSNEveryDayLais] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [Date_t] datetime  NOT NULL,
    [Lai] decimal(6,2)  NOT NULL,
    [CityID] int  NOT NULL
);
GO

-- Creating table 'WSNImagesPaths'
CREATE TABLE [dbo].[WSNImagesPaths] (
    [ID] int  NOT NULL,
    [CityID] int  NOT NULL,
    [NodeAddress] varchar(50)  NOT NULL,
    [UploadDate] datetime  NOT NULL,
    [ImageName] varchar(50)  NOT NULL,
    [ImagePath] varchar(50)  NOT NULL
);
GO

-- Creating table 'WSNLAIs'
CREATE TABLE [dbo].[WSNLAIs] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [Updatatime] datetime  NOT NULL,
    [CityID] int  NOT NULL,
    [Lai] decimal(6,2)  NULL,
    [LaiSort] nvarchar(50)  NULL
);
GO

-- Creating table 'WSNMail_Code'
CREATE TABLE [dbo].[WSNMail_Code] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [sendmailaddress] nvarchar(50)  NOT NULL,
    [authorizationcode] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'WSNMessageRecevieTables'
CREATE TABLE [dbo].[WSNMessageRecevieTables] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [MessageRecevieName] nvarchar(50)  NOT NULL,
    [MessageRecevieSex] nvarchar(50)  NOT NULL,
    [MessageRecevieTel] nvarchar(50)  NOT NULL,
    [MessageRecevieMail] nvarchar(50)  NULL,
    [NetAuthorityNum] nvarchar(50)  NOT NULL,
    [NetAuthorityRemark] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'WSNNeighborTables'
CREATE TABLE [dbo].[WSNNeighborTables] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UploadTime] datetime  NOT NULL,
    [FatherNode] nvarchar(10)  NOT NULL,
    [ChildNode] nvarchar(10)  NULL,
    [CityID] int  NULL
);
GO

-- Creating table 'WSNNodeLocationTables'
CREATE TABLE [dbo].[WSNNodeLocationTables] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nchar(4)  NOT NULL,
    [NodeLocation] varchar(30)  NOT NULL,
    [CityID] int  NULL
);
GO

-- Creating table 'WSNReadUploadDatas'
CREATE TABLE [dbo].[WSNReadUploadDatas] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [UploadTime] datetime  NOT NULL,
    [AirHumidity] decimal(6,2)  NULL,
    [AirTemperature] decimal(6,2)  NULL,
    [SoilHumidity] decimal(6,2)  NULL,
    [SoilTemperature] decimal(6,2)  NULL,
    [Rainfall] decimal(6,2)  NULL,
    [APower] decimal(6,2)  NULL,
    [BPower] decimal(6,2)  NULL,
    [SPower] decimal(6,2)  NULL,
    [CityID] int  NOT NULL,
    [Lai] decimal(6,2)  NULL
);
GO

-- Creating table 'WSNRouteTables'
CREATE TABLE [dbo].[WSNRouteTables] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UploadTime] datetime  NOT NULL,
    [SourceNode] nvarchar(10)  NOT NULL,
    [NextNode] nvarchar(10)  NOT NULL,
    [TargetNode] nvarchar(10)  NOT NULL,
    [CityID] int  NULL
);
GO

-- Creating table 'WSNRTUploadDatas'
CREATE TABLE [dbo].[WSNRTUploadDatas] (
    [ID] int  NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [UploadTime] datetime  NOT NULL,
    [AirHumidity] decimal(6,2)  NULL,
    [AirTemperature] decimal(6,2)  NULL,
    [SoilHumidity] decimal(6,2)  NULL,
    [SoilTemperature] decimal(6,2)  NULL,
    [Rainfall] decimal(6,2)  NULL,
    [APower] decimal(6,2)  NULL,
    [BPower] decimal(6,2)  NULL,
    [SPower] decimal(6,2)  NULL,
    [CityID] int  NOT NULL,
    [Lai] decimal(6,2)  NULL
);
GO

-- Creating table 'WSNTimeSetTables'
CREATE TABLE [dbo].[WSNTimeSetTables] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SleepTime] int  NOT NULL,
    [CollectTime] int  NOT NULL,
    [CityID] int  NOT NULL,
    [NodeID] int  NULL
);
GO

-- Creating table 'WSNUploadDatas'
CREATE TABLE [dbo].[WSNUploadDatas] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [UploadTime] datetime  NOT NULL,
    [AirHumidity] decimal(6,2)  NULL,
    [AirTemperature] decimal(6,2)  NULL,
    [SoilHumidity] decimal(6,2)  NULL,
    [SoilTemperature] decimal(6,2)  NULL,
    [Rainfall] decimal(6,2)  NULL,
    [APower] decimal(6,2)  NULL,
    [RSSI] decimal(6,2)  NULL,
    [SPower] decimal(6,2)  NULL,
    [CityID] int  NOT NULL,
    [Lai] decimal(6,2)  NULL,
    [CI] decimal(6,2)  NULL,
    [DIFN] decimal(6,2)  NULL,
    [MLai] decimal(6,2)  NULL,
    [MTA] decimal(6,2)  NULL,
    [BPower] decimal(6,2)  NULL
);
GO

-- Creating table 'WSNUsers'
CREATE TABLE [dbo].[WSNUsers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(200)  NOT NULL,
    [UserPassword] nvarchar(200)  NOT NULL,
    [NickName] nvarchar(200)  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [IsAdmin] bit  NOT NULL
);
GO

-- Creating table 'WSNUser_WSNCity'
CREATE TABLE [dbo].[WSNUser_WSNCity] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [CityID] int  NOT NULL,
    [DFSelected] bit  NOT NULL
);
GO

-- Creating table 'WSNEveryDayLaiBK20210910'
CREATE TABLE [dbo].[WSNEveryDayLaiBK20210910] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [Date_t] datetime  NOT NULL,
    [Lai] decimal(6,2)  NOT NULL,
    [CityID] int  NOT NULL
);
GO

-- Creating table 'wsneverydaylaibk20211213'
CREATE TABLE [dbo].[wsneverydaylaibk20211213] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NodeAddress] nvarchar(50)  NOT NULL,
    [Date_t] datetime  NOT NULL,
    [Lai] decimal(6,2)  NOT NULL,
    [CityID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [ID] in table 'WSNCheckNodeAddresses'
ALTER TABLE [dbo].[WSNCheckNodeAddresses]
ADD CONSTRAINT [PK_WSNCheckNodeAddresses]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNCheckNodeLosts'
ALTER TABLE [dbo].[WSNCheckNodeLosts]
ADD CONSTRAINT [PK_WSNCheckNodeLosts]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNCities'
ALTER TABLE [dbo].[WSNCities]
ADD CONSTRAINT [PK_WSNCities]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNDrawTopologyTables'
ALTER TABLE [dbo].[WSNDrawTopologyTables]
ADD CONSTRAINT [PK_WSNDrawTopologyTables]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNEveryDayLais'
ALTER TABLE [dbo].[WSNEveryDayLais]
ADD CONSTRAINT [PK_WSNEveryDayLais]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNImagesPaths'
ALTER TABLE [dbo].[WSNImagesPaths]
ADD CONSTRAINT [PK_WSNImagesPaths]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNLAIs'
ALTER TABLE [dbo].[WSNLAIs]
ADD CONSTRAINT [PK_WSNLAIs]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNMail_Code'
ALTER TABLE [dbo].[WSNMail_Code]
ADD CONSTRAINT [PK_WSNMail_Code]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNMessageRecevieTables'
ALTER TABLE [dbo].[WSNMessageRecevieTables]
ADD CONSTRAINT [PK_WSNMessageRecevieTables]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNNeighborTables'
ALTER TABLE [dbo].[WSNNeighborTables]
ADD CONSTRAINT [PK_WSNNeighborTables]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNNodeLocationTables'
ALTER TABLE [dbo].[WSNNodeLocationTables]
ADD CONSTRAINT [PK_WSNNodeLocationTables]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNReadUploadDatas'
ALTER TABLE [dbo].[WSNReadUploadDatas]
ADD CONSTRAINT [PK_WSNReadUploadDatas]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNRouteTables'
ALTER TABLE [dbo].[WSNRouteTables]
ADD CONSTRAINT [PK_WSNRouteTables]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNRTUploadDatas'
ALTER TABLE [dbo].[WSNRTUploadDatas]
ADD CONSTRAINT [PK_WSNRTUploadDatas]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNTimeSetTables'
ALTER TABLE [dbo].[WSNTimeSetTables]
ADD CONSTRAINT [PK_WSNTimeSetTables]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNUploadDatas'
ALTER TABLE [dbo].[WSNUploadDatas]
ADD CONSTRAINT [PK_WSNUploadDatas]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNUsers'
ALTER TABLE [dbo].[WSNUsers]
ADD CONSTRAINT [PK_WSNUsers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WSNUser_WSNCity'
ALTER TABLE [dbo].[WSNUser_WSNCity]
ADD CONSTRAINT [PK_WSNUser_WSNCity]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID], [NodeAddress], [Date_t], [Lai], [CityID] in table 'WSNEveryDayLaiBK20210910'
ALTER TABLE [dbo].[WSNEveryDayLaiBK20210910]
ADD CONSTRAINT [PK_WSNEveryDayLaiBK20210910]
    PRIMARY KEY CLUSTERED ([ID], [NodeAddress], [Date_t], [Lai], [CityID] ASC);
GO

-- Creating primary key on [ID], [NodeAddress], [Date_t], [Lai], [CityID] in table 'wsneverydaylaibk20211213'
ALTER TABLE [dbo].[wsneverydaylaibk20211213]
ADD CONSTRAINT [PK_wsneverydaylaibk20211213]
    PRIMARY KEY CLUSTERED ([ID], [NodeAddress], [Date_t], [Lai], [CityID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------