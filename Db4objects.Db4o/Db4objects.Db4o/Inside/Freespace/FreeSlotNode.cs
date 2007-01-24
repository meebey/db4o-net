namespace Db4objects.Db4o.Inside.Freespace
{
	/// <exclude></exclude>
	public sealed class FreeSlotNode : Db4objects.Db4o.TreeInt
	{
		internal static int sizeLimit;

		internal Db4objects.Db4o.Inside.Freespace.FreeSlotNode _peer;

		internal FreeSlotNode(int a_key) : base(a_key)
		{
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Inside.Freespace.FreeSlotNode frslot = new Db4objects.Db4o.Inside.Freespace.FreeSlotNode
				(_key);
			frslot._peer = _peer;
			return base.ShallowCloneInternal(frslot);
		}

		internal void CreatePeer(int a_key)
		{
			_peer = new Db4objects.Db4o.Inside.Freespace.FreeSlotNode(a_key);
			_peer._peer = this;
		}

		public override bool Duplicates()
		{
			return true;
		}

		public sealed override int OwnLength()
		{
			return Db4objects.Db4o.YapConst.INT_LENGTH * 2;
		}

		internal static Db4objects.Db4o.Foundation.Tree RemoveGreaterOrEqual(Db4objects.Db4o.Inside.Freespace.FreeSlotNode
			 a_in, Db4objects.Db4o.TreeIntObject a_finder)
		{
			if (a_in == null)
			{
				return null;
			}
			int cmp = a_in._key - a_finder._key;
			if (cmp == 0)
			{
				a_finder._object = a_in;
				return a_in.Remove();
			}
			if (cmp > 0)
			{
				a_in._preceding = RemoveGreaterOrEqual((Db4objects.Db4o.Inside.Freespace.FreeSlotNode
					)a_in._preceding, a_finder);
				if (a_finder._object != null)
				{
					a_in._size--;
					return a_in;
				}
				a_finder._object = a_in;
				return a_in.Remove();
			}
			a_in._subsequent = RemoveGreaterOrEqual((Db4objects.Db4o.Inside.Freespace.FreeSlotNode
				)a_in._subsequent, a_finder);
			if (a_finder._object != null)
			{
				a_in._size--;
			}
			return a_in;
		}

		public override object Read(Db4objects.Db4o.YapReader a_reader)
		{
			int size = a_reader.ReadInt();
			int address = a_reader.ReadInt();
			if (size > sizeLimit)
			{
				Db4objects.Db4o.Inside.Freespace.FreeSlotNode node = new Db4objects.Db4o.Inside.Freespace.FreeSlotNode
					(size);
				node.CreatePeer(address);
				return node;
			}
			return null;
		}

		public sealed override void Write(Db4objects.Db4o.YapReader a_writer)
		{
			a_writer.WriteInt(_key);
			a_writer.WriteInt(_peer._key);
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "FreeSlotNode " + _key;
			if (_peer != null)
			{
				str += " peer: " + _peer._key;
			}
			return str;
		}
	}
}
