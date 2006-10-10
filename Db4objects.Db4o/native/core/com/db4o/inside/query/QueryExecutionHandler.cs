namespace Db4objects.Db4o.Inside.Query
{
	public enum QueryExecutionKind
	{
		Unoptimized,
		DynamicallyOptimized,
		PreOptimized
	}

	public class QueryExecutionEventArgs : System.EventArgs
	{
		private object _predicate;
		private QueryExecutionKind _kind;

		public QueryExecutionEventArgs(object predicate, QueryExecutionKind kind)
		{
			_predicate = predicate;
			_kind = kind;
		}

		public object Predicate
		{
			get { return _predicate; }
		}

		public QueryExecutionKind ExecutionKind
		{
			get { return _kind; }
		}
	}

	public delegate void QueryExecutionHandler(object sender, QueryExecutionEventArgs args);
}