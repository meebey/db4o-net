namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUserMessage : Db4objects.Db4o.Internal.CS.Messages.MsgObject
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			if (MessageRecipient() != null)
			{
				Unmarshall();
				MessageRecipient().ProcessMessage(Stream(), Stream().Unmarshall(_payLoad));
			}
			return true;
		}

		private Db4objects.Db4o.Messaging.IMessageRecipient MessageRecipient()
		{
			return Config().MessageRecipient();
		}
	}
}
