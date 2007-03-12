namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public interface IQueryResult : System.Collections.IEnumerable
	{
		object Get(int index);

		Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs();

		object Lock();

		Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer();

		int IndexOf(int id);

		int Size();

		void Sort(Db4objects.Db4o.Query.IQueryComparator cmp);
	}
}
