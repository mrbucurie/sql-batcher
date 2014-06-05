using System;
using System.Globalization;
using Batcher.Columns;
using Batcher.Internals;
using Batcher.QueryBuilder;

namespace Batcher
{
	public class SqlSort : ISqlQuery
	{
		#region Properties
		public ISqlColumn Column { get; set; }

		public bool Descending { get; set; }
		#endregion


		#region .ctor
		public SqlSort(ISqlColumn column)
		{
			this.Column = column;
		}

		public SqlSort(ISqlColumn column, bool descending)
			: this(column)
		{
			this.Descending = descending;
		}

		public SqlSort(string columnName)
		{
			this.Column = new SqlColumn(columnName);
		}

		public SqlSort(string columnName, bool descending)
			: this(columnName)
		{
			this.Descending = descending;
		}
		#endregion


		#region Public methods (static)
		public static string DetermineColumnName<T>(string propertyName)
		{
			var property = typeof(T).GetProperty(propertyName);
			if (property == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find property {0} in {1}.", propertyName, typeof(T).FullName));
			}

			return Utility.GetColumnName(property);
		}
		#endregion


		#region ISqlQuery
		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			ISqlAlias columnAlias = this.Column as ISqlAlias;
			if (columnAlias != null)
			{
				appender.Append(columnAlias.AsName);
			}
			else
			{
				appender.Append(this.Column.GetQuery());
			}
			appender.Append(this.Descending ? " DESC" : " ASC");

			return appender.GetQuery();
		}
		#endregion
	}
}