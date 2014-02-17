using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Batcher.Tests.Data;
using Batcher.Tests.Data.Model;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Batcher.Tests.Tests
{
	[TestClass]
	public class SelectTests
	{
		[TestMethod]
		public void TestSelectMethod()
		{
			var bigData = SqlStore.For<BigData>();
			var bigData2 = SqlStore.For<BigData>().As("bigData2");

			var query = Sql.Select(bigData).Columns(bigData.Wildcard)
							.InnerJoin(bigData2, on: bigData[t => t.ID] == bigData2[t => t.ID])
							.Where(bigData[t => t.ID].Between(5, 10))
							.IncludeTotal();

			using (var dbContext = new BatcherDbContext())
			{
				var values = dbContext.GetPage<BigData>(query);

				Assert.AreEqual(values.PageData.Count, values.TotalCount);
			}
		}

		[TestMethod]
		public void TestGroupBy()
		{
			var bigData = SqlStore.For<BigData>();

			var query = Sql.Select(bigData)
							.Columns(bigData[t => t.Title], Sql.Count().As("TitleCount"))
							.GroupBy(bigData[t => t.Title])
							.IncludeTotal();

			using (var dbContext = new BatcherDbContext())
			{
				using (var dataSets = dbContext.Execute(query))
				{
					var values = dataSets.GetSetAnonymous(new { Title = (string)null, TitleCount = 0 }).ToList();

					Assert.IsTrue(values.All(v => v.TitleCount > 0));

					var total = dataSets.GetSetBase<int>().First();

					Assert.AreEqual(total, values.Count);
				}
			}
		}

		public void CheckPerformace(int count)
		{
			long batcherMillis, dapperMillis;

			using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["BatcherDb"].ConnectionString))
			{
				connection.Open();

				Stopwatch sw = Stopwatch.StartNew();

				var values = connection.Query<BigData>("select TOP (@rows) * from BigData", new { rows = count });

				var last = values.ToList().LastOrDefault();

				sw.Stop();

				dapperMillis = sw.ElapsedMilliseconds;
			}


			var bigData = SqlStore.For<BigData>();
			var query = Sql.Select(bigData).Take(count);
			using (var dbContext = new BatcherDbContext())
			{
				Stopwatch sw = Stopwatch.StartNew();

				var values = dbContext.GetResult<BigData>(query);

				var last = values.LastOrDefault();

				sw.Stop();

				batcherMillis = sw.ElapsedMilliseconds;
			}

			Console.WriteLine("Perf (ms) for {0} items - Batcher vs Dapper: {1} - {2}", count, batcherMillis, dapperMillis);
		}
	}
}