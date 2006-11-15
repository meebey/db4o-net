namespace Db4objects.Db4o.Defragment
{
	/// <summary>Implements one step in the defragmenting process.</summary>
	/// <remarks>Implements one step in the defragmenting process.</remarks>
	/// <exclude></exclude>
	internal interface IPassCommand
	{
		void ProcessObjectSlot(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.YapClass
			 yapClass, int id, bool registerAddresses);

		void ProcessClass(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.YapClass
			 yapClass, int id, int classIndexID);

		void ProcessClassCollection(Db4objects.Db4o.Defragment.DefragContextImpl context);

		void ProcessBTree(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.Inside.Btree.BTree
			 btree);

		void Flush(Db4objects.Db4o.Defragment.DefragContextImpl context);
	}
}
