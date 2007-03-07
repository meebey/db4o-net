namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <summary>get the classname for an internal ID</summary>
	internal sealed class MClassNameForID : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int id = _payLoad.ReadInt();
			string name = string.Empty;
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.ClassMetadata yapClass = Stream().GetYapClass(id);
				if (yapClass != null)
				{
					name = yapClass.GetName();
				}
			}
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.CLASS_NAME_FOR_ID.GetWriterForString
				(Transaction(), name));
			return true;
		}
	}
}
