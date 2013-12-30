using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Batcher.QueryBuilder;

namespace Batcher.Internals
{
	internal class SqlSelect : ISqlSelect
	{
		#region Private members
		private bool _includeTotal;

		private bool _distinct;

		private readonly ISqlQuery _fromStore;

		private WithHintType? _withHint;

		private readonly GroupFilter _whereCriteria;

		private readonly List<SqlSort> _sorts;

		private readonly SqlPage _page;

		private readonly List<ISqlColumn> _columns;

		private readonly List<ISqlColumn> _groupBy;
		#endregion


		#region .ctor
		public SqlSelect(ISqlQuery fromStore)
		{
			this._fromStore = fromStore;
			this._columns = new List<ISqlColumn>();
			this._whereCriteria = new GroupFilter(GroupFilterType.And);
			this._sorts = new List<SqlSort>();
			this._page = new SqlPage();
			this._groupBy = new List<ISqlColumn>();
		}
		#endregion


		#region ISqlSelect
		public ISqlSelect Distinct()
		{
			this._distinct = true;
			return this;
		}

		public ISqlSelect Columns(params ISqlColumn[] columns)
		{
			this._columns.AddRange((columns ?? new ISqlColumn[0]).Where(c => c != null));
			return this;
		}

		public ISqlSelect WithNoLock()
		{
			this._withHint = WithHintType.NoLock;
			return this;
		}

		public ISqlSelect Where(ISqlFilter whereCriteria)
		{
			this._whereCriteria.Add(whereCriteria);
			return this;
		}

		public ISqlSelect OrderByAsc(ISqlColumn column)
		{
			this._sorts.Add(new SqlSort(column));
			return this;
		}

		public ISqlSelect OrderByDesc(ISqlColumn column)
		{
			this._sorts.Add(new SqlSort(column, true));
			return this;
		}

		public ISqlSelect OrderBy(SqlSort sort)
		{
			if (sort != null)
			{
				this._sorts.Add(sort);
			}
			return this;
		}

		public ISqlSelect OrderBy(IEnumerable<SqlSort> sorts)
		{
			if (sorts != null)
			{
				this._sorts.AddRange(sorts.Where(s => s != null));
			}
			return this;
		}

		public ISqlSelect Skip(int? offset)
		{
			if (offset.HasValue)
			{
				this._page.Skip += offset.GetValueOrDefault();
			}
			return this;
		}

		public ISqlSelect Take(int? fetch)
		{
			if (fetch.HasValue)
			{
				this._page.Take = this._page.Take.GetValueOrDefault() + fetch.Value;
			}
			return this;
		}

		public ISqlSelect GroupBy(ISqlColumn column)
		{
			if (column != null)
			{
				this._groupBy.Add(column);

			}
			return this;
		}

		public ISqlSelect GroupBy(IEnumerable<ISqlColumn> columns)
		{
			if (columns != null)
			{
				this._groupBy.AddRange(columns.Where(c => c != null));
			}
			return this;
		}

		public ISqlSelect IncludeTotal()
		{
			this._includeTotal = true;
			return this;
		}

		public ISqlSelect Apply(SelectQueryOptions selectOptions)
		{
			if (selectOptions != null)
			{
				this.Where(selectOptions.Where);
				this.OrderBy(selectOptions.Sorts);
				this.Skip(selectOptions.Skip);
				this.Take(selectOptions.Take);
			}
			return this;
		}

		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			AppendSelect(appender);

			AppendWhere(appender);

			AppendSort(appender);

			AppendPage(appender);

			AppendGroupBy(appender);

			appender.AppendLine();

			AppendTotalCount(appender);

			return appender.GetQuery();
		}

		public override string ToString()
		{
			return GetQuery().Debug();
		}
		#endregion


		#region Private methods
		private void AppendSelect(SqlQueryAppender appender)
		{
			appender.Append("SELECT ");

			if (this._distinct)
			{
				appender.Append("DISTINCT ");
			}

			if (this._page.Skip == 0 && this._page.Take.GetValueOrDefault() != 0)
			{
				appender.Append(string.Format(CultureInfo.InvariantCulture, " TOP {0} ", this._page.Take.GetValueOrDefault()));
			}

			if (this._columns.Count == 0)
			{
				appender.Append(" * ");
			}
			else
			{
				appender.Append(this._columns[0].GetQuery());
				for (int i = 1; i < this._columns.Count; i++)
				{
					appender.AppendLine(",");
					appender.Append(this._columns[i].GetQuery());
				}
			}

			appender.AppendLine();
			appender.Append("FROM ");
			appender.Append(this._fromStore.GetQuery());
			if (this._withHint.HasValue)
			{
				appender.Append(string.Format(CultureInfo.InvariantCulture, " {0}", this._withHint.Value.GetSql()));
			}
			appender.AppendLine();
		}

		private void AppendWhere(SqlQueryAppender appender)
		{
			if (this._whereCriteria.HasFilters)
			{
				appender.Append("WHERE ");
				appender.Append(this._whereCriteria.GetQuery());
				appender.AppendLine();
			}
		}

		private void AppendSort(SqlQueryAppender appender)
		{
			if (this._sorts.Count != 0)
			{
				appender.Append("ORDER BY ");
				appender.Append(this._sorts[0].GetQuery());
				for (int i = 1; i < this._sorts.Count; i++)
				{
					appender.Append(",");
					appender.Append(this._sorts[i].GetQuery());
				}
				appender.AppendLine();
			}
		}

		private void AppendPage(SqlQueryAppender appender)
		{
			if (this._page.Skip != 0)
			{
				appender.Append(this._page.GetQuery());
			}
		}

		private void AppendGroupBy(SqlQueryAppender appender)
		{
			if (this._groupBy.Count != 0)
			{
				appender.Append("GROUP BY ");
				appender.Append(this._groupBy[0].GetQuery());
				for (int i = 1; i < this._groupBy.Count; i++)
				{
					appender.Append(",");
					appender.Append(this._groupBy[i].GetQuery());
				}
				appender.AppendLine();
			}
		}

		private void AppendTotalCount(SqlQueryAppender appender)
		{
			if (this._includeTotal)
			{
				appender.AppendLine("SELECT COUNT(*) AS [TotalRowsCount] FROM (");
				
				AppendSelect(appender);
				
				AppendWhere(appender);
				
				AppendGroupBy(appender);
				
				appender.AppendLine(") AS [Query]");
			}
		}
		#endregion
	}
}