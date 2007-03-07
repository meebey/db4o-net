namespace Db4objects.Db4o.Internal.CS
{
	/// <summary>Defines a strategy on how to prefetch objects from the server.</summary>
	/// <remarks>Defines a strategy on how to prefetch objects from the server.</remarks>
	public interface IPrefetchingStrategy
	{
		int PrefetchObjects(Db4objects.Db4o.Internal.CS.ClientObjectContainer container, 
			Db4objects.Db4o.Foundation.IIntIterator4 ids, object[] prefetched, int prefetchCount
			);
	}
}
