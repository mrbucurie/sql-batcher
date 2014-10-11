using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Batcher.Internals;

namespace Batcher
{
	public class SqlDataSets : IDisposable
	{
		#region Private members
		private bool _isDisposed;

		private readonly SqlCommand _command;

		private IDataReader _dataReader;
		#endregion


		#region .ctor
		private SqlDataSets(SqlCommand command)
		{
			this._command = command;
		}

		public static SqlDataSets Get(SqlCommand command)
		{
			var dataSets = new SqlDataSets(command);

			dataSets._dataReader = dataSets._command.ExecuteReader();

			return dataSets;
		}

		public static async Task<SqlDataSets> GetAsync(SqlCommand command)
		{
			var dataSets = new SqlDataSets(command);

			dataSets._dataReader = await dataSets._command.ExecuteReaderAsync();

			return dataSets;
		}

		public static async Task<SqlDataSets> GetAsync(SqlCommand command, CancellationToken cancellationToken)
		{
			var dataSets = new SqlDataSets(command);

			dataSets._dataReader = await dataSets._command.ExecuteReaderAsync(cancellationToken);

			return dataSets;
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
			if (!this._isDisposed)
			{
				if (isDisposing)
				{
					this._dataReader.Dispose();
					this._command.Dispose();
				}

				this._isDisposed = true;
			}
		}
		#endregion


		#region Public methods
		/// <summary>
		/// Skips the next set of data from the underlying data reader.
		/// </summary>
		public void SkipSet()
		{
			this._dataReader.NextResult();
		}

		/// <summary>
		/// Returns the next set of data from the underlying data reader.
		/// </summary>
		/// <typeparam name="T">The type in which the data is encapsulated.</typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetSet<T>()
			where T : class, new()
		{
			return GetSet(() => new T());
		}

		/// <summary>
		/// Returns the next set of data from the underlying data reader.
		/// </summary>
		/// <typeparam name="T">The type in which the data is encapsulated.</typeparam>
		/// <param name="instanceInitializer">An initializer for the object instance, in case the default constructor is not available.</param>
		/// <returns></returns>
		public IEnumerable<T> GetSet<T>(Func<T> instanceInitializer)
			where T : class
		{
			if (instanceInitializer == null)
			{
				throw new ArgumentNullException("instanceInitializer");
			}

			return new SqlDataSet<T>(this, this._dataReader.GetObjects(instanceInitializer));
		}

		/// <summary>
		/// Returns all sets of data from the underlying data reader mapped to a single type.
		/// </summary>
		/// <typeparam name="T">The type in which the data is encapsulated.</typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetUnifiedSets<T>()
			where T : class, new()
		{
			return GetUnifiedSets(() => new T());
		}

		/// <summary>
		/// Returns all sets of data from the underlying data reader mapped to a single type.
		/// </summary>
		/// <typeparam name="T">The type in which the data is encapsulated.</typeparam>
		/// <param name="instanceInitializer">An initializer for the object instance, in case the default constructor is not available.</param>
		/// <returns></returns>
		public IEnumerable<T> GetUnifiedSets<T>(Func<T> instanceInitializer)
			where T : class
		{
			return new SqlDataSet<T>(this, GetUnifiedSetsInternal(instanceInitializer).SelectMany(sets => sets));
		}

		/// <summary>
		/// Returns the next set of data from the underlying data reader, taking only the first column.
		/// </summary>
		/// <typeparam name="T">The base type (e.g.: string, int, decimal).</typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetSetBase<T>()
		{
			return new SqlDataSet<T>(this, this._dataReader.GetBaseObjects<T>());
		}

		/// <summary>
		/// Returns all sets of data from the underlying data reader, taking only the first column.
		/// </summary>
		/// <typeparam name="T">The base type (e.g.: string, int, decimal).</typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetUnifiedSetsBase<T>()
		{
			return new SqlDataSet<T>(this, GetUnifiedSetsBaseInternal<T>().SelectMany(sets => sets));
		}

		/// <summary>
		/// Returns the next set of data from the underlying data reader.
		/// </summary>
		/// <typeparam name="T">The type in which the data is encapsulated.</typeparam>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public IEnumerable<T> GetSetAnonymous<T>(T defaultValue)
		{
			return new SqlDataSet<T>(this, this._dataReader.GetAnonymous(defaultValue));
		}

		/// <summary>
		/// Returns all sets of data from the underlying data reader.
		/// </summary>
		/// <typeparam name="T">The type in which the data is encapsulated.</typeparam>
		/// <param name="defaultValue">The default anonymous object value.</param>
		/// <returns></returns>
		public IEnumerable<T> GetUnifiedSetsAnonymous<T>(T defaultValue)
		{
			return new SqlDataSet<T>(this, GetUnifiedSetsAnonymousInternal(defaultValue).SelectMany(sets => sets));
		}
		#endregion


		#region Private methods
		private IEnumerable<IEnumerable<T>> GetUnifiedSetsInternal<T>(Func<T> instanceInitializer)
			where T : class
		{
			if (instanceInitializer == null)
			{
				throw new ArgumentNullException("instanceInitializer");
			}

			yield return this._dataReader.GetObjects(instanceInitializer);
			while (this._dataReader.NextResult())
			{
				yield return this._dataReader.GetObjects(instanceInitializer);
			}
		}

		private IEnumerable<IEnumerable<T>> GetUnifiedSetsBaseInternal<T>()
		{
			yield return this._dataReader.GetBaseObjects<T>();
			while (this._dataReader.NextResult())
			{
				yield return this._dataReader.GetBaseObjects<T>();
			}
		}

		internal IEnumerable<IEnumerable<T>> GetUnifiedSetsAnonymousInternal<T>(T defaultValue)
		{
			yield return this._dataReader.GetAnonymous(defaultValue);
			while (this._dataReader.NextResult())
			{
				yield return this._dataReader.GetAnonymous(defaultValue);
			}
		}
		#endregion
	}
}