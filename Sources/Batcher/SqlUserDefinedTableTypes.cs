using Batcher.Internals;

namespace Batcher
{
	public class SqlUserDefinedTableTypes
	{
		public static void Initialize<T>()
			where T : DbContext, new()
		{
			using (T dbContext = new T())
			{
				SqlUserDefinedTableTypes<T>.Initalize(dbContext);
			}
		}
	}
}