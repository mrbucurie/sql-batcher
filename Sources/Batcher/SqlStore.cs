using System.Collections.Generic;
using Batcher.Stores;

namespace Batcher
{
	public abstract class SqlStore : ISqlQuery
	{
		#region ISqlQuery
		public string StoreName { get; protected set; }

		public abstract SqlQuery GetQuery(); 
		#endregion


		#region Tables and views
		public static SqlStore<T> For<T>()
		{
			return new SqlStore<T>();
		}

		public static SqlStore<T> For<T>(string tableOrViewName)
		{
			return new SqlStore<T>(tableOrViewName);
		}

		public static SqlStore<T> ForAnonymous<T>(T anonymousObject, string tableOrViewName)
		{
			return new SqlStore<T>(tableOrViewName);
		} 
		#endregion


		#region Functions
		public static SqlStore<T> ForFct<T>(IEnumerable<object> functionParameters = null)
		{
			return new SqlFunctionStore<T>(functionParameters);
		}

		public static SqlStore<T> ForFct<T>(string functionName, IEnumerable<object> functionParameters = null)
		{
			return new SqlFunctionStore<T>(functionName, functionParameters);
		}

		public static SqlStore<T> ForFctForAnonymous<T>(T anonymousObject, IEnumerable<object> functionParameters = null)
		{
			return new SqlFunctionStore<T>(functionParameters);
		}

		public static SqlStore<T> ForFctForAnonymous<T>(T anonymousObject, string functionName, IEnumerable<object> functionParameters = null)
		{
			return new SqlFunctionStore<T>(functionName, functionParameters);
		} 
		#endregion
	}
}