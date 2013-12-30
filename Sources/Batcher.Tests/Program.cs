﻿using System;
using Batcher.Tests.Execution;
using Batcher.Tests.Tests;

namespace Batcher.Tests
{
	class Program
	{
		static readonly Random Random = new Random();

		public static void Main()
		{
			//CreateItems(); //populate database

			var result = Safe.Execute(TestPerf);
			if (result.Exception != null)
			{
				Console.WriteLine("Error:");
				Console.WriteLine(result.Exception);
			}
			Console.WriteLine("Duration:" + result.PerformanceCounter.TotalMilliseconds);

			Console.WriteLine("Done...");
			Console.ReadKey();
		}

		private static void CreateItems()
		{
			for (int i = 0; i < 1000; i++)
			{
				new InsertTests().TestInsertMethod(); //creates 100 items
			}
		}

		private static void TestPerf()
		{
			var insertTest = new SelectTests();
			for (int i = 0; i < 10; i++)
			{
				insertTest.CheckPerformace(Random.Next(500, 5000));
			}
		}
	}
}
