using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
			Stopwatch sw = Stopwatch.StartNew();

			var bigData = SqlStore.For<BigData>();
			var bigData2 = SqlStore.For<BigData>().As("bigData2");

			var query = Sql.Select(bigData).Columns(bigData2.AllColumns)
							.InnerJoin(bigData2, on: bigData[t => t.ID] == bigData2[t => t.ID])
							.IncludeTotal();

			using (var dbContext = new BatcherDbContext())
			{
				var values = dbContext.GetPage<BigData>(query);

				sw.Stop();
				Console.WriteLine("TestSelectMethod: {0}", sw.ElapsedMilliseconds);

				Assert.AreEqual(values.PageData.Count, values.TotalCount);
			}
		}

		[TestMethod]
		public void TestSelectAsync()
		{
			var data = SqlStore.For<BigData>();

			var query = Sql.Select(data)
							.Where(new GroupFilter(GroupFilterType.And) 
							{
 								data[t => t.CreatedDate] <= new DateTime(2014, 6, 5),
 								data[t => t.CreatedDate] >= new DateTime(2014, 2, 18),
							});

			using (var dbContext = new BatcherDbContext())
			{
				var valuesResult = dbContext.GetResultAsync<LegacyData>(query);
				valuesResult.Wait();
				
				IList<LegacyData> values = valuesResult.Result;

				IList<LegacyData> values2 = dbContext.GetResult<LegacyData>(query);

				Assert.AreEqual(string.Join(",",values.Select(v => v.UniqueID)),string.Join(",",values2.Select(v => v.UniqueID)));
			}
		}

		[TestMethod]
		public void TestSelectScalar()
		{
			var data = SqlStore.For<BigData>();

			var query = Sql.Select(data).Columns(data[t => t.UniqueID]).Take(1);

			using (var dbContext = new BatcherDbContext())
			{
				var result = dbContext.ExecuteScalar(query) as Guid?;

				Assert.IsTrue(result.HasValue);
			}
		}

		[TestMethod]
		public void TestSelectMethodColumnMapping()
		{
			Stopwatch sw = Stopwatch.StartNew();

			var data = SqlStore.For<LegacyData>();
			var data2 = SqlStore.For<LegacyData>().As("legacy_data2");

			var query = Sql.Select(data).Columns(data2.AllColumns)
							.InnerJoin(data2, on: data[t => t.ID] == data2[t => t.ID])
							.IncludeTotal();

			using (var dbContext = new BatcherDbContext())
			{
				var values = dbContext.GetPage<LegacyData>(query);

				sw.Stop();
				Console.WriteLine("TestSelectMethodColumnMapping: {0}", sw.ElapsedMilliseconds);

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

		[TestMethod]
		public void TestGroupByColumnMapping()
		{
			var data = SqlStore.For<LegacyData>();

			var query = Sql.Select(data)
							.Columns(data[t => t.IsBatcher], Sql.Count().As("DataCount"))
							.GroupBy(data[t => t.IsBatcher])
							.IncludeTotal();

			using (var dbContext = new BatcherDbContext())
			{
				using (var dataSets = dbContext.Execute(query))
				{
					var values = dataSets.GetSetAnonymous(new { IsBatcher = default(bool), DataCount = 0 }).ToList();

					Assert.IsTrue(values.All(v => v.DataCount > 0));

					var total = dataSets.GetSetBase<int>().First();

					Assert.AreEqual(total, values.Count);
				}
			}
		}

		[TestMethod]
		public void TestAnonymous()
		{
			var bigDataObj = new
			{
				CreatedDate = default(DateTime),
				IsBatcher = default(bool?),
				Amount = default(decimal),
				Title = default(string),
			};

			var bigData = SqlStore.ForAnonymous(bigDataObj, "dbo.BigData");

			var query = Sql.Select(bigData).Take(2);

			using (var dbContext = new BatcherDbContext())
			{
				var data = dbContext.AnonymousGetResult(query, bigDataObj);

				Assert.IsTrue(data.Count == 2);
			}
		}

		public void CheckPerformace(int count)
		{
			long batcherMillis, batcher2Millis, dapperMillis;

			using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["BatcherDb"].ConnectionString))
			{
				connection.Open();

				Stopwatch sw = Stopwatch.StartNew();

				var values = connection.Query<BigData>("select TOP (@rows) * from BigData", new { rows = count });

				var last = values.LastOrDefault();

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

			var legacyData = SqlStore.For<LegacyData>();
			var query2 = Sql.Select(legacyData).Take(count);
			using (var dbContext = new BatcherDbContext())
			{
				Stopwatch sw = Stopwatch.StartNew();

				var values = dbContext.GetResult<LegacyData>(query2);

				var last = values.LastOrDefault();

				sw.Stop();

				batcher2Millis = sw.ElapsedMilliseconds;
			}

			Console.WriteLine("Perf (ms) for {0} items - Batcher vs Dapper: {1}/{2} - {3}", count, batcherMillis, batcher2Millis, dapperMillis);
		}
	}
}