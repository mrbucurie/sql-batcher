using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Batcher.Internals;
using Batcher.Stores;

namespace Batcher.Columns
{
	public class SqlColumn : ISqlColumn
	{
		#region Properties
		private readonly string _expression;
		#endregion


		#region .ctor
		public SqlColumn(string expression)
		{
			this._expression = expression;
		}

		internal static SqlColumn From<T>(Expression<Func<T, object>> propertySelector)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "[{0}]", Utility.GetPropertyName(propertySelector)));
		}

		internal static SqlColumn From<T>(SqlStore<T> store, Expression<Func<T, object>> propertySelector)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.[{1}]", store.StoreName, Utility.GetPropertyName(propertySelector)));
		}

		internal static SqlColumn From<T>(SqlStoreAlias<T> storeAlias, Expression<Func<T, object>> propertySelector)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.[{1}]", storeAlias.AsName, Utility.GetPropertyName(propertySelector)));
		}

		internal static SqlColumn From<T>(ProcessedStore<T> store, Expression<Func<T, object>> propertySelector)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.[{1}]", store.StoreName, Utility.GetPropertyName(propertySelector)));
		}

		internal static SqlColumn From<T>(SqlStore<T> store, string expression)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.[{1}]", store.StoreName, expression));
		}

		internal static SqlColumn From<T>(SqlStoreAlias<T> storeAlias, string expression)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.[{1}]", storeAlias.AsName, expression));
		}

		internal static SqlColumn From<T>(ProcessedStore<T> store, string expression)
		{
			return new SqlColumn(string.Format(CultureInfo.InvariantCulture, "{0}.[{1}]", store.StoreName, expression));
		}
		#endregion


		#region Public methods
		public SqlColumnAlias As(string asName)
		{
			return new SqlColumnAlias(this, asName);
		}
		#endregion


		#region ISqlQueryBuilder
		public virtual SqlQuery GetQuery()
		{
			return new SqlQuery(this._expression);
		}
		#endregion


		#region Equality comparer
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return (this._expression != null ? this._expression.GetHashCode() : 0);
		}
		#endregion


		#region Filtering
		public static BinaryFilter operator ==(SqlColumn column, object value)
		{
			return new BinaryFilter(column, BinaryFilterType.Equal, value);
		}

		public static BinaryFilter operator !=(SqlColumn column, object value)
		{
			return new BinaryFilter(column, BinaryFilterType.NotEqual, value);
		}

		public static BinaryFilter operator <(SqlColumn column, object value)
		{
			return new BinaryFilter(column, BinaryFilterType.Lower, value);
		}

		public static BinaryFilter operator <=(SqlColumn column, object value)
		{
			return new BinaryFilter(column, BinaryFilterType.LowerEqual, value);
		}

		public static BinaryFilter operator >(SqlColumn column, object value)
		{
			return new BinaryFilter(column, BinaryFilterType.Greater, value);
		}

		public static BinaryFilter operator >=(SqlColumn column, object value)
		{
			return new BinaryFilter(column, BinaryFilterType.GreaterEqual, value);
		}

		public BinaryFilter BeginsWith(string value)
		{
			return new BinaryFilter(this, BinaryFilterType.BeginsWith, value);
		}

		public BinaryFilter BeginsWith(ISqlQuery query)
		{
			return new BinaryFilter(this, BinaryFilterType.BeginsWith, query);
		}

		public BinaryFilter Contains(string value)
		{
			return new BinaryFilter(this, BinaryFilterType.Contains, value);
		}

		public BinaryFilter Contains(ISqlQuery query)
		{
			return new BinaryFilter(this, BinaryFilterType.Contains, query);
		}

		public BinaryFilter EndsWith(string value)
		{
			return new BinaryFilter(this, BinaryFilterType.EndsWith, value);
		}

		public BinaryFilter EndsWith(ISqlQuery query)
		{
			return new BinaryFilter(this, BinaryFilterType.EndsWith, query);
		}

		public BinaryFilter In<T>(IEnumerable<T> collection)
		{
			return new BinaryFilter(this, BinaryFilterType.In, collection);
		}

		public BinaryFilter In(ISqlQuery query)
		{
			return new BinaryFilter(this, BinaryFilterType.In, query);
		}

		public BinaryFilter NotIn<T>(IEnumerable<T> collection)
		{
			return new BinaryFilter(this, BinaryFilterType.NotIn, collection);
		}

		public BinaryFilter NotIn(ISqlQuery query)
		{
			return new BinaryFilter(this, BinaryFilterType.NotIn, query);
		}

		public BinaryFilter Between(object minValue, object maxValue)
		{
			return new BinaryFilter(this, BinaryFilterType.Between, new[] { minValue, maxValue });
		}
		#endregion
	}
}
