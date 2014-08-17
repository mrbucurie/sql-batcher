namespace Batcher
{
	public interface ISqlUpdate : ISqlUpdateValues
	{
		ISqlUpdateValues WithRowLock();
	}

	public interface ISqlUpdateValues
	{
		ISqlUpdateWhere Set<T>(T item);
		ISqlUpdateColumnValue Set(ISqlColumn column, object value);
	}

	public interface ISqlUpdateColumnValue : ISqlUpdateWhere
	{
		ISqlUpdateColumnValue Set(ISqlColumn column, object value);
	}

	public interface ISqlUpdateWhere : ISqlUpdateOutput
	{
		ISqlUpdateOutput Where(ISqlFilter whereCriteria);
	}

	public interface ISqlUpdateOutput : IExecutableSqlQuery
	{
		IExecutableSqlQuery Output(params ISqlColumn[] columns);
	}
}
