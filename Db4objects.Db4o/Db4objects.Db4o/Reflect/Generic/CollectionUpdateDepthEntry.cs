namespace Db4objects.Db4o.Reflect.Generic
{
	internal class CollectionUpdateDepthEntry
	{
		internal readonly Db4objects.Db4o.Reflect.IReflectClassPredicate _predicate;

		internal readonly int _depth;

		internal CollectionUpdateDepthEntry(Db4objects.Db4o.Reflect.IReflectClassPredicate
			 predicate, int depth)
		{
			_predicate = predicate;
			_depth = depth;
		}
	}
}
