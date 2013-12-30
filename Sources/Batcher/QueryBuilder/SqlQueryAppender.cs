using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Batcher.QueryBuilder
{
	public class SqlQueryAppender : ISqlQuery
	{
		#region Constants
		public const string SqlParameterFormat = "@p{0}";
		#endregion


		#region Private members
		protected readonly StringBuilder Builder;

		protected readonly List<SqlParameter> Parameters;
		#endregion


		#region .ctor
		private SqlQueryAppender()
		{
			this.Builder = new StringBuilder();
			this.Parameters = new List<SqlParameter>();
		}

		public static SqlQueryAppender Create()
		{
			return new SqlQueryAppender();
		}
		#endregion


		#region Methods (non-public)
		protected int AddParameter(object value)
		{
			this.Parameters.Add(new SqlParameter { Value = value });
			return this.Parameters.Count - 1;
		}

		protected void AdjustAndAppend(string sqlStringFormat, IEnumerable<SqlParameter> sqlParameters)
		{
			object[] adjustedParameters = sqlParameters.Select<SqlParameter, object>(p => string.Format(CultureInfo.InvariantCulture, "{{{0}}}", this.AddParameter(p.Value))).ToArray();
			string sqlClause = adjustedParameters.Length == 0 ? sqlStringFormat : string.Format(CultureInfo.InvariantCulture, sqlStringFormat, adjustedParameters);

			Append(sqlClause);
		}
		#endregion


		#region Public methods
		public void AppendLine()
		{
			this.Builder.AppendLine();
		}

		public void Append(string sqlString)
		{
			this.Builder.Append(sqlString);
		}

		public void AppendLine(string sqlString)
		{
			this.Builder.AppendLine(sqlString);
		}

		public void Append(SqlQuery query)
		{
			this.AdjustAndAppend(query.SqlFormat, query.SqlParams);
		}

		public virtual void AppendParam(object value)
		{
			if (value == null)
			{
				Builder.Append("NULL");
				return;
			}

			Builder.AppendFormat(CultureInfo.InvariantCulture, "{{{0}}}", AddParameter(value));
		}

		public void AppendParams(IEnumerable value)
		{
			IEnumerator enumerator = value.GetEnumerator();
			if (enumerator.MoveNext())
			{
				AppendParam(enumerator.Current);
				while (enumerator.MoveNext())
				{
					this.Builder.Append(",");
					AppendParam(enumerator.Current);
				}
			}
			else
			{
				this.Append("NULL");
			}
		}
		#endregion


		#region ISqlQuery
		public virtual SqlQuery GetQuery()
		{
			return new SqlQuery(this.Builder.ToString(), this.Parameters);
		}
		#endregion
	}
}