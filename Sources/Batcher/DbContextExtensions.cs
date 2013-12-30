using System.Data.SqlClient;

namespace Batcher
{
	public static class DbContextExtensions
	{
		public static SqlDataSets SP<T>(this T dbContext, string storedProcedureName)
			where T : DbContext
		{
			return SP(dbContext, storedProcedureName, null);
		}

		public static SqlDataSets SP<T>(this T dbContext, string storedProcedureName, object parameters)
			where T : DbContext
		{
			return new SqlDataSets(Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters));
		}

		public static int SPNonQuery<T>(this T dbContext, string storedProcedureName)
			where T : DbContext
		{
			return SPNonQuery(dbContext, storedProcedureName, null);
		}

		public static int SPNonQuery<T>(this T dbContext, string storedProcedureName, object parameters)
			where T : DbContext
		{
			using (SqlCommand command = Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters))
			{
				return command.ExecuteNonQuery();
			}
		}

		public static object SPScalar<T>(this T dbContext, string storedProcedureName)
			where T : DbContext
		{
			return SPScalar(dbContext, storedProcedureName, null);
		}

		public static object SPScalar<T>(this T dbContext, string storedProcedureName, object parameters)
			where T : DbContext
		{
			using (SqlCommand command = Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters))
			{
				return command.ExecuteScalar();
			}
		}
	}
}