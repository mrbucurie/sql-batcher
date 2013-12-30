using System.Collections.Generic;
using Batcher.QueryBuilder;

namespace Batcher
{
	public class QueryBatch : List<IExecutableSqlQuery>, IExecutableSqlQuery
	{
		#region .ctor
		public QueryBatch() { }

		public QueryBatch(IEnumerable<IExecutableSqlQuery> queries) : base(queries) { }
		#endregion


		#region IExecutableSqlQuery
		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			foreach (IExecutableSqlQuery query in this)
			{
				appender.Append(query.GetQuery());
				appender.AppendLine();
			}

			return appender.GetQuery();
		}
		#endregion
	}
}