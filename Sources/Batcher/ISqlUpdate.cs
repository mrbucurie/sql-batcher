using Batcher.Columns;

namespace Batcher
{
	public interface ISqlUpdate : ISqlUpdateValues
	{
		ISqlUpdateValues WithRowLock();
	}

	public interface ISqlUpdateValues
	{
		ISqlUpdateWhere Set(object values);
	}

	public interface ISqlUpdateWhere : ISqlUpdateOutput
	{
		ISqlUpdateOutput Where(ISqlFilter whereCriteria);
	}

	public interface ISqlUpdateOutput : IExecutableSqlQuery
	{
		IExecutableSqlQuery Output(params SqlColumn[] columns);
	}
}
