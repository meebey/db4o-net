/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;

namespace Db4objects.Db4o.Internal.Classindex
{
	/// <exclude></exclude>
	public class BTreeClassIndexStrategy : AbstractClassIndexStrategy
	{
		private BTree _btreeIndex;

		public BTreeClassIndexStrategy(ClassMetadata yapClass) : base(yapClass)
		{
		}

		public virtual BTree Btree()
		{
			return _btreeIndex;
		}

		public override int EntryCount(Transaction ta)
		{
			return _btreeIndex != null ? _btreeIndex.Size(ta) : 0;
		}

		public override void Initialize(ObjectContainerBase stream)
		{
			CreateBTreeIndex(stream, 0);
		}

		public override void Purge()
		{
		}

		public override void Read(ObjectContainerBase stream, int indexID)
		{
			ReadBTreeIndex(stream, indexID);
		}

		public override int Write(Transaction trans)
		{
			if (_btreeIndex == null)
			{
				return 0;
			}
			_btreeIndex.Write(trans);
			return _btreeIndex.GetID();
		}

		public override void TraverseAll(Transaction ta, IVisitor4 command)
		{
			// better alternatives for this null check? (has been moved as is from YapFile)
			if (_btreeIndex != null)
			{
				_btreeIndex.TraverseKeys(ta, command);
			}
		}

		private void CreateBTreeIndex(ObjectContainerBase stream, int btreeID)
		{
			if (stream.IsClient())
			{
				return;
			}
			_btreeIndex = ((LocalObjectContainer)stream).CreateBTreeClassIndex(btreeID);
			_btreeIndex.SetRemoveListener(new _IVisitor4_61(this, stream));
		}

		private sealed class _IVisitor4_61 : IVisitor4
		{
			public _IVisitor4_61(BTreeClassIndexStrategy _enclosing, ObjectContainerBase stream
				)
			{
				this._enclosing = _enclosing;
				this.stream = stream;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				stream.ReferenceSystemRegistry().RemoveId(id);
			}

			private readonly BTreeClassIndexStrategy _enclosing;

			private readonly ObjectContainerBase stream;
		}

		private void ReadBTreeIndex(ObjectContainerBase stream, int indexId)
		{
			if (!stream.IsClient() && _btreeIndex == null)
			{
				CreateBTreeIndex(stream, indexId);
			}
		}

		protected override void InternalAdd(Transaction trans, int id)
		{
			_btreeIndex.Add(trans, id);
		}

		protected override void InternalRemove(Transaction ta, int id)
		{
			_btreeIndex.Remove(ta, id);
		}

		public override void DontDelete(Transaction transaction, int id)
		{
		}

		public override void DefragReference(ClassMetadata classMetadata, DefragmentContextImpl
			 context, int classIndexID)
		{
			int newID = -classIndexID;
			context.WriteInt(newID);
		}

		public override int Id()
		{
			return _btreeIndex.GetID();
		}

		public override IEnumerator AllSlotIDs(Transaction trans)
		{
			return _btreeIndex.AllNodeIds(trans);
		}

		public override void DefragIndex(DefragmentContextImpl context)
		{
			_btreeIndex.DefragIndex(context);
		}

		public static BTree Btree(ClassMetadata clazz)
		{
			IClassIndexStrategy index = clazz.Index();
			if (!(index is Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy))
			{
				throw new InvalidOperationException();
			}
			return ((Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy)index).Btree
				();
		}

		public static IEnumerator Iterate(ClassMetadata clazz, Transaction trans)
		{
			return Btree(clazz).AsRange(trans).Keys();
		}
	}
}
