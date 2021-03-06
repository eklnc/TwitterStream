USE [master]
GO
/****** Object:  Database [TWITTERSTREAM]    Script Date: 26.01.2017 16:41:36 ******/
CREATE DATABASE [TWITTERSTREAM]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TWITTERSTREAM', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.QIDAN\MSSQL\DATA\TWITTERSTREAM.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TWITTERSTREAM_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.QIDAN\MSSQL\DATA\TWITTERSTREAM_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TWITTERSTREAM] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TWITTERSTREAM].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TWITTERSTREAM] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET ARITHABORT OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TWITTERSTREAM] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TWITTERSTREAM] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TWITTERSTREAM] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TWITTERSTREAM] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TWITTERSTREAM] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET RECOVERY FULL 
GO
ALTER DATABASE [TWITTERSTREAM] SET  MULTI_USER 
GO
ALTER DATABASE [TWITTERSTREAM] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TWITTERSTREAM] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TWITTERSTREAM] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TWITTERSTREAM] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'TWITTERSTREAM', N'ON'
GO
USE [TWITTERSTREAM]
GO
/****** Object:  Table [dbo].[T_TS_CATEGORY]    Script Date: 26.01.2017 16:41:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_TS_CATEGORY](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](max) NOT NULL,
	[CategoryTrackKeywords] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_T_TS_CATEGORY] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[T_TS_EXCEPTION]    Script Date: 26.01.2017 16:41:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_TS_EXCEPTION](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExceptionGuid] [nvarchar](50) NOT NULL,
	[ExceptionType] [nvarchar](100) NOT NULL,
	[ExceptionMessage] [nvarchar](max) NOT NULL,
	[StackTrace] [nvarchar](max) NOT NULL,
	[ExceptionData] [nvarchar](max) NOT NULL,
	[ExceptionSource] [nvarchar](max) NOT NULL,
	[ExceptionStatus] [nvarchar](max) NULL,
 CONSTRAINT [PK_T_TS_ERROR] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[T_TS_PARAMETERS]    Script Date: 26.01.2017 16:41:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_TS_PARAMETERS](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParamaterName] [nvarchar](max) NOT NULL,
	[ParameterValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_T_TS_PARAMETERS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[T_TS_TWEETS]    Script Date: 26.01.2017 16:41:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_TS_TWEETS](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tweet] [nvarchar](max) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[CreateTime] [nvarchar](100) NOT NULL,
	[Coordinates] [nvarchar](50) NULL,
	[Place] [nvarchar](100) NULL,
	[CategoryId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_T_TS_TWEETS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
USE [master]
GO
ALTER DATABASE [TWITTERSTREAM] SET  READ_WRITE 
GO
