using System;
using System.Collections.Generic;
using System.Linq;
using Batcher.Tests.Data;
using Batcher.Tests.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Batcher.Tests.Tests
{
	[TestClass]
	public class InsertTests
	{
		[TestMethod]
		public void TestInsertMethod()
		{
			const int count = 100;
			var bigData = SqlStore.For<BigData>();

			var values = GetBigDataItems(count);

			var query = Sql.Insert(bigData).ValuesBatch(values).Output();

			using (var dbContext = new BatcherDbContext())
			{
				values = dbContext.GetResult<BigData>(query).ToList();
			}

			Assert.AreEqual(values.Count(), count);
		}

		public static IEnumerable<BigData> GetBigDataItems(int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new BigData
				{
					Title = "Item" + i,
					UniqueID = Guid.NewGuid(),
					CreatedDate = DateTime.Now,
					UpdatedDate = DateTime.Now,
					IsBatcher = true,
					Amount = i,
					DataContent = new byte[] { 1, 2, 3, 4, 5 }
				};
			}
		}
	}
}