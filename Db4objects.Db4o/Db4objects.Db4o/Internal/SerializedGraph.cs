namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class SerializedGraph
	{
		public readonly int _id;

		public readonly byte[] _bytes;

		public SerializedGraph(int id, byte[] bytes)
		{
			_id = id;
			_bytes = bytes;
		}

		public virtual int Length()
		{
			return _bytes.Length;
		}

		public virtual int MarshalledLength()
		{
			return (Db4objects.Db4o.Internal.Const4.INT_LENGTH * 2) + Length();
		}

		public virtual void Write(Db4objects.Db4o.Internal.Buffer buffer)
		{
			buffer.WriteInt(_id);
			buffer.WriteInt(Length());
			buffer.Append(_bytes);
		}

		public static Db4objects.Db4o.Internal.SerializedGraph Read(Db4objects.Db4o.Internal.Buffer
			 buffer)
		{
			int id = buffer.ReadInt();
			int length = buffer.ReadInt();
			return new Db4objects.Db4o.Internal.SerializedGraph(id, buffer.ReadBytes(length));
		}
	}
}
