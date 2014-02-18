namespace Batcher
{
	public interface ISqlDelete : IExecutableSqlQuery
	{
		ISqlDelete Where(ISqlFilter whereCriteria);
		ISqlDelete Output(params ISqlColumn[] columns);
	}
}
