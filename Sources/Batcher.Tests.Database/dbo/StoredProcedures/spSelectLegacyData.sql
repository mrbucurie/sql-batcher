CREATE PROCEDURE [dbo].[spSelectLegacyData]
	@LegacyDataTable [dbo].[udLegacyDataTable] READONLY
AS
	SELECT * FROM @LegacyDataTable
GO
