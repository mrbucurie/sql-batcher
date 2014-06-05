using System.Globalization;
using Batcher.QueryBuilder;

namespace Batcher.Columns
{
	public abstract class SqlFuncColumn : ISqlColumn
	{
		#region Private methods
		private readonly string _func;

		private readonly ISqlColumn _column;
		#endregion


		#region .ctor
		protected SqlFuncColumn(string func)
		{
			_func = func;
		}

		protected SqlFuncColumn(string func, ISqlColumn column)
			: this(func)
		{
			_column = column;
		}
		#endregion


		#region ISqlColumn
		public SqlColumnAlias As(string asName)
		{
			return new SqlColumnAlias(this, asName);
		}

		public SqlQuery GetQuery()
		{
			if (this._column == null)
			{
				return new SqlQuery(string.Format(CultureInfo.InvariantCulture, "{0}()", _func));
			}
			SqlQueryAppender appender = SqlQueryAppender.Create();
			appender.Append(string.Format(CultureInfo.InvariantCulture, "{0}(", _func));
			appender.Append(_column.GetQuery());
			appender.Append(")");
			return appender.GetQuery();
		}
		#endregion
	}
}