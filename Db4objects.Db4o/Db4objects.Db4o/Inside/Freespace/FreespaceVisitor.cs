namespace Db4objects.Db4o.Inside.Freespace
{
	public class FreespaceVisitor
	{
		internal int _key;

		internal int _value;

		private bool _visited;

		public virtual void Visit(int key, int value)
		{
			_key = key;
			_value = value;
			_visited = true;
		}

		public virtual bool Visited()
		{
			return _visited;
		}
	}
}
