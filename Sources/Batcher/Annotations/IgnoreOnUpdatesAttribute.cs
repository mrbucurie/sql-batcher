using System;

namespace Batcher.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class IgnoreOnUpdatesAttribute : Attribute
	{
		public bool IgnoreOnRead { get; set; }
	}
}