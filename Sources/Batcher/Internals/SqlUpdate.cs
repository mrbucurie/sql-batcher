using System;
using System.Collections.Generic;
using System.Globalization;
using Batcher.Columns;
using Batcher.QueryBuilder;

namespace Batcher.Internals
{
	internal class SqlUpdate : ISqlUpdate, ISqlUpdateWhere
	{
		#region Private members
		private readonly SqlStore _store;

		private WithHintType? _withHint;

		private object _values;

		private ISqlFilter _whereCriteria;

		private ISqlColumn[] _outputColumns;
		#endregion


		#region .ctor
		public SqlUpdate(SqlStore store)
		{
			this._store = store;
		}
		#endregion


		#region ISqlQuery
		public ISqlUpdateValues WithRowLock()
		{
			this._withHint = WithHintType.RowLock;
			return this;
		}

		public ISqlUpdateWhere Set<T>(T values)
		{
			this._values = values;
			return this;
		}

		public ISqlUpdateOutput Where(ISqlFilter whereCriteria)
		{
			this._whereCriteria = whereCriteria;
			return this;
		}

		public IExecutableSqlQuery Output(params ISqlColumn[] columns)
		{
			this._outputColumns = columns;
			if (this._outputColumns == null || this._outputColumns.Length == 0)
			{
				this._outputColumns = new ISqlColumn[] { new SqlColumn("[INSERTED].*") };
			}
			return this;
		}

		public SqlQuery GetQuery()
		{
			if (this._values == null)
			{
				throw new InvalidOperationException("Updated values cannot be null.");
			}

			SqlQueryAppender appender = SqlQueryAppender.Create();
			
			GetSingleUpdateQuery(appender);

			return appender.GetQuery();
		}
		#endregion


		#region Private methods
		private void GetSingleUpdateQuery(SqlQueryAppender appender)
		{
			appender.Append("UPDATE ");
			appender.Append(this._store.GetQuery());
			if (this._withHint.HasValue)
			{
				appender.Append(string.Format(CultureInfo.InvariantCulture, " {0}", this._withHint.Value.GetSql()));
			}
			appender.AppendLine(" SET ");

			IList<SqlColumnMetadata> identityColumns;
			IList<SqlColumnMetadata> valueProperties = SqlColumnMetadata.GetWriteableColumnsNoIdentity(this._values, out identityColumns);

			appender.Append(string.Format(CultureInfo.InvariantCulture, "[{0}]=", valueProperties[0].Name));
			appender.AppendParam(valueProperties[0].GetValue(this._values));

			for (int i = 1; i < valueProperties.Count; i++)
			{
				appender.Append(",");
				appender.Append(string.Format(CultureInfo.InvariantCulture, "[{0}]=", valueProperties[i].Name));
				appender.AppendParam(valueProperties[i].GetValue(this._values));
			}
			appender.AppendLine();

			AppendOutput(this._outputColumns, appender);

			if (this._whereCriteria != null || (identityColumns != null && identityColumns.Count != 0))
			{
				appender.Append("WHERE ");
				bool requiresAnd = false;

				if (identityColumns != null && identityColumns.Count != 0)
				{
					requiresAnd = true;

					appender.Append(string.Format(CultureInfo.InvariantCulture, "[{0}]=", identityColumns[0].Name));
					appender.AppendParam(identityColumns[0].GetValue(this._values));

					for (int i = 1; i < identityColumns.Count; i++)
					{
						appender.Append(",");
						appender.Append(string.Format(CultureInfo.InvariantCulture, "[{0}]=", identityColumns[i].Name));
						appender.AppendParam(identityColumns[i].GetValue(this._values));
					}
					appender.AppendLine();
				}

				if (this._whereCriteria != null)
				{
					if (requiresAnd)
					{
						appender.Append("AND ");
					}
					appender.Append(this._whereCriteria.GetQuery());
					appender.AppendLine();
				}

			}
		}

		private static void AppendOutput(IEnumerable<ISqlColumn> columns, SqlQueryAppender appender)
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