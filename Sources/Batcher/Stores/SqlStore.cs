using System;
using System.Globalization;
using System.Linq.Expressions;
using Batcher.Columns;
using Batcher.Internals;

namespace Batcher.Stores
{
	public class SqlStore<T> : SqlStore
	{
		#region Properties
		public virtual SqlColumn this[Expression<Func<T, object>> selector] { get { return SqlColumn.From(this, selector); } }

		public virtual SqlColumn Wildcard { get { return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.*", this.StoreName)); } }
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