/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Classindex
{
	/// <summary>representation to collect and hold all IDs of one class</summary>
	public class ClassIndex : PersistentBase, IReadWriteable
	{
		private readonly ClassMetadata _clazz;

		/// <summary>contains TreeInt with object IDs</summary>
		private TreeInt i_root;

		internal ClassIndex(ClassMetadata yapClass)
		{
			_clazz = yapClass;
		}

		public virtual void Add(int a_id)
		{
			i_root = TreeInt.Add(i_root, a_id);
		}

		public int MarshalledLength()
		{
			return Const4.IntLength * (Tree.Size(i_root) + 1);
		}

		public void Clear()
		{
			i_root = null;
		}

		internal virtual void EnsureActive(Transaction trans)
		{
			if (!IsActive())
			{
				SetStateDirty();
				Read(trans);
			}
		}

		internal virtual int EntryCount(Transaction ta)
		{
			if (IsActive() || IsNew())
			{
				return Tree.Size(i_root);
			}
			Slot slot = ((LocalTransaction)ta).GetCurrentSlotOfID(GetID());
			int length = Const4.IntLength;
			BufferImpl reader = new BufferImpl(length);
			reader.ReadEncrypt(ta.Container(), slot.Address());
			return reader.ReadInt();
		}

		public sealed override byte GetIdentifier()
		{
			return Const4.Yapindex;
		}

		internal virtual TreeInt GetRoot()
		{
			return i_root;
		}

		public sealed override int OwnLength()
		{
			return Const4.ObjectLength + MarshalledLength();
		}

		public object Read(BufferImpl a_reader)
		{
			throw Exceptions4.VirtualException();
		}

		public sealed override void ReadThis(Transaction a_trans, BufferImpl a_reader)
		{
			i_root = (TreeInt)new TreeReader(a_reader, new TreeInt(0)).Read();
		}

		public virtual void Remove(int a_id)
		{
			i_root = TreeInt.RemoveLike(i_root, a_id);
		}

		internal virtual void SetDirty(ObjectContainerBase a_stream)
		{
			// TODO: get rid of the setDirty call
			a_stream.SetDirtyInSystemTransaction(this);
		}

		public virtual void Write(BufferImpl a_writer)
		{
			WriteThis(null, a_writer);
		}

		public sealed override void WriteThis(Transaction trans, BufferImpl a_writer)
		{
			TreeInt.Write(a_writer, i_root);
		}

		public override string ToString()
		{
			return base.ToString();
			return _clazz + " index";
		}
	}
}
