using System;

namespace Batcher.Annotations
{
	public class StoreAttribute : Attribute
	{
		public string StoreName { get; private set; }

		public StoreAttribute(string storeName)
		{
			this.StoreName = storeName;
		}
	}
}