using System;
using System.Globalization;
using System.Linq.Expressions;
using Batcher.Columns;

namespace Batcher.Stores
{
	public class ProcessedStore<T> : SqlStore
	{
		#region Properties
		public virtual SqlColumn this[Expression<Func<T, object>> selector] { get { return SqlColumn.From(this, selector); } }

		public virtual SqlColumn Wildcard { get { return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.*", this.StoreName)); } }
		#endregion

		
		#region .ctor
		public ProcessedStore(string storeName)
		{
			this.StoreName = storeName;
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