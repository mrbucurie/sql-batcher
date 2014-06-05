namespace Batcher.Columns
{
	public class SqlCountColumn : SqlFuncColumn
	{
		#region .ctor
		public SqlCountColumn(ISqlColumn column)
			: base("COUNT", column) { }
		#endregion
	}
}