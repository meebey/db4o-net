namespace Db4objects.Db4o.Internal
{
	/// <exclude>TODO: remove this class or make it private to ClassMetadataRepository</exclude>
	public class ClassMetadataIterator : Db4objects.Db4o.Foundation.MappingIterator
	{
		private readonly Db4objects.Db4o.Internal.ClassMetadataRepository i_collection;

		internal ClassMetadataIterator(Db4objects.Db4o.Internal.ClassMetadataRepository a_collection
			, System.Collections.IEnumerator iterator) : base(iterator)
		{
			i_collection = a_collection;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata CurrentClass()
		{
			return (Db4objects.Db4o.Internal.ClassMetadata)Current;
		}

		protected override object Map(object current)
		{
			return i_collection.ReadYapClass((Db4objects.Db4o.Internal.ClassMetadata)current, 
				null);
		}
	}
}
