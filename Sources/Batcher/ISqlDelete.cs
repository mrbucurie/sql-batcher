using Batcher.Columns;

namespace Batcher
{
	public interface ISqlDelete : IExecutableSqlQuery
	{
		ISqlDelete Where(ISqlFilter whereCriteria);
		ISqlDelete Output(params SqlColumn[] columns);
	}
}
