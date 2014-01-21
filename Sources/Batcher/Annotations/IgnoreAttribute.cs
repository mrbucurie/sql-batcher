using System;

namespace Batcher.Annotations
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	internal class IgnoreAttribute : Attribute
	{
		public string PropertyName { get; private set; }

		public IgnoreAttribute(string propertyName)
		{
			this.PropertyName = propertyName;
		}
	}
}