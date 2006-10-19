namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	internal class QResultClient : Db4objects.Db4o.QueryResultImpl
	{
		internal QResultClient(Db4objects.Db4o.Transaction ta) : base(ta)
		{
		}

		internal QResultClient(Db4objects.Db4o.Transaction ta, int initialSize) : base(ta
			, initialSize)
		{
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.QResultClientIterator(this);
		}
	}
}
