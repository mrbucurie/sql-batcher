using System.Globalization;
using System.Text;

namespace Batcher.Internals
{
	internal class SqlPage : ISqlQuery
	{
		#region Properties
		public int Skip { get; set; }

		public int? Take { get; set; }
		#endregion


		#region ISqlQuery
		public SqlQuery GetQuery()
		{
			StringBuilder result = new StringBuilder();
			if (this.Skip != 0 || this.Take.HasValue)
			{
				result.AppendFormat(CultureInfo.InvariantCulture, "OFFSET {0} ROWS", this.Skip);

				if (this.Take.HasValue)
				{
					result.AppendFormat(CultureInfo.InvariantCulture, " FETCH NEXT {0} ROWS ONLY", this.Take.Value);
				}
			}
			return new SqlQuery(result.ToString());
		}
		#endregion
	}
}