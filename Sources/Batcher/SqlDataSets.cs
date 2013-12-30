using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Batcher.Internals;

namespace Batcher
{
	public class SqlDataSets : IDisposable
	{
		#region Private members
		private bool _isDisposed;

		private readonly SqlCommand _command;

		private readonly IDataReader _dataReader;
		#endregion


		#region .ctor
		public SqlDataSets(SqlCommand command)
		{
			this._command = command;
			this._dataReader = this._command.ExecuteReader();
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
		/// Returns the next set of data from the underlying data reader, taking only the first column.
		/// </summary>
		/// <typeparam name="T">The base type (e.g.: string, int, decimal).</typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetSetBase<T>()
		{
			return new SqlDataSet<T>(this, this._dataReader.GetBaseObjects<T>());
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
		#endregion
	}
}