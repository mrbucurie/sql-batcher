using System;

namespace Batcher.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SqlColumnAttribute : Attribute
	{
		public string Name { get; set; }

		public SqlColumnAttribute() { }

		public SqlColumnAttribute(string columnName)
		{
			this.Name = columnName;
		}
	}
}