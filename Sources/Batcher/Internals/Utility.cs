using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Batcher.Annotations;

namespace Batcher.Internals
{
	internal static partial class Utility
	{
		#region Mapping
		public static string GetDefaultSoreName<T>()
		{
			Type tType = typeof(T);

			StoreAttribute storeAttribute = tType.GetCustomAttributes(typeof(StoreAttribute), true).FirstOrDefault() as StoreAttribute;

			string storeName = storeAttribute != null ? storeAttribute.StoreName : String.Format(CultureInfo.InvariantCulture, "[{0}]", tType.Name);

			return storeName;
		}

		public static MemberInfo GetProperty<T>(Expression<Func<T, object>> propertySelector)
		{
			MemberExpression memberExpression = propertySelector.Body as MemberExpression;

			if (memberExpression == null)
			{
				UnaryExpression unaryExpression = (UnaryExpression)propertySelector.Body;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot extract property from expressions of other types then MemberExpression or UnaryExpression."));
			}

			return memberExpression.Member;
		}

		public static string GetPropertyName<T>(Expression<Func<T, object>> propertySelector)
		{
			return GetProperty(propertySelector).Name;
		}

		public static string GetColumnName<T>(Expression<Func<T, object>> propertySelector)
		{
			return GetColumnName(GetProperty(propertySelector));
		}

		public static string GetColumnName(MemberInfo member)
		{
			var columnAttr = member.GetCustomAttribute<SqlColumnAttribute>();

			return columnAttr != null ? columnAttr.Name : member.Name;
		}

		public static string GetColumnName(PropertyDescriptor property)
		{
			var columnAttr = property.Attributes.OfType<SqlColumnAttribute>().FirstOrDefault();

			return columnAttr != null ? columnAttr.Name : property.Name;
		}
		#endregion


		#region Reader
		public static IEnumerable<T> GetObjects<T>(this IDataReader dataReader, Func<T> instanceInitializer)
			where T : class
		{
			Type dataType = typeof(T);

			Dictionary<string, PropertyInfo> columnsProperties = dataType.GetProperties().ToDictionary(GetColumnName, p => p, StringComparer.OrdinalIgnoreCase);

			while (dataReader.Read())
			{
				T t = instanceInitializer();

				for (int i = 0; i < dataReader.FieldCount; i++)
				{
					string columnName = dataReader.GetName(i);
					object columnValue = dataReader[i];

					PropertyInfo property;
					if (columnsProperties.TryGetValue(columnName, out property))
					{
						object convertedValue;
						if (SqlDataConvertion.ConvertionDelegate(columnValue, property.PropertyType, out convertedValue))
						{
							property.SetValue(t, convertedValue);
						}
					}
				}

				yield return t;
			}
		}

		public static IEnumerable<T> GetBaseObjects<T>(this IDataReader dataReader)
		{
			Type dataType = typeof(T);

			while (dataReader.Read())
			{
				object convertedValue;

				SqlDataConvertion.ConvertionDelegate(dataReader[0], dataType, out convertedValue);

				yield return (T)convertedValue;
			}
		}

		public static IEnumerable<T> GetAnonymous<T>(this IDataReader dataReader, T defaultValue)
		{
			Type dataType = typeof(T);

			var properties = GetPropertiesAnonymous(dataReader, dataType);

			var ctor = dataType.GetConstructors().First();

			while (dataReader.Read())
			{
				object[] ctorParams = new object[properties.Count];
				var index = 0;
				foreach (var property in properties)
				{
					object convertedValue;

					if (property.Value && SqlDataConvertion.ConvertionDelegate(dataReader[property.Key.Name], property.Key.PropertyType, out convertedValue))
					{
						ctorParams[index] = convertedValue;
					}
					else
					{
						ctorParams[index] = property.Key.GetValue(defaultValue);
					}
					index++;
				}

				yield return (T)ctor.Invoke(ctorParams);
			}
		}

		private static IDictionary<PropertyInfo, bool> GetPropertiesAnonymous(IDataReader dataReader, Type targetType)
		{
			Dictionary<PropertyInfo, bool> usedProperties = targetType.GetProperties().ToDictionary(p => p, p => false);

			for (int i = 0; i < dataReader.FieldCount; i++)
			{
				string columnName = dataReader.GetName(i);

				var property = usedProperties.FirstOrDefault(p => string.Equals(p.Key.Name, columnName, StringComparison.OrdinalIgnoreCase));
				if (property.Key != null)
				{
					usedProperties[property.Key] = true;
				}
			}
			return usedProperties;
		}
		#endregion


		#region Sql parameters
		public static void AddParameters(this SqlParameterCollection source, IEnumerable<SqlParameter> parameters)
		{
			foreach (SqlParameter parameter in parameters)
			{
				source.Add(parameter);
			}
		}

		public static IEnumerable<SqlParameter> GetSqlParameters<T>(this T dbContext, object parameters)
			where T : DbContext
		{
			if (parameters != null)
			{
				var properties = TypeDescriptor.GetProperties(parameters);
				foreach (PropertyDescriptor propertyDescriptor in properties)
				{
					var parameterName = propertyDescriptor.Name;
					var parameterValue = propertyDescriptor.GetValue(parameters);

					if (parameterValue == null)
					{
						yield return new SqlParameter(parameterName, DBNull.Value);
					}
					else
					{
						var attr = TypeDescriptor.GetAttributes(parameterValue).OfType<UserDefinedTableTypeAttribute>().FirstOrDefault();
						if (attr != null)
						{
							var elementType = attr.ElementType ?? propertyDescriptor.PropertyType.GetInterface(typeof(IEnumerable<>).Name).GetGenericArguments()[0];

							IEnumerable<string> orderdColumns = SqlUserDefinedTableTypes<T>.GetUserDefinedTableOrderedColumns(dbContext, attr.ObjectName);

							yield return new SqlParameter(parameterName, SqlDbType.Structured)
							{
								Direction = ParameterDirection.Input,
								TypeName = attr.ObjectName,
								Value = GetUDTTParameter((IEnumerable)parameterValue, elementType, orderdColumns)
							};
						}
						else
						{
							yield return new SqlParameter(parameterName, parameterValue);
						}
					}
				}
			}
		}

		public static DataTable GetUDTTParameter(IEnumerable elements, Type elementType, IEnumerable<string> orderedProperties)
		{
			DataTable dataTable = new DataTable("UDTTVar");

			List<PropertyInfo> properties = new List<PropertyInfo>();

			foreach (string propertyName in orderedProperties)
			{
				PropertyInfo property = elementType.GetProperty(propertyName);

				Type columnType = null;
				if (property != null)
				{
					columnType = property.PropertyType;

					if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						columnType = columnType.GetGenericArguments()[0];
					}
				}

				dataTable.Columns.Add(new DataColumn(propertyName, columnType ?? typeof(string)));
				properties.Add(property);
			}

			foreach (object element in elements)
			{
				dataTable.Rows.Add(properties.Select(p => (p != null ? p.GetValue(element) : null) ?? DBNull.Value).ToArray());
			}

			return dataTable;
		}
		#endregion


		#region Sql command
		public static SqlCommand GetTextCommand(SqlConnection connection, IExecutableSqlQuery query, Compatibility compatibilityMode)
		{
			ICompatibileExecutableSqlQuery compatibileQuery = query as ICompatibileExecutableSqlQuery;
			SqlQuery sqlQuery = compatibileQuery != null ? compatibileQuery.GetQuery(compatibilityMode) : query.GetQuery();

			SqlCommand command = connection.CreateCommand();
			command.CommandTimeout = 0;
			command.CommandType = CommandType.Text;

			List<object> parameterNames = new List<object>();
			int paramIndex = 0;
			foreach (SqlParameter parameter in sqlQuery.SqlParams)
			{
				parameter.ParameterName = string.Format(CultureInfo.InvariantCulture, "@p{0}", paramIndex++);

				SqlDataConvertion.AdjustSqlParameterDelegate(parameter);

				command.Parameters.Add(parameter);
				parameterNames.Add(parameter.ParameterName);
			}

			command.CommandText = string.Format(CultureInfo.InvariantCulture, sqlQuery.SqlFormat, parameterNames.ToArray());

			return command;
		}

		public static SqlCommand GetSPCommand<T>(T dbContext, string spName, object parameters)
			where T : DbContext
		{
			SqlCommand command = dbContext.Connection.CreateCommand();
			command.CommandTimeout = 0;
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = spName;

			if (parameters != null)
			{
				IEnumerable<SqlParameter> sqlParameters = dbContext.GetSqlParameters(parameters);
				command.Parameters.AddParameters(sqlParameters.Select(p => { SqlDataConvertion.AdjustSqlParameterDelegate(p); return p; }));
			}

			return command;
		}
		#endregion
	}
}
