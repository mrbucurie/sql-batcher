using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Batcher.Annotations;

namespace Batcher.Internals
{
	public class SqlColumnMetadata
	{
		private PropertyDescriptor _propertyDescriptor;

		public bool IsIdentity { get; set; }
		
		public bool IsInsertIdentity { get; set; }

		public string Name { get { return this._propertyDescriptor.Name; } }

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

				var identity = identities.Find(id => string.Equals(id.PropertyName, property.Name, StringComparison.OrdinalIgnoreCase));
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
	}
}