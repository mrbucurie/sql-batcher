using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Batcher.Internals
{
	internal static class SqlUserDefinedTableTypes<T>
		where T : DbContext
	{
		private static readonly ConcurrentDictionary<string, TableVariableMetadata> TableVariables = new ConcurrentDictionary<string, TableVariableMetadata>(StringComparer.OrdinalIgnoreCase);

		public static void Initalize(T dbContext)
		{
			var metaData = TableVariableMetadata.GetMetaData(dbContext);
			foreach (var definedTableType in metaData)
			{
				TableVariables[definedTableType.Name] = definedTableType;
			}
		}

		public static IEnumerable<string> GetUserDefinedTableOrderedColumns(T dbContext, string objectName)
		{
			string[] nameParts = objectName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

			string schemaName = nameParts.Length > 1 ? Regex.Replace(nameParts[0], "\\[|\\]", string.Empty, RegexOptions.Compiled) : "dbo";
			string tableName = schemaName + "." + Regex.Replace(nameParts[nameParts.Length - 1], "\\[|\\]", string.Empty, RegexOptions.Compiled);

			TableVariableMetadata definedTableType;
			if (!TableVariables.TryGetValue(tableName, out definedTableType))
			{
				definedTableType = TableVariableMetadata.GetMetaData(dbContext, tableName);
				if (definedTableType != null)
				{
					TableVariables[definedTableType.Name] = definedTableType;
				}
			}

			if (definedTableType == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not resolve the SqlUserDefinedTableType {0}!", objectName));
			}

			return definedTableType.OrderColumns;
		}
	}
}