using System.Collections.Generic;

namespace Batcher
{
	public interface ISqlSelect : ICompatibileExecutableSqlQuery
	{
		ISqlSelect Distinct();
		ISqlSelect WithNoLock();
		ISqlSelect Columns(params ISqlColumn[] columns);
		ISqlSelect Where(ISqlFilter whereCriteria);
		ISqlSelect OrderByAsc(ISqlColumn column);
		ISqlSelect OrderByDesc(ISqlColumn column);
		ISqlSelect OrderBy(SqlSort sort);
		ISqlSelect OrderBy(IEnumerable<SqlSort> sorts);
		ISqlSelect GroupBy(ISqlColumn column);
		ISqlSelect GroupBy(IEnumerable<ISqlColumn> columns);
		ISqlSelect Skip(int? offset);
		ISqlSelect Take(int? fetch);
		ISqlSelect Apply(SelectQueryOptions selectOptions);
		ISqlSelect IncludeTotal();
	}
}