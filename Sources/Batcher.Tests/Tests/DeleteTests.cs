using System;
using System.Linq;
using Batcher.Tests.Data;
using Batcher.Tests.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Batcher.Tests.Tests
{
	[TestClass]
	public class DeleteTests
	{
		[TestMethod]
		public void TestDeleteMethod()
		{
			//ensure an unique id
			var uniqueId = Guid.NewGuid();

			var bigData = SqlStore.For<BigData>();

			BigData item = InsertTests.GetBigDataItems(1).First();
			item.UniqueID = uniqueId;

			using (var dbContext = new BatcherDbContext())
			{
				var query = Sql.Insert(bigData).Values(item).Output();
				item = dbContext.GetResult<BigData>(query).First();
				Assert.AreEqual(item.UniqueID, uniqueId);

				query = Sql.Delete(bigData).Where(bigData[d => d.UniqueID] == uniqueId).Output();
				var deletedItem = dbContext.GetResult<BigData>(query).First();
				Assert.AreEqual(item.ID, deletedItem.ID);

				query = Sql.Select(bigData).Columns(Sql.Count()).Where(bigData[d => d.UniqueID] == uniqueId);
				var selectResult = dbContext.GetFirstBase<int>(query);
				Assert.AreEqual(selectResult, 0);
			}
		}
	}
}