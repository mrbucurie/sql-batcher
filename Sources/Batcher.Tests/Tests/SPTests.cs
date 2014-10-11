using System.Collections.Generic;
using System.Linq;
using Batcher.Tests.Data;
using Batcher.Tests.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Batcher.Tests.Tests
{
	[TestClass]
	public class SPTests
	{
		[TestMethod]
		public void TestUserDefinedTableType()
		{
			var legacyData = SqlStore.For<LegacyData>();

			var query = Sql.Select(legacyData).Take(100);

			using (var dbContext = new BatcherDbContext())
			{
				IList<LegacyData> tableValues = dbContext.GetResult<LegacyData>(query);

				using (var dbSets = dbContext.SP("dbo.spSelectLegacyData", new { @LegacyDataTable = tableValues.AsMappedTo("dbo.udLegacyDataTable") }))
				{
					List<LegacyData> udTableValues = dbSets.GetSet<LegacyData>().ToList();

					for (int i = 0; i < tableValues.Count; i++)
					{
						Assert.AreEqual(tableValues[i].ID, udTableValues[i].ID);
						Assert.AreEqual(tableValues[i].Title, udTableValues[i].Title);
						Assert.AreEqual(tableValues[i].UniqueID, udTableValues[i].UniqueID);
						Assert.AreEqual(tableValues[i].Amount, udTableValues[i].Amount);
						Assert.AreEqual(tableValues[i].CreatedDate, udTableValues[i].CreatedDate);
						Assert.AreEqual(tableValues[i].UpdatedDate, udTableValues[i].UpdatedDate);
					}
				}
			}
		}
	}
}