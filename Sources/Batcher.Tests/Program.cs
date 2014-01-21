using System;
using Batcher.Tests.Execution;
using Batcher.Tests.Tests;

namespace Batcher.Tests
{
	class Program
	{
		static readonly Random Random = new Random();

		public static void Main()
		{
			TestPerfOnInsert();
			TestPerfOnSelect();

			Console.WriteLine("Done...");
			Console.ReadKey();
		}

		private static void TestPerfOnSelect()
		{
			var insertTest = new SelectTests();
			for (int i = 0; i < 10; i++)
			{
				insertTest.CheckPerformace(Random.Next(500, 5000));
			}
		}

		private static void TestPerfOnInsert()
		{
			var insertTest = new InsertTests();
			for (int i = 0; i < 10; i++)
			{
				insertTest.CheckPerformace();
			}
		}
	}
}
