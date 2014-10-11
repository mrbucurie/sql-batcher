CREATE TYPE [dbo].[udLegacyDataTable] AS TABLE
(
	[id]          INT,
	[title]       NVARCHAR (1024),
	[unique_id]    UNIQUEIDENTIFIER,
	[is_batcher]   BIT,
	[data_content] VARBINARY (MAX),
	[amount]      NUMERIC (14, 4),
	[created_date] DATETIME,
	[updated_date] DATETIME        
)