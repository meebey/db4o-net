namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapClassCollectionIterator : Db4objects.Db4o.Foundation.MappingIterator
	{
		private readonly Db4objects.Db4o.YapClassCollection i_collection;

		internal YapClassCollectionIterator(Db4objects.Db4o.YapClassCollection a_collection
			, System.Collections.IEnumerator iterator) : base(iterator)
		{
			i_collection = a_collection;
		}

		public virtual Db4objects.Db4o.YapClass CurrentClass()
		{
			return (Db4objects.Db4o.YapClass)Current;
		}

		protected override object Map(object current)
		{
			return i_collection.ReadYapClass((Db4objects.Db4o.YapClass)current, null);
		}
	}
}
