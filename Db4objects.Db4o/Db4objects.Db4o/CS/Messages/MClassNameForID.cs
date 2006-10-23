namespace Db4objects.Db4o.CS.Messages
{
	/// <summary>get the classname for an internal ID</summary>
	internal sealed class MClassNameForID : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			int id = _payLoad.ReadInt();
			string name = "";
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				try
				{
					Db4objects.Db4o.YapClass yapClass = stream.GetYapClass(id);
					if (yapClass != null)
					{
						name = yapClass.GetName();
					}
				}
				catch (System.Exception t)
				{
				}
			}
			Db4objects.Db4o.CS.Messages.Msg.CLASS_NAME_FOR_ID.GetWriterForString(GetTransaction
				(), name).Write(stream, sock);
			return true;
		}
	}
}
