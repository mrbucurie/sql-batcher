using System.Globalization;

namespace Batcher.Internals
{
	internal enum WithHintType
	{
		NoLock,
		RowLock
	}


	internal static class WithHintTypeExtensions
	{
		public static string GetSql(this WithHintType value)
		{
			return string.Format(CultureInfo.InvariantCulture, "WITH({0})", value.ToString().ToUpperInvariant());
		}
	}
}
