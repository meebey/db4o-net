namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public interface IQueryResult : System.Collections.IEnumerable
	{
		object Get(int index);

		Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs();

		int Size();

		object StreamLock();

		Db4objects.Db4o.IObjectContainer ObjectContainer();

		int IndexOf(int id);

		void Sort(Db4objects.Db4o.Query.IQueryComparator cmp);
	}
}