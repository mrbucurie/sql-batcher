namespace Batcher.Internals
{
	internal class SqlScopeIdentity : ISqlQuery
	{
		public SqlQuery GetQuery()
		{
			return new SqlQuery("SCOPE_IDENTITY()");
		}
	}
}