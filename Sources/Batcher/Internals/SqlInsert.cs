using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Batcher.Columns;
using Batcher.QueryBuilder;

namespace Batcher.Internals
{
	internal class SqlInsert : ISqlInsert, ISqlInsertOutput
	{
		#region Private members
		private readonly SqlStore _inStore;

		private object _values;

		private SqlColumn[] _outputColumns;

		private bool _isValuesCollection;
		#endregion


		#region .ctor
		public SqlInsert(SqlStore inStore)
		{
			_inStore = inStore;
		}
		#endregion


		#region ISqlQuery
		public ISqlInsertOutput Values<T>(T rowValues)
		{
			this._values = rowValues;
			this._isValuesCollection = false;
			return this;
		}

		public ISqlInsertOutput ValuesBatch<T>(IEnumerable<T> rowsValues)
		{
			this._values = rowsValues;
			this._isValuesCollection = true;
			return this;
		}

		public IExecutableSqlQuery Output(params SqlColumn[] columns)
		{
			this._outputColumns = columns;
			if (this._outputColumns == null || this._outputColumns.Length == 0)
			{
				this._outputColumns = new[] { new SqlColumn("[INSERTED].*") };
			}
			return this;
		}

		public SqlQuery GetQuery()
		{
			if (this._values == null)
			{
				throw new InvalidOperationException("Inserted values cannot be null.");
			}

			SqlQueryAppender appender = SqlQueryAppender.Create();

			appender.Append("INSERT INTO ");
			appender.Append(this._inStore.GetQuery());
			appender.AppendLine();

			IList<SqlColumnMetadata> columnsMetadata;

			if (this._isValuesCollection)
			{
				IEnumerator valuesEnumerator = ((IEnumerable)this._values).GetEnumerator();
				if (!valuesEnumerator.MoveNext())
				{
					throw new InvalidOperationException("Values cannot be empty.");
				}
				
				columnsMetadata = SqlColumnMetadata.GetWriteableColumns(valuesEnumerator.Current).Where(c => !c.IsInsertIdentity).ToList();

				AppendValuesColumns(columnsMetadata, appender);
				appender.AppendLine();

				AppendOutput(this._outputColumns, appender);

				appender.AppendLine("VALUES");

				AppendRowValues(columnsMetadata, valuesEnumerator.Current, appender);

				while (valuesEnumerator.MoveNext())
				{
					appender.AppendLine(",");
					AppendRowValues(columnsMetadata, valuesEnumerator.Current, appender);
				}
			}
			else
			{
				columnsMetadata = SqlColumnMetadata.GetWriteableColumns(this._values).Where(c => !c.IsInsertIdentity).ToList();

				AppendValuesColumns(columnsMetadata, appender);
				appender.AppendLine();

				AppendOutput(this._outputColumns, appender);

				appender.AppendLine("VALUES");

				AppendRowValues(columnsMetadata, this._values, appender);
			}

			return appender.GetQuery();
		}
		#endregion


		#region Private methods
		private static void AppendValuesColumns(IEnumerable<SqlColumnMetadata> valueProperties, SqlQueryAppender appender)
		{
			appender.Append("([");
			appender.Append(string.Join("],[", valueProperties.Select(p => p.PropertyInfo.Name)));
			appender.Append("])");
		}

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

		private static void AppendRowValues(IEnumerable<SqlColumnMetadata> valueProperties, object value, SqlQueryAppender appender)
		{
			appender.Append("(");
			appender.AppendParams(valueProperties.Select(vp => vp.PropertyInfo.GetValue(value)));
			appender.AppendLine(")");
		}
		#endregion
	}
}