using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Batcher.Annotations;
using Batcher.Internals;

namespace Batcher
{
	public static class ModelExtensions
	{
		#region Annotations
		public static T SetIdentity<T>(this T source, Expression<Func<T, object>> propertySelector, bool asInsertIdentity = true)
			where T : class
		{
			TypeDescriptor.AddAttributes(source, new IdentityAttribute(Utility.GetPropertyName(propertySelector), asInsertIdentity));

			return source;
		}

		public static IEnumerable<T> WithIdentity<T>(this IEnumerable<T> source, Expression<Func<T, object>> propertySelector, bool asInsertIdentity = true)
			where T : class
		{
			foreach (T value in source)
			{
				TypeDescriptor.AddAttributes(value, new IdentityAttribute(Utility.GetPropertyName(propertySelector), asInsertIdentity));
				yield return value;
			}
		}

		public static IEnumerable<T> AsMappedTo<T>(this IEnumerable<T> source, string userDefinedTableTypeName)
			where T : class
		{
			TypeDescriptor.AddAttributes(source, new UserDefinedTableTypeAttribute(userDefinedTableTypeName) { ElementType = typeof(T) });
			return source;
		}
		#endregion
	}
}