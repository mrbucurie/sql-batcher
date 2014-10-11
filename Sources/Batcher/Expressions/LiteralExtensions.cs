using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Batcher.Internals;

namespace Batcher.Expressions
{
	public static class LiteralExtensions
	{
		private static ISqlQuery ToLiteralInternal<T>(this IEnumerable<T> source, IFormatProvider formatProvider)
			where T : IConvertible
		{
			return new LiteralQuery(string.Join(",", source.Select(s => s.ToString(formatProvider))));
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma, using <see cref="CultureInfo.InvariantCulture"/> as IFormatProvider to convert numbers to string.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<short> source)
		{
			return source.ToLiteralInternal(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <param name="formatProvider">The format provider used to conver the numbers to string.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<short> source, IFormatProvider formatProvider)
		{
			return source.ToLiteralInternal(formatProvider);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma, using <see cref="CultureInfo.InvariantCulture"/> as IFormatProvider to convert numbers to string.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<int> source)
		{
			return source.ToLiteralInternal(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <param name="formatProvider">The format provider used to conver the numbers to string.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<int> source, IFormatProvider formatProvider)
		{
			return source.ToLiteralInternal(formatProvider);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma, using <see cref="CultureInfo.InvariantCulture"/> as IFormatProvider to convert numbers to string.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<long> source)
		{
			return source.ToLiteralInternal(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <param name="formatProvider">The format provider used to conver the numbers to string.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<long> source, IFormatProvider formatProvider)
		{
			return source.ToLiteralInternal(formatProvider);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma, using <see cref="CultureInfo.InvariantCulture"/> as IFormatProvider to convert numbers to string.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<decimal> source)
		{
			return source.ToLiteralInternal(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <param name="formatProvider">The format provider used to conver the numbers to string.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<decimal> source, IFormatProvider formatProvider)
		{
			return source.ToLiteralInternal(formatProvider);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma, using <see cref="CultureInfo.InvariantCulture"/> as IFormatProvider to convert numbers to string.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<double> source)
		{
			return source.ToLiteralInternal(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <param name="formatProvider">The format provider used to conver the numbers to string.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<double> source, IFormatProvider formatProvider)
		{
			return source.ToLiteralInternal(formatProvider);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma, using <see cref="CultureInfo.InvariantCulture"/> as IFormatProvider to convert numbers to string.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<float> source)
		{
			return source.ToLiteralInternal(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Creates a literal query by joining the numbers with comma.<para/>
		/// This is usefull when dealing with a more parameters than the maximum number of parameters in an sql query (currently 2100).
		/// </summary>
		/// <param name="source">The numbers to be converted to a literal query.</param>
		/// <param name="formatProvider">The format provider used to conver the numbers to string.</param>
		/// <returns></returns>
		public static ISqlQuery ToLiteral(this IEnumerable<float> source, IFormatProvider formatProvider)
		{
			return source.ToLiteralInternal(formatProvider);
		}
	}
}