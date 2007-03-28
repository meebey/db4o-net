namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUserMessage : Db4objects.Db4o.Internal.CS.Messages.MsgObject
		, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			if (MessageRecipient() != null)
			{
				Unmarshall();
				MessageRecipient().ProcessMessage(Stream(), ReadObjectFromPayLoad());
			}
			return true;
		}

		private Db4objects.Db4o.Messaging.IMessageRecipient MessageRecipient()
		{
			return Config().MessageRecipient();
		}
	}
}
