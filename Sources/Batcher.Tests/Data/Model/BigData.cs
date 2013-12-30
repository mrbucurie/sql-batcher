using System;
using Batcher.Annotations;

namespace Batcher.Tests.Data.Model
{
	[Identity("ID")]
	public class BigData
	{
		public int ID { get; set; }

		public string Title { get; set; }
		
		public Guid UniqueID { get; set; }
		
		public bool? IsBatcher { get; set; }
		
		public byte[] DataContent { get; set; }
		
		public decimal? Amount { get; set; }
		
		public DateTime CreatedDate { get; set; }
		
		public DateTime UpdatedDate { get; set; }
	}
}