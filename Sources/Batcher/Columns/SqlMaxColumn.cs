namespace Batcher.Columns
{
	public class SqlMaxColumn : SqlFuncColumn
	{
		#region .ctor
		public SqlMaxColumn(ISqlColumn column)
			: base("MAX", column) { }
		#endregion
	}
}