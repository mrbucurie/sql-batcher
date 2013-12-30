using System.Linq;
using Batcher.Tests.Data;
using Batcher.Tests.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Batcher.Tests.Tests
{
	[TestClass]
	public class UpdateTests
	{
		[TestMethod]
		public void TestUpdateMethod()
		{
			const string insertTitle = "BigDataInsertTitle";
			const string updateTitle = "BigDataUpdateTitle";

			var bigData = SqlStore.For<BigData>();

			BigData item = InsertTests.GetBigDataItems(1).First();

			using (var dbContext = new BatcherDbContext())
			{
				//insert the new item
				item.Title = insertTitle;
				var query = Sql.Insert(bigData).Values(item).Output();
				item = dbContext.GetResult<BigData>(query).First();

				Assert.AreEqual(item.Title, insertTitle);

				//update the item
				item.Title = updateTitle;
				query = Sql.Update(bigData).Set(item).Output();
				item = dbContext.GetResult<BigData>(query).First();

				Assert.AreEqual(item.Title, updateTitle);

				//update the item again
				item.Title = updateTitle + "1";
				
				//NOTE - specify which property is the ID column
				query = Sql.Update(bigData).Set(new { item.Title, item.ID }.SetIdentity(d => d.ID)).Output();
				
				item = dbContext.GetResult<BigData>(query).First();
				
				Assert.AreEqual(item.Title, updateTitle + "1");

				//update the item again
				item.Title = updateTitle + "2";
				
				//NOTE - specify the where critaria explicitly
				query = Sql.Update(bigData).Set(new { item.Title }).Where(bigData[d => d.ID] == item.ID).Output();
				
				item = dbContext.GetResult<BigData>(query).First();

				Assert.AreEqual(item.Title, updateTitle + "2");
			}
		}
	}
}