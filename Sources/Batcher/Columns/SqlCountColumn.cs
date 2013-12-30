using Batcher.QueryBuilder;

namespace Batcher.Columns
{
	public class SqlCountColumn : ISqlColumn
	{
		#region Private methods
		private readonly ISqlColumn _column;
		#endregion


		#region .ctor
		public SqlCountColumn() { }

		public SqlCountColumn(ISqlColumn column)
		{
			_column = column;
		}
		#endregion


		#region ISqlColumn
		public override string ToString()
		{
			return GetQuery().Debug();
		}

		public SqlColumnAlias As(string asName)
		{
			return new SqlColumnAlias(this, asName);
		}

		public SqlQuery GetQuery()
		{
			if (this._column == null)
			{
				return new SqlQuery("COUNT(*)");
			}
			SqlQueryAppender appender = SqlQueryAppender.Create();
			appender.Append("COUNT(");
			appender.Append(_column.GetQuery());
			appender.Append(")");
			return appender.GetQuery();
		}
		#endregion
	}
}