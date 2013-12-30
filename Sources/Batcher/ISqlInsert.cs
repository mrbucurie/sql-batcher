using System.Collections.Generic;
using Batcher.Columns;

namespace Batcher
{
	public interface ISqlInsert
	{
		ISqlInsertOutput Values<T>(T rowValues);
		ISqlInsertOutput ValuesBatch<T>(IEnumerable<T> rowsValues);
	}

	public interface ISqlInsertOutput : IExecutableSqlQuery
	{
		IExecutableSqlQuery Output(params SqlColumn[] columns);
	}
}
