using System;

namespace Batcher.Annotations
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class UserDefinedTableTypeAttribute : Attribute
	{
		public string ObjectName { get; private set; }

		public Type ElementType { get; set; }

		public UserDefinedTableTypeAttribute(string objectName)
		{
			this.ObjectName = objectName;
		}
	}
}