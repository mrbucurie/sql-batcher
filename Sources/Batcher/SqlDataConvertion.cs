using System;

namespace Batcher
{
	public static class SqlDataConvertion
	{
		#region Properties
		public static TryConvertDelegate ConvertionDelegate { get; private set; }

		public static AdjustSqlParameterDelegate AdjustSqlParameterDelegate { get; private set; }
		#endregion


		#region .ctor
		static SqlDataConvertion()
		{
			ConvertionDelegate = Internals.Utility.Delegates.TryConvertValue;
			AdjustSqlParameterDelegate = Internals.Utility.Delegates.AdjustSqlParameter;
		}
		#endregion


		#region Public methods
		public static void SetReadConversionDelegate(TryConvertDelegate convertionDelegate)
		{
			if (convertionDelegate == null)
			{
				throw new ArgumentNullException("convertionDelegate");
			}
			ConvertionDelegate = convertionDelegate;
		}

		public static void SetSqlParameterAdjustDelegate(AdjustSqlParameterDelegate adjustSqlParameterDelegate)
		{
			if (adjustSqlParameterDelegate == null)
			{
				throw new ArgumentNullException("adjustSqlParameterDelegate");
			}
			AdjustSqlParameterDelegate = adjustSqlParameterDelegate;
		}
		#endregion
	}
}