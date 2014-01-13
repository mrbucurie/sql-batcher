using Batcher.Columns;
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