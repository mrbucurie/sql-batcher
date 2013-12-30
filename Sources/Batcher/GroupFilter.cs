using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Batcher.QueryBuilder;

namespace Batcher
{
	public class GroupFilter : IEnumerable<ISqlFilter>, ISqlFilter
	{
		private readonly List<ISqlFilter> _filters;

		#region Properties
		public GroupFilterType FilterFilterType { get; private set; }
		#endregion


		#region .ctor
		public GroupFilter(GroupFilterType filterFilterType)
		{
			this.FilterFilterType = filterFilterType;
			this._filters = new List<ISqlFilter>();
		}

		public GroupFilter(GroupFilterType filterFilterType, IEnumerable<ISqlFilter> filters)
		{
			this.FilterFilterType = filterFilterType;
			this._filters = new List<ISqlFilter>(filters);
		}
		#endregion


		#region ISqlFilter
		public bool HasFilters { get { return this._filters.Count != 0; } }

		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();
			if (this.HasFilters)
			{
				appender.Append("(");

				int i = 0;
				appender.Append(this._filters[i++].GetQuery());
				while (i < this._filters.Count)
				{
					appender.Append(string.Format(CultureInfo.InvariantCulture, " {0} ", this.FilterFilterType.ToString().ToUpperInvariant()));
					appender.Append(this._filters[i++].GetQuery());
				}

				appender.Append(")");
			}

			return appender.GetQuery();
		}
		#endregion


		#region Public methods
		public void Add(ISqlFilter filter, params ISqlFilter[] filters)
		{
			if (filter != null && filter.HasFilters)
			{
				this._filters.Add(filter);
			}

			if (filters != null)
			{
				this._filters.AddRange(filters.Where(f => f != null && f.HasFilters));
			}
		}

		public void Add(IEnumerable<ISqlFilter> filters)
		{
			if (filters != null)
			{
				this._filters.AddRange(filters.Where(filter => filter.HasFilters));
			}
		}
		#endregion


		#region IEnumerable
		public IEnumerator<ISqlFilter> GetEnumerator()
		{
			return this._filters.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}