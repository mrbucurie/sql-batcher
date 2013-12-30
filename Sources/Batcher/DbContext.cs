using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
		#endregion


		#region .ctor
		protected DbContext(string connectionString)
		{
			this.Connection = new SqlConnection(connectionString);
			this.Connection.Open();
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


		#region Public methods
		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteReader.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public SqlDataSets Execute(IExecutableSqlQuery query)
		{
			return new SqlDataSets(Utility.GetTextCommand(this.Connection, query));
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteNonQuery.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public int ExecuteNonQuery(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query))
			{
				return command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes the query. This is equivalent with SqlCommand.ExecuteScalar.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <returns></returns>
		public object ExecuteScalar(IExecutableSqlQuery query)
		{
			using (var command = Utility.GetTextCommand(this.Connection, query))
			{
				return command.ExecuteScalar();
			}
		}

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
		/// Executes the query and returns the first 2 data sets, assuming the first is the page data and the sencond is the total count.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public PagedData<T> GetPageAnonymous<T>(IExecutableSqlQuery query, T defaultValue)
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
		/// Executes the query and returns the only first data set, taking only the first column.
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
		/// Executes the query and returns the only first data set.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public IList<T> GetResultAnonymous<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSetAnonymous(defaultValue).ToList();
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
		/// Executes the query and returns only the first column of first row of the data set.<para/>
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
		/// Executes the query and returns only the first data set. <para/>
		/// Returns null when if data is available.
		/// </summary>
		/// <param name="query">The query to be executed.</param>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public T GetFirstAnonymous<T>(IExecutableSqlQuery query, T defaultValue)
			where T : class
		{
			using (var dataSets = this.Execute(query))
			{
				return dataSets.GetSetAnonymous(defaultValue).FirstOrDefault();
			}
		}
		#endregion
	}
}