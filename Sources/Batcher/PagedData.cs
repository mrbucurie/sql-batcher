using System.Collections.Generic;

namespace Batcher
{
	public class PagedData<T>
	{
		#region Public properties
		public IList<T> PageData { get; set; }

		public int TotalCount { get; set; }
		#endregion
	}
}