using System;

namespace Batcher.Annotations
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class IdentityAttribute : Attribute
	{
		public string PropertyName { get; private set; }
		
		public bool IsInsertIdentity { get; private set; }

		public IdentityAttribute(string propertyName, bool isInsertIdentity = true)
		{
			this.PropertyName = propertyName;
			IsInsertIdentity = isInsertIdentity;
		}
	}
}