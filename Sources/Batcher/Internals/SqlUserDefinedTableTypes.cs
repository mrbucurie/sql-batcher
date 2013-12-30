using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Batcher.Internals
{
	internal class SqlUserDefinedTableTypes<T>
		where T : DbContext
	{
		private static readonly List<TableVariableMetadata> TableVariables = new List<TableVariableMetadata>();

		public static void Initalize(T dbContext)
		{
			TableVariables.AddRange(TableVariableMetadata.GetMetaData(dbContext));
		}

		public static IEnumerable<string> GetUserDefinedTableOrderedColumns(string objectName)
		{
			string[] nameParts = objectName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

			string schemaName = nameParts.Length > 1 ? Regex.Replace(nameParts[0], "\\[|\\]", string.Empty, RegexOptions.Compiled) : "dbo";
			string tableName = schemaName + "." + Regex.Replace(nameParts[nameParts.Length - 1], "\\[|\\]", string.Empty, RegexOptions.Compiled);

			if (TableVariables == null)
			{
				throw new InvalidOperationException("SqlUserDefinedTableTypes have not been initialized!");
			}

			TableVariableMetadata definedTableType = TableVariables.FirstOrDefault(udf => string.Equals(udf.Name, tableName, StringComparison.OrdinalIgnoreCase));

			if (definedTableType == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not resolve the SqlUserDefinedTableTypes {0}!", objectName));
			}

			return definedTableType.OrderColumns;
		}
	}
}