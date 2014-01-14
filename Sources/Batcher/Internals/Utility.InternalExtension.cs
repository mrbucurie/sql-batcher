namespace Batcher.Internals
{
	internal static partial class Utility
	{
		public static bool IsNull(this ISqlColumn column)
		{
			return ReferenceEquals(column, null);
		}
	}
}
