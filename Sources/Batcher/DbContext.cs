using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Batcher.Internals;

namespace Batcher
{
	public abstract class DbContext : IDisposable
	{
		#region Private members
		private bool _disposed;
		#endregion


		#region Properties
		public SqlConnection Connection { get; protected set; }

		protected abstract Compatibility CompatibilityMode { get; }
		#endregion


		#region .ctor
		protected DbContext(string connectionString, bool openConnection = true)
		{
			this.Connection = new SqlConnection(connectionString);
			if (openConnection)
			{
				OpenConnection();
			}
		}
		#endregion

		
		#region Protected methods
		protected void OpenConnection()
		{
			if (this.Connection.State == System.Data.ConnectionState.Closed || this.Connection.State == System.Data.ConnectionState.Broken)
			{
				this.Connection.Open();
			}
		}

		protected async Task OpenConnectionAsync()
		{
			if (this.Connection.State == System.Data.ConnectionState.Closed || this.Connection.State == System.Data.ConnectionState.Broken)
			{
				await this.Connection.OpenAsync();
			}
		}
		#endregion


		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (!this._disposed)
			{
				if (isDisposing)
				{
					this.Connection.Dispose();
				}
				_disposed = true;
			}
		}
		#endregion


		#region Execute
		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteReader.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public SqlDataSets Execute(IExecutableSqlQuery query)
		{
			return SqlDataSets.Get(Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode));
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteReaderAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<SqlDataSets> ExecuteAsync(IExecutableSqlQuery query)
		{
			return await SqlDataSets.GetAsync(Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode));
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteReaderAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="cancellationToken">The cancellation instruction.</param>
		/// <returns></returns>
		public async Task<SqlDataSets> ExecuteAsync(IExecutableSqlQuery query, CancellationToken cancellationToken)
		{
			return await SqlDataSets.GetAsync(Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode), cancellationToken);
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteNonQuery.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public int ExecuteNonQuery(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteNonQueryAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<int> ExecuteNonQueryAsync(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return await command.ExecuteNonQueryAsync();
			}
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteNonQueryAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="cancellationToken">The cancellation instruction.</param>
		/// <returns></returns>
		public async Task<int> ExecuteNonQueryAsync(IExecutableSqlQuery query, CancellationToken cancellationToken)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return await command.ExecuteNonQueryAsync(cancellationToken);
			}
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteScalar.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public object ExecuteScalar(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return command.ExecuteScalar();
			}
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteScalarAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<object> ExecuteScalarAsync(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return await command.ExecuteScalarAsync();
			}
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteScalarAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="cancellationToken">The cancellation instruction.</param>
		/// <returns></returns>
		public async Task<object> ExecuteScalarAsync(IExecutableSqlQuery query, CancellationToken cancellationToken)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return await command.ExecuteScalarAsync(cancellationToken);
			}
		}

		/// <summary>
		/// Executes the query and returns only the first row of the data set.<para/>
		/// Returns null when if data is available.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public T GetFirst<T>(IExecutableSqlQuery query)
			where T : class, new()
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSet<T>().FirstOrDefault();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetFirst(query).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<T> GetFirstAsync<T>(IExecutableSqlQuery query)
			where T : class, new()
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSet<T>().FirstOrDefault();
			}
		}

		/// <summary>
		/// Executes the query and returns a SqlDataReader. This is equivalent with SqlCommand.ExecuteReader.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public SqlDataReader ExecuteReader(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return command.ExecuteReader();
			}
		}

		/// <summary>
		/// Executes the query and returns a SqlDataReader. This is equivalent with SqlCommand.ExecuteReaderAsync.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<SqlDataReader> ExecuteReaderAsync(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query, this.CompatibilityMode))
			{
				return await command.ExecuteReaderAsync();
			}
		}
		#endregion


		#region GetResult
		/// <summary>
		/// Executes the query and returns the only first data set.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public IList<T> GetResult<T>(IExecutableSqlQuery query)
			where T : class, new()
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSet<T>().ToList();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetResult(query).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<IList<T>> GetResultAsync<T>(IExecutableSqlQuery query)
			where T : class, new()
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSet<T>().ToList();
			}
		}

		/// <summary>
		/// Executes the query and returns the only first data set.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="mapper">The mapping delegate</param>
		/// <returns></returns>
		public IList<TMap> GetResult<T, TMap>(IExecutableSqlQuery query, Func<T, TMap> mapper)
			where T : class, new()
			where TMap : class, new()
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSet<T>().Select(mapper).ToList();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetResult(query,mapper).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="mapper">The mapping delegate</param>
		/// <returns></returns>
		public async Task<IList<TMap>> GetResultAsync<T, TMap>(IExecutableSqlQuery query, Func<T, TMap> mapper)
			where T : class, new()
			where TMap : class, new()
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSet<T>().Select(mapper).ToList();
			}
		}
		#endregion


		#region GetPage
		/// <summary>
		/// Executes the query and returns the first 2 data sets, assuming the first is the page data and the sencond is the total count.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public PagedData<T> GetPage<T>(IExecutableSqlQuery query)
			where T : class, new()
		{
			PagedData<T> result = new PagedData<T>();

			using (var dataSets = this.Execute(query))
			{
				result.PageData = dataSets.GetSet<T>().ToList();
				result.TotalCount = dataSets.GetSetBase<int>().FirstOrDefault();
			}
			return result;
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetPage(query).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<PagedData<T>> GetPageAsync<T>(IExecutableSqlQuery query)
			where T : class, new()
		{
			PagedData<T> result = new PagedData<T>();

			using (var dataSets = await this.ExecuteAsync(query))
			{
				result.PageData = dataSets.GetSet<T>().ToList();
				result.TotalCount = dataSets.GetSetBase<int>().FirstOrDefault();
			}
			return result;
		}

		/// <summary>
		/// Executes the query and returns the first 2 data sets, assuming the first is the page data and the sencond is the total count.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="mapper">The mapping delegate</param>
		/// <returns></returns>
		public PagedData<TMap> GetPage<T, TMap>(IExecutableSqlQuery query, Func<T, TMap> mapper)
			where T : class, new()
			where TMap : class, new()
		{
			PagedData<TMap> result = new PagedData<TMap>();

			using (var dataSets = this.Execute(query))
			{
				result.PageData = dataSets.GetSet<T>().Select(mapper).ToList();
				result.TotalCount = dataSets.GetSetBase<int>().FirstOrDefault();
			}
			return result;
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetPage(query, mapper).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="mapper">The mapping delegate</param>
		/// <returns></returns>
		public async Task<PagedData<TMap>> GetPageAsync<T, TMap>(IExecutableSqlQuery query, Func<T, TMap> mapper)
			where T : class, new()
			where TMap : class, new()
		{
			PagedData<TMap> result = new PagedData<TMap>();

			using (var dataSets = await this.ExecuteAsync(query))
			{
				result.PageData = dataSets.GetSet<T>().Select(mapper).ToList();
				result.TotalCount = dataSets.GetSetBase<int>().FirstOrDefault();
			}
			return result;
		}
		#endregion


		#region Base types
		/// <summary>
		/// Executes the query and returns the only first data set, taking only the first column as a base type (e.g: string, int, Guid, DateTime, etc.).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public IList<T> GetResultBase<T>(IExecutableSqlQuery query)
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSetBase<T>().ToList();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetResultBase(query).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<IList<T>> GetResultBaseAsync<T>(IExecutableSqlQuery query)
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSetBase<T>().ToList();
			}
		}

		/// <summary>
		/// Executes the query and returns only the first column of first row of the data set as a base type (e.g: string, int, Guid, DateTime, etc.)..<para/>
		/// Returns default when if data is available.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public T GetFirstBase<T>(IExecutableSqlQuery query)
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSetBase<T>().FirstOrDefault();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.GetFirstBase(query).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public async Task<T> GetFirstBaseAsync<T>(IExecutableSqlQuery query)
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSetBase<T>().FirstOrDefault();
			}
		}
		#endregion


		#region Anonymous
		/// <summary>
		/// Executes the query and returns the first 2 data sets, assuming the first is the page data and the sencond is the total count.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public PagedData<T> AnonymousGetPage<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class, new()
		{
			PagedData<T> result = new PagedData<T>();

			using (var dataSets = this.Execute(query))
			{
				result.PageData = dataSets.GetSetAnonymous(defaultValue).ToList();
				result.TotalCount = dataSets.GetSetBase<int>().FirstOrDefault();
			}
			return result;
		}

		/// <summary>
		/// An asynchronous version of the DbContext.AnonymousGetPage(query, defaultValue).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public async Task<PagedData<T>> AnonymousGetPageAsync<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class, new()
		{
			PagedData<T> result = new PagedData<T>();

			using (var dataSets = await this.ExecuteAsync(query))
			{
				result.PageData = dataSets.GetSetAnonymous(defaultValue).ToList();
				result.TotalCount = dataSets.GetSetBase<int>().FirstOrDefault();
			}
			return result;
		}

		/// <summary>
		/// Executes the query and returns the only first data set.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public IList<T> AnonymousGetResult<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSetAnonymous(defaultValue).ToList();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.AnonymousGetResult(query, defaultValue).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public async Task<IList<T>> AnonymousGetResultAsync<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSetAnonymous(defaultValue).ToList();
			}
		}

		/// <summary>
		/// Executes the query and returns only the first data set. <para/>
		/// Returns null when if data is available.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public T AnonymousGetFirst<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSetAnonymous(defaultValue).FirstOrDefault();
			}
		}

		/// <summary>
		/// An asynchronous version of the DbContext.AnonymousGetFirst(query, defaultValue).
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public async Task<T> AnonymousGetFirstAsyn<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class
		{
			using (var dataSets = await this.ExecuteAsync(query))
			{
				return dataSets.GetSetAnonymous(defaultValue).FirstOrDefault();
			}
		}
		#endregion
	}
}