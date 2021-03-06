USE [master]
GO
/****** Object:  Database [e0615]    Script Date: 2017/10/5 下午 12:57:57 ******/
CREATE DATABASE [e0615]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'e0615', FILENAME = N'E:\DB\MSSQL\e0615.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'e0615_log', FILENAME = N'E:\DB\MSSQL\e0615_log.ldf' , SIZE = 14336KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [e0615] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [e0615].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [e0615] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [e0615] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [e0615] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [e0615] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [e0615] SET ARITHABORT OFF 
GO
ALTER DATABASE [e0615] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [e0615] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [e0615] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [e0615] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [e0615] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [e0615] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [e0615] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [e0615] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [e0615] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [e0615] SET  DISABLE_BROKER 
GO
ALTER DATABASE [e0615] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [e0615] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [e0615] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [e0615] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [e0615] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [e0615] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [e0615] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [e0615] SET RECOVERY FULL 
GO
ALTER DATABASE [e0615] SET  MULTI_USER 
GO
ALTER DATABASE [e0615] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [e0615] SET DB_CHAINING OFF 
GO
ALTER DATABASE [e0615] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [e0615] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'e0615', N'ON'
GO
USE [e0615]
GO
/****** Object:  User [epcs]    Script Date: 2017/10/5 下午 12:57:57 ******/
CREATE USER [epcs] FOR LOGIN [epcs] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [epcs]
GO
/****** Object:  Table [dbo].[Auth]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Auth](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [varchar](20) NOT NULL,
	[Account] [nvarchar](50) NOT NULL,
	[MenuNo] [varchar](10) NOT NULL,
	[Add] [bit] NOT NULL,
	[Edit] [bit] NOT NULL,
	[Del] [bit] NOT NULL,
	[Query] [bit] NOT NULL,
	[Audit] [bit] NOT NULL,
	[Print] [bit] NOT NULL,
	[Export] [bit] NOT NULL,
	[Import] [bit] NOT NULL,
	[Admin] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AdminAuth] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Group]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group](
	[GroupID] [varchar](20) NOT NULL,
	[GroupName] [nvarchar](50) NOT NULL,
	[GroupDesc] [nvarchar](255) NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GroupUser]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupUser](
	[GroupID] [varchar](20) NOT NULL,
	[Account] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Innolux_DockDoor]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Innolux_DockDoor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DockDoorID] [varchar](50) NOT NULL,
	[Remark] [varchar](max) NULL,
	[Locate] [varchar](50) NULL,
	[IP] [varchar](20) NOT NULL,
	[CaptionPanelIP] [varchar](20) NOT NULL,
	[FilterCode] [varchar](20) NOT NULL,
	[Alarm] [char](1) NOT NULL,
	[ContainerID] [varchar](11) NOT NULL,
	[ContainerStatus] [char](1) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[Creater] [nvarchar](50) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[Updater] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Innolux_Filter]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Innolux_Filter](
	[FilterCode] [varchar](20) NOT NULL,
	[FilterName] [varchar](50) NOT NULL,
	[FilterRules] [varchar](100) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Innolux_TagLog]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Innolux_TagLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DockDoorID] [varchar](50) NOT NULL,
	[IP] [varchar](20) NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Status] [varchar](1) NOT NULL,
	[Msg] [varchar](max) NOT NULL,
	[Data] [text] NOT NULL,
	[PostData] [text] NOT NULL,
 CONSTRAINT [PK_Innolux_TagLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Menu]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[MenuNo] [varchar](10) NOT NULL,
	[MenuName] [nvarchar](50) NOT NULL,
	[MenuLink] [nvarchar](100) NULL,
	[Type] [nvarchar](10) NOT NULL,
	[MenuDesc] [ntext] NULL,
	[OrderID] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[MenuIco] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[MenuNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SysParams]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SysParams](
	[ParaCode] [varchar](50) NOT NULL,
	[ParaValue] [varchar](50) NOT NULL,
	[ParaDesc] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Account] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsSuper] [bit] NOT NULL,
	[IsLock] [bit] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserLoginLog]    Script Date: 2017/10/5 下午 12:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLoginLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Account] [nvarchar](50) NOT NULL,
	[IP] [varchar](20) NOT NULL,
	[LoginDate] [datetime] NOT NULL,
 CONSTRAINT [PK_UserLoginLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Innolux_DockDoor] ON 

INSERT [dbo].[Innolux_DockDoor] ([ID], [DockDoorID], [Remark], [Locate], [IP], [CaptionPanelIP], [FilterCode], [Alarm], [ContainerID], [ContainerStatus], [CreateTime], [Creater], [UpdateTime], [Updater]) VALUES (1, N'Door1-1', N'進貨
出貨
廠內調撥', N'一樓碼頭1', N'192.168.1.194', N'192.168.1.222', N'R1', N' ', N'', N' ', CAST(N'2017-07-27T00:00:00.000' AS DateTime), N'admin', CAST(N'2017-08-22T14:34:38.703' AS DateTime), N'admin')
INSERT [dbo].[Innolux_DockDoor] ([ID], [DockDoorID], [Remark], [Locate], [IP], [CaptionPanelIP], [FilterCode], [Alarm], [ContainerID], [ContainerStatus], [CreateTime], [Creater], [UpdateTime], [Updater]) VALUES (11, N'Door1-2', N'', N'', N'192.168.1.195', N'192.168.1.223', N'R1', N' ', N'', N' ', CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'admin')
SET IDENTITY_INSERT [dbo].[Innolux_DockDoor] OFF
INSERT [dbo].[Innolux_Filter] ([FilterCode], [FilterName], [FilterRules]) VALUES (N'R1', N'棧板', N'L0')
INSERT [dbo].[Innolux_Filter] ([FilterCode], [FilterName], [FilterRules]) VALUES (N'R2', N'棧板、箱子', N'LA,CC,BB,AA')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'00', N'主頁', N'Home/Index', N'Page', NULL, 1, 1, N'fa-home')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'01', N'系統管理', NULL, N'System', NULL, 3, 1, N'fa-cogs')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0101', N'使用者帳號維護', N'User/List', N'Page', NULL, 1, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0102', N'功能選單維護', N'Menu/List', N'Page', NULL, 2, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0103', N'群組權限維護', N'Group/List', N'Page', NULL, 3, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0104', N'系統參數設定', N'SysParams/List', N'Page', NULL, 4, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0105', N'帳號登入明細', N'SysLog/List', N'Page', NULL, 5, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'02', N'RFID管理', NULL, N'System', NULL, 2, 1, N'fa-bullseye')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0201', N'碼頭設定作業', N'DockDoor/List', N'Page', NULL, 6, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0202', N'讀取紀錄明細', N'TagLog/List', N'Page', NULL, 7, 1, N'fa-caret-right')
INSERT [dbo].[Menu] ([MenuNo], [MenuName], [MenuLink], [Type], [MenuDesc], [OrderID], [Enabled], [MenuIco]) VALUES (N'0204', N'白名單規則', N'TagRule/List', N'Page', NULL, 9, 1, N'fa-caret-right')
INSERT [dbo].[SysParams] ([ParaCode], [ParaValue], [ParaDesc]) VALUES (N'DefaultUserPassword', N'123456', N'建立或初始使用者帳戶時的預設密碼')
INSERT [dbo].[SysParams] ([ParaCode], [ParaValue], [ParaDesc]) VALUES (N'ClearAlarmPassword', N'123', N'關閉碼頭警報的驗証密碼')
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([ID], [Email], [Password], [Account], [Name], [IsSuper], [IsLock], [LastLoginDate], [CreateDate]) VALUES (1, N'jones@epcsolutionsinc.com.tw', N'e10adc3949ba59abbe56e057f20f883e', N'admin', N'系統管理員', 1, 0, CAST(N'2017-09-22T16:43:56.377' AS DateTime), CAST(N'2017-05-12T06:03:21.370' AS DateTime))
SET IDENTITY_INSERT [dbo].[User] OFF
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Admin]    Script Date: 2017/10/5 下午 12:57:57 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [IX_Admin] UNIQUE NONCLUSTERED 
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [e0615] SET  READ_WRITE 
GO
