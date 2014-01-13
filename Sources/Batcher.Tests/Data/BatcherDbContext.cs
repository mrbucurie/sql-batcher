using System.Configuration;

namespace Batcher.Tests.Data
{
	public class BatcherDbContext : DbContext
	{
		protected override Compatibility CompatibilityMode
		{
			get { return Compatibility.SQL2012; }
		}

		public BatcherDbContext()
			: base(ConfigurationManager.ConnectionStrings["BatcherDb"].ConnectionString)
		{ }
	}
}