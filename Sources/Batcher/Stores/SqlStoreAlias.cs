using System;
using System.Globalization;
using System.Linq.Expressions;
using Batcher.Columns;
using Batcher.QueryBuilder;

namespace Batcher.Stores
{
	public class SqlStoreAlias<T> : SqlStore<T>, ISqlAlias
	{
		#region Properties
		public string AsName { get; private set; }

		public override SqlColumn this[Expression<Func<T, object>> selector] { get { return SqlColumn.From(this, selector); } }

		public override SqlColumn Wildcard { get { return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.*", this.AsName)); } }
		#endregion


		#region .ctor
		internal SqlStoreAlias(string storeName, string asAliasName)
			: base(storeName)
		{
			this.AsName = asAliasName;
		}
		#endregion


		#region Public methods
		public override SqlQuery GetQuery()
		{
			var appender = SqlQueryAppender.Create();
			appender.Append(base.GetQuery());
			appender.Append(string.Format(CultureInfo.InvariantCulture, " AS {0}", this.AsName));
			return appender.GetQuery();
		} 
		#endregion
	}
}