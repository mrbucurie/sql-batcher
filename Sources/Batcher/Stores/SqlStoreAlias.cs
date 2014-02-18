using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Batcher.Columns;
using Batcher.Internals;
using Batcher.QueryBuilder;

namespace Batcher.Stores
{
	public class SqlStoreAlias<T> : SqlStore<T>, ISqlAlias
	{
		#region Properties
		public string AsName { get; private set; }

		public override SqlColumn this[Expression<Func<T, object>> selector] { get { return SqlColumn.From(this, selector); } }

		public override ISqlColumn Wildcard { get { return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.*", this.AsName)); } }

		public override IEnumerable<ISqlColumn> AllColumns { get { return SqlColumnMetadata.GetReadableColumnNames<T>().Select(columnName => SqlColumn.From(this, columnName)); } }
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