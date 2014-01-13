namespace Batcher
{
	public interface ICompatibileExecutableSqlQuery : IExecutableSqlQuery
	{
		SqlQuery GetQuery(Compatibility compatibilityMode);
	}
}