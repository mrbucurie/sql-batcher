namespace Batcher
{
	public interface ISqlFilter : ISqlQuery
	{
		bool HasFilters { get; }
	}
}