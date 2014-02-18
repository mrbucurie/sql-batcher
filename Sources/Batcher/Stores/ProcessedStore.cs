using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Batcher.Columns;
using Batcher.Internals;

namespace Batcher.Stores
{
	public class ProcessedStore<T> : SqlStore
	{
		#region Properties
		public virtual SqlColumn this[Expression<Func<T, object>> selector] { get { return SqlColumn.From(this, selector); } }

		public virtual ISqlColumn Wildcard { get { return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.*", this.StoreName)); } }

		public virtual IEnumerable<ISqlColumn> AllColumns { get { return SqlColumnMetadata.GetReadableColumnNames<T>().Select(columnName => SqlColumn.From(this, columnName)); } }
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