CREATE TABLE [dbo].[TypesTableNullable](
	[PrimaryKeyInt] [INT] IDENTITY(1,1) NOT NULL,
	[DecimalColNullable] [DECIMAL](8, 2) NULL,
	[DateColNullable] [DATE] NULL,
	[DateTime2ColNullable] [DATETIME2](7) NULL,
	[AnsiStringNullable] [VARCHAR](255) NULL,
	[UnicodeStringNullable] [NVARCHAR](255) NULL,
	PRIMARY KEY CLUSTERED ([PrimaryKeyInt] ASC)
)
