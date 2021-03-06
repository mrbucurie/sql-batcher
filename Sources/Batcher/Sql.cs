using System;
using Batcher.Columns;
using Batcher.Internals;

namespace Batcher
{
	public static class Sql
	{
		#region Queries
		public static ISqlSelect Select(SqlStore store) { return new SqlSelect(store); }

		public static ISqlInsert Insert(SqlStore store) { return new SqlInsert(store); }

		public static ISqlUpdate Update(SqlStore store) { return new SqlUpdate(store); }

		public static ISqlDelete Delete(SqlStore store) { return new SqlDelete(store); }

		public static ISqlQuery ScopeIdentity() { return new SqlScopeIdentity(); }

		public static IExecutableSqlQuery SelectScopeIdentity(string asAliasName = "ScopeIdentity")
		{
			if (string.IsNullOrWhiteSpace(asAliasName))
			{
				throw new ArgumentException("Parameter asAliasName cannot be null or empty.", "asAliasName");
			}
			return new SqlSelectScopeIdentity(asAliasName);
		}
		#endregion


		#region Columns
		public static SqlCountColumn Count() { return new SqlCountColumn(new SqlColumn("*")); }

		public static SqlCountColumn Count(ISqlColumn column) { return new SqlCountColumn(column); }

		public static SqlMaxColumn Max(ISqlColumn column) { return new SqlMaxColumn(column); }

		public static SqlMinColumn Min(ISqlColumn column) { return new SqlMinColumn(column); }
		#endregion
	}
}