using System.Collections.Generic;

namespace Batcher
{
	public class SelectQueryOptions
	{
		public ISqlFilter Where { get; set; }

		public IList<SqlSort> Sorts { get; private set; }

		public int Skip { get; set; }

		public int? Take { get; set; }

		public SelectQueryOptions()
		{
			this.Sorts = new List<SqlSort>();
		}
	}
}