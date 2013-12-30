using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Batcher
{
	public class SqlQuery
	{
		#region Properties
		public string SqlFormat { get; private set; }

		public IEnumerable<SqlParameter> SqlParams { get; private set; }
		#endregion


		#region .ctor
		public SqlQuery(string sqlString)
			: this(sqlString, null)
		{ }

		public SqlQuery(string sqlStringFormat, IEnumerable<SqlParameter> parameters)
		{
			this.SqlFormat = sqlStringFormat;
			this.SqlParams = parameters ?? new SqlParameter[0];
		}
		#endregion


		#region Debug methods
		private static object FormatValue(SqlParameter parameter)
		{
			var value = parameter.Value;
			if (value == null || value == DBNull.Value)
			{
				return "NULL";
			}
			bool? boolValue = value as bool?;
			if (boolValue.HasValue)
			{
				value = boolValue.Value ? 1 : 0;
			}
			else
			{
				string stringValue = value as string;
				if (stringValue != null)
				{
					value = "'" + stringValue + "'";
				}
				else
				{
					DateTime? dateTimeValue = value as DateTime?;
					if (dateTimeValue.HasValue)
					{
						value = "'" + dateTimeValue.Value.ToString("yyyy-MM-dd hh:mm:ss.fff", CultureInfo.InvariantCulture) + "'";
					}
					else
					{
						byte[] bytesValue = value as byte[];
						if (bytesValue != null)
						{
							value = "0x" + BitConverter.ToString(bytesValue).Replace("-", "");
						}
					}
				}
			}
			return value.ToString();
		}

		public string Debug()
		{
			return string.Format(CultureInfo.InvariantCulture, this.SqlFormat, this.SqlParams.Select(FormatValue).ToArray());
		}
		#endregion
	}
}