using System.Collections;
using System.Collections.Generic;

namespace Batcher.Internals
{
	internal class SqlDataSet<T> : IEnumerable<T>
	{
		#region Private members
		private readonly SqlDataSets _dataSetsOwner;

		private readonly IEnumerable<T> _items;
		#endregion


		#region .ctor
		public SqlDataSet(SqlDataSets dataSetsOwner, IEnumerable<T> items)
		{
			this._dataSetsOwner = dataSetsOwner;
			this._items = items;
		}
		#endregion


		#region IEnumerable
		public IEnumerator<T> GetEnumerator()
		{
			return new SqlDataSetEnumerator<T>(this._dataSetsOwner, this._items.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}