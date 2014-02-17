using System;
using System.Globalization;
using Batcher.QueryBuilder;

namespace Batcher.Internals
{
	internal class SqlJoinSelect : ISqlQuery
	{
		#region Private members
		private readonly SqlJoinType _joinType;

		private readonly ISqlQuery _rightSide;

		private WithHintType? _withHint;

		private ISqlFilter _onCriteria;
		#endregion


		#region .ctor
		public SqlJoinSelect(SqlJoinType joinType, ISqlQuery rightSide)
		{
			this._joinType = joinType;
			this._rightSide = rightSide;
		}
		#endregion


		#region ISqlSelectJoinOn
		public SqlJoinSelect With(WithHintType? withHint)
		{
			this._withHint = withHint;
			return this;
		}

		public SqlJoinSelect On(ISqlFilter onCriteria)
		{
			this._onCriteria = onCriteria;
			return this;
		}
		#endregion


		#region ISqlQuery
		public SqlQuery GetQuery()
		{
			SqlQueryAppender appender = SqlQueryAppender.Create();

			switch (this._joinType)
			{
				case SqlJoinType.Inner:
					appender.Append("INNER JOIN ");
					break;
				case SqlJoinType.Left:
					appender.Append("LEFT JOIN ");
					break;
				default:
					throw new ArgumentOutOfRangeException("_joinType");
			}

			appender.Append(this._rightSide.GetQuery());
			if (this._withHint.HasValue)
			{
				appender.Append(string.Format(CultureInfo.InvariantCulture, " {0}", this._withHint.Value.GetSql()));
			}
			appender.AppendLine();
			appender.Append("ON ");
			appender.Append(this._onCriteria.GetQuery());

			return appender.GetQuery();
		}
		#endregion
	}
}