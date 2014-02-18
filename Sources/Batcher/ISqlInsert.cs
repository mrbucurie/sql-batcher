using System.Collections.Generic;

namespace Batcher
{
	public interface ISqlInsert
	{
		ISqlInsertOutput Values<T>(T item);
		ISqlInsertOutput ValuesBatch<T>(IEnumerable<T> items);
	}

	public interface ISqlInsertOutput : IExecutableSqlQuery
	{
		IExecutableSqlQuery Output(params ISqlColumn[] columns);
	}
}
