using System;
using System.Collections.Generic;
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
				values = dbContext.GetResult<BigData>(query);
			}

			Assert.AreEqual(values.Count(), count);
		}

		[TestMethod]
		public void TestInsertWithScopeIdentity()
		{
			var value = GetBigDataItems(1).First();

			var bigData = SqlStore.For<BigData>();

			var query = new QueryBatch
			{
				Sql.Insert(bigData).Values(value).Output(),
				Sql.SelectScopeIdentity(),
			};
			using (var dbContext = new BatcherDbContext())
			{
				using (var sbSets = dbContext.Execute(query))
				{
					value = sbSets.GetSet<BigData>().First();
					int insertedID = sbSets.GetSetBase<int>().First();

					Assert.AreEqual(value.ID, insertedID);
				}
			}
		}

		[TestMethod]
		public void TestInsertsWithScopeIdentities()
		{
			const int count = 100;
			var values = GetBigDataItems(count);

			var bigData = SqlStore.For<BigData>();

			var query = new QueryBatch();
			foreach (var value in values)
			{
				query.Add(Sql.Insert(bigData).Values(value));
				query.Add(Sql.Select(bigData).Where(bigData[t => t.ID] == Sql.ScopeIdentity()));
			}

			using (var dbContext = new BatcherDbContext())
			{
				using (var dbSets = dbContext.Execute(query))
				{
					values = dbSets.GetUnifiedSets<BigData>().ToList();
				}
			}

			Assert.AreEqual(values.Count(), count);
		}

		[TestMethod]
		public void CheckPerformace()
		{
			long batcherMillis, batcher2Millis, dapperMillis;

			const int count = 100;

			using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["BatcherDb"].ConnectionString))
			{
				connection.Open();

				var values = GetBigDataItems(count);

				Stopwatch sw = Stopwatch.StartNew();

				var result = connection.Execute(@"insert into dbo.BigData 
													(Title, UniqueId, CreatedDate, UpdatedDate, IsBatcher, Amount, DataContent) values (@Title, @UniqueId, @CreatedDate, @UpdatedDate, @IsBatcher, @Amount, @DataContent)",
													values);
				sw.Stop();

				dapperMillis = sw.ElapsedMilliseconds;
			}

			using (var dbContext = new BatcherDbContext())
			{
				var values = GetBigDataItems(count);

				var bigData = SqlStore.For<BigData>();
				var query = Sql.Insert(bigData).ValuesBatch(values);

				Stopwatch sw = Stopwatch.StartNew();

				var result = dbContext.ExecuteNonQuery(query);

				sw.Stop();

				batcherMillis = sw.ElapsedMilliseconds;
			}

			using (var dbContext = new BatcherDbContext())
			{
				var values = GetLegacyDataItems(count);

				var legacyData = SqlStore.For<LegacyData>();
				var query = Sql.Insert(legacyData).ValuesBatch(values);

				Stopwatch sw = Stopwatch.StartNew();

				var result = dbContext.ExecuteNonQuery(query);

				sw.Stop();

				batcher2Millis = sw.ElapsedMilliseconds;
			}

			Console.WriteLine("Perf (ms) for inserting {0} items - Batcher vs Dapper: {1}/{2} - {3}", count, batcherMillis, batcher2Millis, dapperMillis);
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

		public static IEnumerable<LegacyData> GetLegacyDataItems(int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new LegacyData
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