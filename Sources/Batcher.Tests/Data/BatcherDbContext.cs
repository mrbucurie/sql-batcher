using System.Configuration;

namespace Batcher.Tests.Data
{
	public class BatcherDbContext : DbContext
	{
		public BatcherDbContext()
			: base(ConfigurationManager.ConnectionStrings["BatcherDb"].ConnectionString)
		{ }
	}
}