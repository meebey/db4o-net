namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MCommitResponse : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		private const int EXCEPTION_FLAG_BYTE = 1;

		private const byte WITH_EXCEPTION = (byte)1;

		private const byte NO_EXCEPTION = (byte)0;

		public static Db4objects.Db4o.Internal.CS.Messages.MsgD CreateWithException(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Ext.Db4oException db4oException)
		{
			Db4objects.Db4o.Internal.SerializedGraph serialized = Db4objects.Db4o.Internal.Serializer
				.Marshall(trans.Stream(), db4oException);
			int length = EXCEPTION_FLAG_BYTE + serialized.MarshalledLength();
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.COMMIT_RESPONSE.GetWriterForLength(trans, length);
			msg._payLoad.Append(WITH_EXCEPTION);
			serialized.Write(msg._payLoad);
			return msg;
		}

		public static Db4objects.Db4o.Internal.CS.Messages.MsgD CreateWithoutException(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return Db4objects.Db4o.Internal.CS.Messages.Msg.COMMIT_RESPONSE.GetWriterForByte(
				trans, NO_EXCEPTION);
		}

		public virtual Db4objects.Db4o.Ext.Db4oException ReadException()
		{
			byte b = _payLoad.ReadByte();
			if (b == NO_EXCEPTION)
			{
				return null;
			}
			return (Db4objects.Db4o.Ext.Db4oException)Db4objects.Db4o.Internal.Serializer.Unmarshall
				(Stream(), Db4objects.Db4o.Internal.SerializedGraph.Read(_payLoad));
		}
	}
}
