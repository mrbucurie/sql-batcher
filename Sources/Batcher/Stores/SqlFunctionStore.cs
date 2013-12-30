using System.Collections.Generic;
using Batcher.Internals;
using Batcher.QueryBuilder;

namespace Batcher.Stores
{
	public class SqlFunctionStore<T> : SqlStore<T>
	{
		#region Private members
		private readonly IEnumerable<object> _functionParameters;
		#endregion


		#region .ctor
		public SqlFunctionStore(IEnumerable<object> functionParameters = null)
			: this(Utility.GetDefaultSoreName<T>(), functionParameters) { }

		public SqlFunctionStore(string storeName, IEnumerable<object> functionParameters = null)
			: base(storeName)
		{
			_functionParameters = functionParameters;
		}
		#endregion


		#region Overriden
		public override SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			appender.Append(this.StoreName);
			appender.Append("(");
			if (_functionParameters != null)
			{
				appender.AppendParams(_functionParameters);
			}
			appender.Append(")");

			return appender.GetQuery();
		} 
		#endregion
	}
}