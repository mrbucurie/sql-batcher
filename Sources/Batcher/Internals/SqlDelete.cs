using System.Collections.Generic;
using Batcher.Columns;
using Batcher.QueryBuilder;

namespace Batcher.Internals
{
	internal class SqlDelete : ISqlDelete
	{
		#region Private members
		private readonly SqlStore _store;

		private ISqlFilter _whereCriteria;

		private SqlColumn[] _outputColumns;
		#endregion


		#region .ctor
		public SqlDelete(SqlStore store)
		{
			this._store = store;
		}
		#endregion


		#region ISqlDelete
		public ISqlDelete Where(ISqlFilter whereCriteria)
		{
			this._whereCriteria = whereCriteria;
			return this;
		}

		public ISqlDelete Output(params SqlColumn[] columns)
		{
			this._outputColumns = columns;
			if (this._outputColumns == null || this._outputColumns.Length == 0)
			{
				this._outputColumns = new[] { new SqlColumn("[DELETED].*") };
			}
			return this;
		}

		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			appender.Append("DELETE FROM ");
			appender.Append(this._store.GetQuery());
			appender.AppendLine();

			AppendOutput(this._outputColumns, appender);

			if (this._whereCriteria != null)
			{
				appender.Append("WHERE ");
				appender.Append(this._whereCriteria.GetQuery());
				appender.AppendLine();
			}

			return appender.GetQuery();
		}
		#endregion


		#region Private methods
		private static void AppendOutput(IEnumerable<SqlColumn> columns, SqlQueryAppender appender)
		{
			if (columns != null)
			{
				appender.Append("OUTPUT ");
				var enumerator = columns.GetEnumerator();
				if (enumerator.MoveNext())
				{
					appender.Append(enumerator.Current.GetQuery());
					while (enumerator.MoveNext())
					{
						appender.Append(",");
						appender.Append(enumerator.Current.GetQuery());
					}
				}
				appender.AppendLine();
			}
		}
		#endregion
	}
}