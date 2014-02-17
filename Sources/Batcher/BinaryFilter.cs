using System;
using System.Collections;
using System.Linq.Expressions;
using Batcher.Columns;
using Batcher.QueryBuilder;

namespace Batcher
{
	public class BinaryFilter : ISqlFilter
	{
		#region Properties
		public object LeftExpression { get; private set; }

		public object RightExpression { get; private set; }

		public BinaryFilterType BinaryFilterType { get; private set; }
		#endregion


		#region .ctor
		internal BinaryFilter(object left, BinaryFilterType binaryFilterType, object right)
		{
			this.LeftExpression = left;
			this.BinaryFilterType = binaryFilterType;
			this.RightExpression = right;
		}

		public static BinaryFilter ColumnValue(ISqlColumn column, BinaryFilterType binaryFilterType, object value)
		{
			return new BinaryFilter(column, binaryFilterType, value);
		}

		public static BinaryFilter ColumnValue(string columnName, BinaryFilterType binaryFilterType, object value)
		{
			return ColumnValue(new SqlColumn(columnName), binaryFilterType, value);
		}

		public static BinaryFilter ColumnValue<T>(Expression<Func<T, object>> columnSelector, BinaryFilterType binaryFilterType, object value)
		{
			return ColumnValue(SqlColumn.From(columnSelector), binaryFilterType, value);
		}
		#endregion


		#region ISqlFilter
		public virtual bool HasFilters { get { return true; } }

		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			ComputeFilter(appender);

			return appender.GetQuery();
		}
		#endregion


		#region Private methods
		private void ComputeFilter(SqlQueryAppender appender)
		{
			switch (this.BinaryFilterType)
			{
				case BinaryFilterType.Equal:
					AppendEquals(appender);
					break;
				case BinaryFilterType.NotEqual:
					AppendNotEquals(appender);
					break;
				case BinaryFilterType.Lower:
					AppendLower(appender);
					break;
				case BinaryFilterType.LowerEqual:
					AppendLowerEquals(appender);
					break;
				case BinaryFilterType.Greater:
					AppendGreater(appender);
					break;
				case BinaryFilterType.GreaterEqual:
					AppendGreaterEquals(appender);
					break;
				case BinaryFilterType.BeginsWith:
					AppendBeginsWith(appender);
					break;
				case BinaryFilterType.Contains:
					AppendContains(appender);
					break;
				case BinaryFilterType.EndsWith:
					AppendEndsWith(appender);
					break;
				case BinaryFilterType.In:
					AppendIn(appender);
					break;
				case BinaryFilterType.NotIn:
					AppendNotIn(appender);
					break;
				case BinaryFilterType.Between:
					AppendBetween(appender);
					break;
			}
		}

		private void AppendEquals(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);

			if (this.RightExpression == null)
			{
				appender.Append(" IS NULL");
			}
			else
			{
				appender.Append(" = ");
				appender.AppendExpression(this.RightExpression);
			}
		}

		private void AppendNotEquals(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);

			if (this.RightExpression == null)
			{
				appender.Append(" IS NOT NULL");
			}
			else
			{
				appender.Append(" <> ");
				appender.AppendExpression(this.RightExpression);
			}
		}

		private void AppendLower(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" < ");
			appender.AppendExpression(this.RightExpression);
		}

		private void AppendLowerEquals(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" <= ");
			appender.AppendExpression(this.RightExpression);
		}

		private void AppendGreater(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" > ");
			appender.AppendExpression(this.RightExpression);
		}

		private void AppendGreaterEquals(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" >= ");
			appender.AppendExpression(this.RightExpression);
		}

		private void AppendBeginsWith(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" LIKE ");
			appender.AppendExpression(this.RightExpression);
			appender.Append(" + '%'");
		}

		private void AppendContains(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" LIKE '%' + ");
			appender.AppendExpression(this.RightExpression);
			appender.Append(" + '%'");
		}

		private void AppendEndsWith(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" LIKE '%' + ");
			appender.AppendExpression(this.RightExpression);
		}

		private void AppendIn(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" IN (");
			AppendExpressionMulti(appender, this.RightExpression);
			appender.Append(")");
		}

		private void AppendNotIn(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" NOT IN (");
			AppendExpressionMulti(appender, this.RightExpression);
			appender.Append(")");
		}

		private void AppendBetween(SqlQueryAppender appender)
		{
			appender.AppendExpression(this.LeftExpression);
			appender.Append(" BETWEEN ");

			IEnumerable enumeration = this.RightExpression as IEnumerable;
			if (enumeration == null)
			{
				throw new InvalidOperationException("Expression must be an instance of IEnumerable and must contain 2 items.");
			}
			var enumerator = enumeration.GetEnumerator();

			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("Expression must be an instance of IEnumerable and must contain 2 items.");
			}
			appender.AppendExpression(enumerator.Current);

			appender.Append(" AND ");

			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("Expression must be an instance of IEnumerable and must contain 2 items.");
			}
			appender.AppendExpression(enumerator.Current);
		}

		private static void AppendExpressionMulti(SqlQueryAppender appender, object expression)
		{
			ISqlQuery sql = expression as ISqlQuery;
			if (sql != null)
			{
				appender.Append(sql.GetQuery());
			}
			else
			{
				IEnumerable enumeration = expression as IEnumerable;
				if (enumeration == null)
				{
					throw new InvalidOperationException("Expression must be an instance of ISqlQuery or IEnumerable.");
				}
				appender.AppendParams(enumeration);
			}
		}
		#endregion
	}
}
