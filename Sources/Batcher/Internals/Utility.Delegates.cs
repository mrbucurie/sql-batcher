using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Batcher.Internals
{
	internal static partial class Utility
	{
		public static class Delegates
		{
			public static bool TryConvertValue(object value, Type toType, out object resultValue)
			{
				resultValue = null;

				if (value == null || value == DBNull.Value)
				{
					return Nullable.GetUnderlyingType(toType) != null;
				}

				if (toType.IsInstanceOfType(value))
				{
					resultValue = value;
					return true;
				}

				Type propertyType = Nullable.GetUnderlyingType(toType) ?? toType;

				try
				{
					resultValue = Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
					return true;
				}
				catch (InvalidCastException) { }
				catch (FormatException) { }
				catch (OverflowException) { }
				catch (ArgumentNullException) { }

				return false;
			}

			public static void AdjustSqlParameter(SqlParameter parameter) { }
		}
	}
}
