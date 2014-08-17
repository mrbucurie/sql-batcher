using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

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
			return SqlDataSets.Get(Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters));
		}


		public static async Task<SqlDataSets> SPAsync<T>(this T dbContext, string storedProcedureName)
			where T : DbContext
		{
			return await SPAsync(dbContext, storedProcedureName, null);
		}

		public static async Task<SqlDataSets> SPAsync<T>(this T dbContext, string storedProcedureName, object parameters)
			where T : DbContext
		{
			return await SPAsync(dbContext, storedProcedureName, parameters, CancellationToken.None);
		}

		public static async Task<SqlDataSets> SPAsync<T>(this T dbContext, string storedProcedureName, object parameters, CancellationToken cancellationToken)
			where T : DbContext
		{
			return await SqlDataSets.GetAsync(Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters), cancellationToken);
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


		public static async Task<int> SPNonQueryAsync<T>(this T dbContext, string storedProcedureName)
			where T : DbContext
		{
			return await SPNonQueryAsync(dbContext, storedProcedureName, null, CancellationToken.None);
		}

		public static async Task<int> SPNonQueryAsync<T>(this T dbContext, string storedProcedureName, object parameters)
			where T : DbContext
		{
			return await SPNonQueryAsync(dbContext, storedProcedureName, parameters, CancellationToken.None);
		}

		public static async Task<int> SPNonQueryAsync<T>(this T dbContext, string storedProcedureName, object parameters, CancellationToken cancelationToken)
			where T : DbContext
		{
			using (SqlCommand command = Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters))
			{
				return await command.ExecuteNonQueryAsync(cancelationToken);
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

		public static async Task<object> SPScalarAsync<T>(this T dbContext, string storedProcedureName)
			where T : DbContext
		{
			return await SPScalarAsync(dbContext, storedProcedureName, null);
		}

		public static async Task<object> SPScalarAsync<T>(this T dbContext, string storedProcedureName, object parameters)
			where T : DbContext
		{
			return await SPScalarAsync(dbContext, storedProcedureName, parameters, CancellationToken.None);
		}

		public static async Task<object> SPScalarAsync<T>(this T dbContext, string storedProcedureName, object parameters, CancellationToken cancellationToken)
			where T : DbContext
		{
			using (SqlCommand command = Internals.Utility.GetSPCommand(dbContext, storedProcedureName, parameters))
			{
				return await command.ExecuteScalarAsync(cancellationToken);
			}
		}
	}
}