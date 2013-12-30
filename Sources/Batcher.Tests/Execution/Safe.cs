using System;
using System.Diagnostics;

namespace Batcher.Tests.Execution
{
	public static class Safe
	{
		/// <summary>
		/// Invokes the provided action and catches any exception that might occur in the execution. The result also provides information about the duration of the execution.
		/// </summary>
		/// <param name="action">The action to be invoked.</param>
		/// <returns></returns>
		public static SafeOutput Execute(Action action)
		{
			SafeOutput output = new SafeOutput();
			Stopwatch timer = Stopwatch.StartNew();

			try
			{
				action();
			}
			catch (Exception ex)
			{
				output.Exception = ex;
			}

			timer.Stop();
			output.PerformanceCounter = timer.Elapsed;

			return output;
		}

		/// <summary>
		/// Invokes the provided action and catches any exception that might occur in the execution. The result also provides information about the duration of the execution.
		/// </summary>
		/// <param name="action">The action to be invoked.</param>
		/// <returns></returns>
		public static SafeOutput<T> Execute<T>(Func<T> action)
		{
			SafeOutput<T> output = new SafeOutput<T>();
			Stopwatch timer = Stopwatch.StartNew();

			try
			{
				output.Result = action();
			}
			catch (Exception ex)
			{
				output.Exception = ex;
			}

			timer.Stop();
			output.PerformanceCounter = timer.Elapsed;

			return output;
		}
	}
}