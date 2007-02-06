namespace Db4objects.Db4o.Internal.Classindex
{
	/// <summary>representation to collect and hold all IDs of one class</summary>
	public class ClassIndex : Db4objects.Db4o.Internal.PersistentBase, Db4objects.Db4o.Internal.IReadWriteable
	{
		private readonly Db4objects.Db4o.Internal.ClassMetadata _yapClass;

		/// <summary>contains TreeInt with object IDs</summary>
		private Db4objects.Db4o.Internal.TreeInt i_root;

		internal ClassIndex(Db4objects.Db4o.Internal.ClassMetadata yapClass)
		{
			_yapClass = yapClass;
		}

		public virtual void Add(int a_id)
		{
			i_root = Db4objects.Db4o.Internal.TreeInt.Add(i_root, a_id);
		}

		public int ByteCount()
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH * (Db4objects.Db4o.Foundation.Tree
				.Size(i_root) + 1);
		}

		public void Clear()
		{
			i_root = null;
		}

		internal virtual void EnsureActive(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (!IsActive())
			{
				SetStateDirty();
				Read(trans);
			}
		}

		internal virtual int EntryCount(Db4objects.Db4o.Internal.Transaction ta)
		{
			if (IsActive() || IsNew())
			{
				return Db4objects.Db4o.Foundation.Tree.Size(i_root);
			}
			Db4objects.Db4o.Internal.Slots.Slot slot = ((Db4objects.Db4o.Internal.LocalTransaction
				)ta).GetCurrentSlotOfID(GetID());
			int length = Db4objects.Db4o.Internal.Const4.INT_LENGTH;
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(length
				);
			reader.ReadEncrypt(ta.Stream(), slot._address);
			return reader.ReadInt();
		}

		public sealed override byte GetIdentifier()
		{
			return Db4objects.Db4o.Internal.Const4.YAPINDEX;
		}

		internal virtual Db4objects.Db4o.Internal.TreeInt GetRoot()
		{
			return i_root;
		}

		public sealed override int OwnLength()
		{
			return Db4objects.Db4o.Internal.Const4.OBJECT_LENGTH + ByteCount();
		}

		public object Read(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}

		public sealed override void ReadThis(Db4objects.Db4o.Internal.Transaction a_trans
			, Db4objects.Db4o.Internal.Buffer a_reader)
		{
			i_root = (Db4objects.Db4o.Internal.TreeInt)new Db4objects.Db4o.Internal.TreeReader
				(a_reader, new Db4objects.Db4o.Internal.TreeInt(0)).Read();
		}

		public virtual void Remove(int a_id)
		{
			i_root = Db4objects.Db4o.Internal.TreeInt.RemoveLike(i_root, a_id);
		}

		internal virtual void SetDirty(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			)
		{
			a_stream.SetDirtyInSystemTransaction(this);
		}

		public virtual void Write(Db4objects.Db4o.Internal.Buffer a_writer)
		{
			WriteThis(null, a_writer);
		}

		public sealed override void WriteThis(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Buffer a_writer)
		{
			Db4objects.Db4o.Internal.TreeInt.Write(a_writer, i_root);
		}

		public override string ToString()
		{
			return base.ToString();
			return _yapClass + " index";
		}
	}
}
