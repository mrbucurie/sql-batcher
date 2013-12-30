CREATE TABLE [dbo].[BigData] (
    [ID]          INT              IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (1024)  NOT NULL,
    [UniqueID]    UNIQUEIDENTIFIER NOT NULL,
    [IsBatcher]   BIT              NULL,
    [DataContent] VARBINARY (MAX)  NULL,
    [Amount]      NUMERIC (14, 4)  NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [UpdatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_BigData] PRIMARY KEY CLUSTERED ([ID] ASC)
);

