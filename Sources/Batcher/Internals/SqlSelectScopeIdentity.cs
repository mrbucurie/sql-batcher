using System.Globalization;

namespace Batcher.Internals
{
	internal class SqlSelectScopeIdentity : IExecutableSqlQuery
	{
		private readonly string _alias;

		public SqlSelectScopeIdentity(string alias)
		{
			this._alias = Utility.StripOfSquareBrackets(alias);
		}

		public SqlQuery GetQuery()
		{
			return new SqlQuery(string.Format(CultureInfo.InvariantCulture, "SELECT SCOPE_IDENTITY() as [{0}]", this._alias));
		}
	}
}