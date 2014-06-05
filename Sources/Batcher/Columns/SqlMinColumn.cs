namespace Batcher.Columns
{
	public class SqlMinColumn : SqlFuncColumn
	{
		#region .ctor
		public SqlMinColumn(ISqlColumn column)
			: base("MIN", column) { }
		#endregion
	}
}