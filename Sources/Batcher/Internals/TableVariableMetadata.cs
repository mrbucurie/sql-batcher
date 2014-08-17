using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Batcher.Internals
{
	public class TableVariableMetadata
	{
		#region Nested types
		private class ColumnMetadata
		{
			public string TableName { get; set; }
			public string Name { get; set; }
			public int Index { get; set; }
		}
		#endregion

		public string Name { get; set; }

		public List<string> OrderColumns { get; set; }

		internal static List<TableVariableMetadata> GetMetaData(DbContext dbContext)
		{
			using (SqlCommand cmd = dbContext.Connection.CreateCommand())
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = @"SELECT  [Schemas].[name] + '.' + [TableVars].[NAME] AS [TableName] ,
											[Columns].[NAME] AS [ColumnName] ,
											[Columns].[column_id] AS [ColumnId]
									FROM    [sys].[table_types] AS TableVars WITH(NOLOCK)
									INNER JOIN [sys].[schemas] AS [Schemas] WITH(NOLOCK) ON [TableVars].[schema_id] = [Schemas].[schema_id]
									INNER JOIN [sys].[COLUMNS] AS [Columns] WITH(NOLOCK) ON [Columns].[object_id] = [TableVars].[type_table_object_id]";

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					List<ColumnMetadata> columns = new List<ColumnMetadata>();

					while (reader.Read())
					{
						columns.Add(new ColumnMetadata
						{
							TableName = Convert.ToString(reader[0], CultureInfo.InvariantCulture),
							Name = Convert.ToString(reader[1], CultureInfo.InvariantCulture),
							Index = Convert.ToInt32(reader[2], CultureInfo.InvariantCulture),
						});
					}

					var result = new List<TableVariableMetadata>(columns.GroupBy(c => c.TableName).Select(tableColumns => new TableVariableMetadata
					{
						Name = tableColumns.Key,
						OrderColumns = tableColumns.OrderBy(c => c.Index).Select(c => c.Name).ToList()
					}));

					return result;
				}
			}
		}

		internal static TableVariableMetadata GetMetaData(DbContext dbContext, string udfName)
		{
			using (SqlCommand cmd = dbContext.Connection.CreateCommand())
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = @"SELECT  [Schemas].[name] + '.' + [TableVars].[NAME] AS [TableName] ,
											[Columns].[NAME] AS [ColumnName] ,
											[Columns].[column_id] AS [ColumnId]
									FROM    [sys].[table_types] AS TableVars WITH(NOLOCK)
									INNER JOIN [sys].[schemas] AS [Schemas] WITH(NOLOCK) ON [TableVars].[schema_id] = [Schemas].[schema_id]
									INNER JOIN [sys].[COLUMNS] AS [Columns] WITH(NOLOCK) ON [Columns].[object_id] = [TableVars].[type_table_object_id]
									WHERE ([Schemas].[name] + '.' + [TableVars].[NAME]) = '" + udfName + "'";

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					List<ColumnMetadata> columns = new List<ColumnMetadata>();

					while (reader.Read())
					{
						columns.Add(new ColumnMetadata
						{
							TableName = Convert.ToString(reader[0], CultureInfo.InvariantCulture),
							Name = Convert.ToString(reader[1], CultureInfo.InvariantCulture),
							Index = Convert.ToInt32(reader[2], CultureInfo.InvariantCulture),
						});
					}

					var result = new List<TableVariableMetadata>(columns.GroupBy(c => c.TableName).Select(tableColumns => new TableVariableMetadata
					{
						Name = tableColumns.Key,
						OrderColumns = tableColumns.OrderBy(c => c.Index).Select(c => c.Name).ToList()
					}));

					return result.FirstOrDefault();
				}
			}
		}
	}
}