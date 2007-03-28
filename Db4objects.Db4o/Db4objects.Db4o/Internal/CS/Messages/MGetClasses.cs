namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetClasses : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			lock (StreamLock())
			{
				try
				{
					stream.ClassCollection().Write(Transaction());
				}
				catch (System.Exception)
				{
				}
			}
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = Db4objects.Db4o.Internal.CS.Messages.Msg
				.GET_CLASSES.GetWriterForLength(Transaction(), Db4objects.Db4o.Internal.Const4.INT_LENGTH
				 + 1);
			Db4objects.Db4o.Internal.Buffer writer = message.PayLoad();
			writer.WriteInt(stream.ClassCollection().GetID());
			writer.Append(stream.StringIO().EncodingByte());
			Write(message);
			return true;
		}
	}
}
