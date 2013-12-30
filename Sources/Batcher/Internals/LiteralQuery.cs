namespace Batcher.Internals
{
	internal class LiteralQuery : ISqlQuery
	{
		public string Text { get; private set; }

		public LiteralQuery(string text)
		{
			this.Text = text;
		}

		public SqlQuery GetQuery()
		{
			return new SqlQuery(this.Text);
		}
	}
}