using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Batcher.Annotations;

namespace Batcher.Internals
{
	internal class SqlColumnMetadata
	{
		#region Private members
		private PropertyDescriptor _propertyDescriptor;

		private string _columnName;
		#endregion


		#region Properties
		public bool IsIdentity { get; set; }

		public bool IsInsertIdentity { get; set; }

		public string Name { get { return _columnName ?? (_columnName = Utility.GetColumnName(this._propertyDescriptor)); } }
		#endregion


		#region Public methods
		public object GetValue(object source)
		{
			return this._propertyDescriptor.GetValue(source);
		}

		public static IEnumerable<SqlColumnMetadata> GetWriteableColumns(object columnValues)
		{
			var attributes = TypeDescriptor.GetAttributes(columnValues);

			var identities = attributes.OfType<IdentityAttribute>().ToList();

			var ignoreProperties = attributes.OfType<IgnoreAttribute>().Select(a => a.PropertyName).ToList();

			var properties = TypeDescriptor.GetProperties(columnValues);

			foreach (PropertyDescriptor property in properties)
			{
				if (property.Attributes.OfType<IgnoreOnUpdatesAttribute>().Count() != 0 || ignoreProperties.Contains(property.Name, StringComparer.Ordinal))
				{
					continue;
				}

				SqlColumnMetadata result = new SqlColumnMetadata
				{
					_propertyDescriptor = property
				};

				var identity = identities.Find(id => string.Equals(id.PropertyName, result.Name, StringComparison.OrdinalIgnoreCase));
				if (identity != null)
				{
					result.IsIdentity = true;
					result.IsInsertIdentity = identity.IsInsertIdentity;
				}
				yield return result;
			}
		}

		public static IList<SqlColumnMetadata> GetWriteableColumnsNoIdentity(object columnValues, out IList<SqlColumnMetadata> identities)
		{
			identities = new List<SqlColumnMetadata>();
			var columns = GetWriteableColumns(columnValues);

			var result = new List<SqlColumnMetadata>();

			foreach (var columnMetadata in columns)
			{
				if (columnMetadata.IsIdentity)
				{
					identities.Add(columnMetadata);
				}
				else
				{
					result.Add(columnMetadata);
				}
			}
			return result;
		}

		public static IEnumerable<string> GetReadableColumnNames<T>()
		{
			Type type = typeof(T);

			var properties = TypeDescriptor.GetProperties(type);

			return from PropertyDescriptor property in properties
				   where !property.Attributes.OfType<IgnoreOnUpdatesAttribute>().Any()
				   select Utility.GetColumnName(property);
		}
		#endregion
	}
}