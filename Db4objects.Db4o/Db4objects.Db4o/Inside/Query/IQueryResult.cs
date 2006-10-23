namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public interface IQueryResult : System.Collections.IEnumerable
	{
		object Get(int index);

		Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs();

		int Size();

		Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer();

		int IndexOf(int id);

		void Sort(Db4objects.Db4o.Query.IQueryComparator cmp);

		void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz);

		void LoadFromQuery(Db4objects.Db4o.QQuery query);

		void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator iterator);

		void LoadFromIdReader(Db4objects.Db4o.YapReader reader);
	}
}
