using System;

namespace Batcher.Annotations
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class StoreAttribute : Attribute
	{
		public string StoreName { get; private set; }

		public StoreAttribute(string storeName)
		{
			this.StoreName = storeName;
		}
	}
}