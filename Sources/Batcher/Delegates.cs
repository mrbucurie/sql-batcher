using System;
using System.Data.SqlClient;

namespace Batcher
{
	public delegate bool TryConvertDelegate(object value, Type toType, out object resultValue);

	public delegate void AdjustSqlParameterDelegate(SqlParameter parameter);
}