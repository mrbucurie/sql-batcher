using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Batcher.Columns;
using Batcher.Internals;

namespace Batcher.Stores
{
	public class SqlStore<T> : SqlStore
	{
		#region Properties
		public virtual SqlColumn this[Expression<Func<T, object>> selector] { get { return SqlColumn.From(this, selector); } }

		public virtual ISqlColumn Wildcard { get { return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.*", this.StoreName)); } }

		public virtual IEnumerable<ISqlColumn> AllColumns { get { return SqlColumnMetadata.GetReadableColumnNames<T>().Select(columnName => SqlColumn.From(this, columnName)); } }

		public virtual ProcessedStore<T> Inserted { get { return new ProcessedStore<T>("INSERTED"); } }

		public virtual ProcessedStore<T> Updated { get { return new ProcessedStore<T>("INSERTED"); } }

		public virtual ProcessedStore<T> Deleted { get { return new ProcessedStore<T>("DELETED"); } }
		#endregion


		#region .ctor
		public SqlStore() : this(Utility.GetDefaultSoreName<T>()) { }

		public SqlStore(string storeName)
		{
			this.StoreName = storeName;
		}
		#endregion


		#region Public methods
		public SqlStoreAlias<T> As(string asAliasName)
		{
			return new SqlStoreAlias<T>(this.StoreName, asAliasName);
		}
		#endregion


		#region ISqlQuery
		public override SqlQuery GetQuery()
		{
			return new SqlQuery(this.StoreName);
		}
		#endregion
	}
}