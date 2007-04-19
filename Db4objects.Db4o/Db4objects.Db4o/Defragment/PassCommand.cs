using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>Implements one step in the defragmenting process.</summary>
	/// <remarks>Implements one step in the defragmenting process.</remarks>
	/// <exclude></exclude>
	internal interface IPassCommand
	{
		void ProcessObjectSlot(DefragContextImpl context, ClassMetadata yapClass, int id, 
			bool registerAddresses);

		void ProcessClass(DefragContextImpl context, ClassMetadata yapClass, int id, int 
			classIndexID);

		void ProcessClassCollection(DefragContextImpl context);

		void ProcessBTree(DefragContextImpl context, BTree btree);

		void Flush(DefragContextImpl context);
	}
}
