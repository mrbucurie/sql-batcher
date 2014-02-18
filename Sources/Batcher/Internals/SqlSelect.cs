using System;
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

		private readonly List<SqlJoinSelect> _joins;

		private readonly GroupFilter _whereCriteria;

		private readonly List<SqlSort> _sorts;

		private readonly SqlPage _page;

		private readonly List<ISqlColumn> _columns;

		private readonly List<ISqlColumn> _groupBy;
		#endregion


		#region Properties
		public Compatibility CompatibilityMode { get; set; }
		#endregion


		#region .ctor
		public SqlSelect(ISqlQuery fromStore)
		{
			this._fromStore = fromStore;
			this._columns = new List<ISqlColumn>();
			this._joins = new List<SqlJoinSelect>();
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

		public ISqlSelect Columns(IEnumerable<ISqlColumn> columns)
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

		public ISqlSelect InnerJoin(SqlStore store, ISqlFilter on)
		{
			AddJoin(store, SqlJoinType.Inner, on);
			return this;
		}

		public ISqlSelect LeftJoin(SqlStore store, ISqlFilter on)
		{
			AddJoin(store, SqlJoinType.Left, on);
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
			return GetQuery(Compatibility.SQL2012);
		}

		public SqlQuery GetQuery(Compatibility compatibilityMode)
		{
			if (this._page.Skip > 0 && this._sorts.Count == 0)
			{
				throw new InvalidOperationException("Invalid usage of Skip. There must be at least one sort defined.");
			}

			SqlQueryAppender appender = SqlQueryAppender.Create();

			if (compatibilityMode == Compatibility.SQL2008R2)
			{
				AppendSelect2008R2(appender);
			}
			else
			{
				AppendSelect(appender);
			}

			AppendJoins(appender);

			AppendWhere(appender);

			AppendGroupBy(appender);

			appender.AppendLine();

			if (compatibilityMode == Compatibility.SQL2008R2)
			{
				if (this._page.Skip > 0)
				{
					AppendPage2008R2(ref appender);
				}
				else
				{
					AppendSort(appender);
				}
			}
			else
			{
				AppendSort(appender);

				AppendPage(appender);
			}

			AppendTotalCount(appender);

			return appender.GetQuery();
		}
		#endregion


		#region Private methods
		private void AddJoin(ISqlQuery store, SqlJoinType joinType, ISqlFilter onCriteria)
		{
			var join = new SqlJoinSelect(joinType, store).With(this._withHint).On(onCriteria);
			this._joins.Add(join);
		}

		private void AppendSelect(SqlQueryAppender appender, bool includeTop = true)
		{
			AppendSelectColumns(appender, includeTop);
			appender.AppendLine();

			AppendSelectFrom(appender);
			appender.AppendLine();
		}

		private void AppendSelectColumns(SqlQueryAppender appender, bool includeTop)
		{
			appender.Append("SELECT ");

			if (this._distinct)
			{
				appender.Append("DISTINCT ");
			}

			if (includeTop)
			{
				AppendSelectTop(appender);
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
		}

		private void AppendSelectTop(SqlQueryAppender appender)
		{
			if (this._page.Skip == 0 && this._page.Take.GetValueOrDefault() != 0)
			{
				appender.Append(string.Format(CultureInfo.InvariantCulture, " TOP {0} ", this._page.Take.GetValueOrDefault()));
			}
		}

		private void AppendSelectFrom(SqlQueryAppender appender)
		{
			appender.Append("FROM ");
			appender.Append(this._fromStore.GetQuery());
			if (this._withHint.HasValue)
			{
				appender.Append(string.Format(CultureInfo.InvariantCulture, " {0}", this._withHint.Value.GetSql()));
			}
		}

		private void AppendSelect2008R2(SqlQueryAppender appender)
		{
			AppendSelectColumns(appender, includeTop: true);

			if (this._page.Skip > 0)
			{
				//prepare row_number for paging.
				appender.AppendLine(",");
				appender.Append("ROW_NUMBER() OVER(ORDER BY ");

				appender.Append(this._sorts[0].GetQuery());
				for (int i = 1; i < this._sorts.Count; i++)
				{
					appender.Append(",");
					appender.Append(this._sorts[i].GetQuery());
				}
				appender.Append(") AS BacherPgnRowNr");
			}
			appender.AppendLine();

			AppendSelectFrom(appender);
			appender.AppendLine();
		}

		private void AppendJoins(SqlQueryAppender appender)
		{
			if (this._joins.Count != 0)
			{
				foreach (SqlJoinSelect joinSelect in this._joins)
				{
					appender.Append(joinSelect.GetQuery());
					appender.AppendLine();
				}
			}
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
				appender.AppendLine();
			}
		}

		private void AppendPage2008R2(ref SqlQueryAppender appender)
		{
			SqlQueryAppender wrapper = SqlQueryAppender.Create();
			wrapper.AppendLine("SELECT * FROM (");
			wrapper.Append(appender.GetQuery());
			wrapper.AppendLine();
			wrapper.AppendLine(") AS Paging");
			wrapper.Append("WHERE BacherPgnRowNr ");
			if (this._page.Take.HasValue)
			{
				wrapper.Append(string.Format("BETWEEN {0} AND {1}", this._page.Skip + 1, this._page.Skip + this._page.Take));
			}
			else
			{
				wrapper.Append(string.Format("> {0}", this._page.Skip));
			}

			wrapper.AppendLine();

			appender = wrapper;
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
				appender.AppendLine();

				appender.AppendLine("SELECT COUNT(*) AS [TotalRowsCount] FROM (");

				AppendSelect(appender, false);

				AppendJoins(appender);

				AppendWhere(appender);

				AppendGroupBy(appender);

				appender.AppendLine(") AS [Query]");
			}
		}
		#endregion
	}
}