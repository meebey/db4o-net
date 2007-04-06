using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MDb4oException : MsgD
	{
		public virtual MDb4oException Clone(Transaction trans, Db4oException e)
		{
			SerializedGraph serialized = Serializer.Marshall(trans.Stream(), e);
			MDb4oException msg = (MDb4oException)GetWriterForLength(trans, serialized.MarshalledLength
				());
			serialized.Write(msg._payLoad);
			return msg;
		}

		public virtual Db4oException Exception()
		{
			return (Db4oException)Serializer.Unmarshall(Stream(), SerializedGraph.Read(_payLoad
				));
		}
	}
}
