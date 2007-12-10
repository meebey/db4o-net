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
		void ProcessObjectSlot(DefragmentServicesImpl context, ClassMetadata yapClass, int
			 id);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessClass(DefragmentServicesImpl context, ClassMetadata yapClass, int id, 
			int classIndexID);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessClassCollection(DefragmentServicesImpl context);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessBTree(DefragmentServicesImpl context, BTree btree);

		void Flush(DefragmentServicesImpl context);
	}
}
