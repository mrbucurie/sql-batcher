CREATE TABLE [dbo].[legacy_data]
(
	[id]          INT              IDENTITY (1, 1) NOT NULL,
    [title]       NVARCHAR (1024)  NOT NULL,
    [unique_id]    UNIQUEIDENTIFIER NOT NULL,
    [is_batcher]   BIT              NULL,
    [data_content] VARBINARY (MAX)  NULL,
    [amount]      NUMERIC (14, 4)  NULL,
    [created_date] DATETIME         NOT NULL,
    [updated_date] DATETIME         NOT NULL,
    CONSTRAINT [PK_legacy_data] PRIMARY KEY CLUSTERED ([ID] ASC)
)
