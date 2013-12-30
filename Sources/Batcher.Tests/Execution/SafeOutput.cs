using System;

namespace Batcher.Tests.Execution
{
	public class SafeOutput
	{
		#region Properties
		/// <summary>
		/// The exception that occured during the execution. If not excetion was thrown, this will be null.
		/// </summary>
		public Exception Exception { get; internal set; }

		/// <summary>
		/// The duration of the execution.
		/// </summary>
		public TimeSpan PerformanceCounter { get; internal set; }
		#endregion
	}


	public class SafeOutput<T> : SafeOutput
	{
		#region Properties
		/// <summary>
		/// The result of the execution. When an exception has occured, this property will have the default value for its type.
		/// </summary>
		public T Result { get; internal set; }
		#endregion
	}
}
