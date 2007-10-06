/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
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
		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessObjectSlot(DefragContextImpl context, ClassMetadata yapClass, int id);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessClass(DefragContextImpl context, ClassMetadata yapClass, int id, int 
			classIndexID);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessClassCollection(DefragContextImpl context);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessBTree(DefragContextImpl context, BTree btree);

		void Flush(DefragContextImpl context);
	}
}
