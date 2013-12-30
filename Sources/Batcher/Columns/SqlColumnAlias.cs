using System.Globalization;
using Batcher.QueryBuilder;

namespace Batcher.Columns
{
	public class SqlColumnAlias : ISqlColumn, ISqlAlias
	{
		#region Private methods
		private readonly ISqlColumn _column;
		#endregion


		#region Properties
		public string AsName { get; private set; }
		#endregion


		#region .ctor
		internal SqlColumnAlias(ISqlColumn column, string asName)
		{
			this._column = column;
			this.AsName = asName;
		}
		#endregion


		#region Public methods
		public override string ToString()
		{
			return GetQuery().Debug();
		}

		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			appender.Append(this._column.GetQuery());
			appender.Append(string.Format(CultureInfo.InvariantCulture, " AS {0}", this.AsName));

			return appender.GetQuery();
		}
		#endregion
	}
}