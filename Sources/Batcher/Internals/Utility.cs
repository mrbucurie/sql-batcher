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

		public static string GetPropertyName<T>(Expression<Func<T, object>> propertySelector)
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

			return memberExpression.Member.Name;
		}
		#endregion


		#region Reader
		public static IEnumerable<T> GetObjects<T>(this IDataReader dataReader, Func<T> instanceInitializer)
			where T : class
		{
			Type dataType = typeof(T);

			List<PropertyInfo> properties = GetProperties(dataReader, dataType);

			while (dataReader.Read())
			{
				T t = instanceInitializer();

				foreach (PropertyInfo property in properties)
				{
					object convertedValue;
					if (SqlDataConvertion.ConvertionDelegate(dataReader[property.Name], property.PropertyType, out convertedValue))
					{
						property.SetValue(t, convertedValue);
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

			List<PropertyInfo> properties = GetProperties(dataReader, dataType);

			var ctor = dataType.GetConstructors().First();

			while (dataReader.Read())
			{
				object[] ctorParams = new object[properties.Count];

				for (int i = 0; i < properties.Count; i++)
				{
					PropertyInfo property = properties[i];

					object convertedValue;

					if (SqlDataConvertion.ConvertionDelegate(dataReader[property.Name], property.PropertyType, out convertedValue))
					{
						ctorParams[i] = convertedValue;
					}
					else
					{
						ctorParams[i] = property.GetValue(defaultValue);
					}
				}

				yield return (T)ctor.Invoke(ctorParams);
			}
		}

		public static List<PropertyInfo> GetProperties(IDataReader dataReader, Type targetType)
		{
			List<PropertyInfo> properties = new List<PropertyInfo>();

			for (int i = 0; i < dataReader.FieldCount; i++)
			{
				string columnName = dataReader.GetName(i);

				PropertyInfo property = targetType.GetProperty(columnName);

				if (property != null)
				{
					properties.Add(property);
				}
			}
			return properties;
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

		public static IEnumerable<SqlParameter> GetSqlParameters<T>(object parameters)
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

							IEnumerable<string> orderdColumns = SqlUserDefinedTableTypes<T>.GetUserDefinedTableOrderedColumns(attr.ObjectName);

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
		public static SqlCommand GetTextCommand(SqlConnection connection, IExecutableSqlQuery query)
		{
			SqlQuery sqlQuery = query.GetQuery();

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
				IEnumerable<SqlParameter> sqlParameters = GetSqlParameters<T>(parameters);
				command.Parameters.AddParameters(sqlParameters.Select(p => { SqlDataConvertion.AdjustSqlParameterDelegate(p); return p; }));
			}

			return command;
		}
		#endregion
	}
}
