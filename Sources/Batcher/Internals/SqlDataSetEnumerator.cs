using System.Collections;
using System.Collections.Generic;

namespace Batcher.Internals
{
	internal class SqlDataSetEnumerator<T> : IEnumerator<T>
	{
		#region Private members
		private readonly SqlDataSets _dataSetsOwner;

		private readonly IEnumerator<T> _itemsEnumerator;
		#endregion


		#region .ctor
		public SqlDataSetEnumerator(SqlDataSets dataSetsOwner, IEnumerator<T> itemsEnumerator)
		{
			this._dataSetsOwner = dataSetsOwner;
			this._itemsEnumerator = itemsEnumerator;
		}
		#endregion


		#region IEnumerator
		public void Dispose()
		{
			this._itemsEnumerator.Dispose();
			this._dataSetsOwner.SkipSet();
		}

		public bool MoveNext()
		{
			return this._itemsEnumerator.MoveNext();
		}

		public void Reset()
		{
			this._itemsEnumerator.Reset();
		}

		public T Current { get { return this._itemsEnumerator.Current; } }

		object IEnumerator.Current { get { return this.Current; } }
		#endregion
	}
}