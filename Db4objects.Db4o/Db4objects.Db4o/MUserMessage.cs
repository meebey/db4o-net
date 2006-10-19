namespace Db4objects.Db4o
{
	internal sealed class MUserMessage : Db4objects.Db4o.MsgObject
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			if (stream.ConfigImpl().MessageRecipient() != null)
			{
				this.Unmarshall();
				stream.ConfigImpl().MessageRecipient().ProcessMessage(stream, stream.Unmarshall(_payLoad
					));
			}
			return true;
		}
	}
}
