/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Collections;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Collections
{
	/// <exclude></exclude>
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class BigSetTypeHandler : ITypeHandler4
	{
		public virtual void Defragment(IDefragmentContext context)
		{
			int pos = context.Offset();
			int id = context.ReadInt();
			BTree bTree = NewBTree(context, id);
			DefragmentServicesImpl services = (DefragmentServicesImpl)context.Services();
			IDMappingCollector collector = new IDMappingCollector();
			services.RegisterBTreeIDs(bTree, collector);
			collector.Flush(services);
			context.Seek(pos);
			context.CopyID();
			bTree.DefragBTree(services);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			InvalidBigSet(context);
			int id = context.ReadInt();
			FreeBTree(context, id);
		}

		private void InvalidBigSet(IDeleteContext context)
		{
			IBigSetPersistence bigSet = (IBigSetPersistence)context.Transaction().ObjectForIdFromCache
				(context.Id());
			if (bigSet != null)
			{
				bigSet.Invalidate();
			}
		}

		private void FreeBTree(IDeleteContext context, int id)
		{
			BTree bTree = NewBTree(context, id);
			bTree.Free(SystemTransaction(context));
			bTree = null;
		}

		private static Transaction SystemTransaction(IContext context)
		{
			return context.Transaction().SystemTransaction();
		}

		private BTree NewBTree(IContext context, int id)
		{
			return new BTree(SystemTransaction(context), id, new IDHandler());
		}

		public virtual object Read(IReadContext context)
		{
			IBigSetPersistence bigSet = (IBigSetPersistence)((UnmarshallingContext)context).PersistentObject
				();
			bigSet.Read(context);
			return bigSet;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			IBigSetPersistence bigSet = (IBigSetPersistence)obj;
			bigSet.Write(context);
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			// TODO Auto-generated method stub
			return null;
		}
	}
}
