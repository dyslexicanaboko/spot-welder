CREATE TABLE [dbo].[TypesTable](
	[PrimaryKeyInt] [INT] IDENTITY(1,1) NOT NULL,
	[DecimalCol] [DECIMAL](8, 2) NOT NULL,
	[DateCol] [DATE] NOT NULL,
	[DateTime2Col] [DATETIME2](7) NOT NULL,
	[AnsiString] [VARCHAR](255) NOT NULL,
	[UnicodeString] [NVARCHAR](255) NOT NULL,
PRIMARY KEY CLUSTERED ([PrimaryKeyInt] ASC))
