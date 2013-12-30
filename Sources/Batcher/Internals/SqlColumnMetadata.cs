using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Batcher.Annotations;

namespace Batcher.Internals
{
	public class SqlColumnMetadata
	{
		public bool IsIdentity { get; set; }
		public bool IsInsertIdentity { get; set; }

		public PropertyInfo PropertyInfo { get; set; }

		public static IEnumerable<SqlColumnMetadata> GetWriteableColumns(object columnValues)
		{
			var identities = TypeDescriptor.GetAttributes(columnValues).OfType<IdentityAttribute>().ToList();

			PropertyInfo[] properties = columnValues.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.IgnoreCase);

			foreach (var property in properties)
			{
				if (property.GetCustomAttribute<IgnoreOnUpdatesAttribute>() != null)
				{
					continue;
				}

				SqlColumnMetadata result = new SqlColumnMetadata
				{
					PropertyInfo = property
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

		public static IEnumerable<SqlColumnMetadata> GetIdendityColumns<T>()
		{
			var type = typeof(T);
			var identities = type.GetCustomAttributes<IdentityAttribute>();

			var result = from identity in identities
						 let property = type.GetProperty(identity.PropertyName)
						 select new SqlColumnMetadata
						 {
							 PropertyInfo = property,
							 IsIdentity = true,
							 IsInsertIdentity = identity.IsInsertIdentity
						 };
			return result;
		}

		public static IEnumerable<SqlColumnMetadata> GetIdendityColumns<T>(T columnValues)
		{
			var type = columnValues.GetType();
			var identities = TypeDescriptor.GetAttributes(columnValues).OfType<IdentityAttribute>();

			var result = from identity in identities
						 let property = type.GetProperty(identity.PropertyName)
						 select new SqlColumnMetadata
									{
										PropertyInfo = property,
										IsIdentity = true,
										IsInsertIdentity = identity.IsInsertIdentity
									};
			return result;
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