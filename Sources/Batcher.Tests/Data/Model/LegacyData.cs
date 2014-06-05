using System;
using Batcher.Annotations;

namespace Batcher.Tests.Data.Model
{
	[Store("dbo.legacy_data"), Identity("ID")]
	public class LegacyData
	{
		[SqlColumn("id")]
		public int ID { get; set; }

		[SqlColumn("title")]
		public string Title { get; set; }

		[SqlColumn("unique_id")]
		public Guid UniqueID { get; set; }

		[SqlColumn("is_batcher")]
		public bool? IsBatcher { get; set; }

		[SqlColumn("data_content")]
		public byte[] DataContent { get; set; }

		[SqlColumn("amount")]
		public decimal? Amount { get; set; }

		[SqlColumn("created_date")]
		public DateTime CreatedDate { get; set; }

		[SqlColumn("updated_date")]
		public DateTime UpdatedDate { get; set; }
	}
}