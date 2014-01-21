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
				values = dbContext.GetResult<BigData>(query).ToList();
			}

			Assert.AreEqual(values.Count(), count);
		}

		[TestMethod]
		public void CheckPerformace()
		{
			long batcherMillis, dapperMillis;

			const int count = 100;

			var bigData = SqlStore.For<BigData>();

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

				var query = Sql.Insert(bigData).ValuesBatch(values);

				Stopwatch sw = Stopwatch.StartNew();

				var result = dbContext.ExecuteNonQuery(query);

				sw.Stop();

				batcherMillis = sw.ElapsedMilliseconds;

			}
			Console.WriteLine("Perf (ms) for inserting {0} items - Batcher vs Dapper: {1} - {2}", count, batcherMillis, dapperMillis);
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