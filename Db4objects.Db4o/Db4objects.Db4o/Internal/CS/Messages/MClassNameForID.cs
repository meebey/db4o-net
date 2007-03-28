namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <summary>get the classname for an internal ID</summary>
	internal sealed class MClassNameForID : Db4objects.Db4o.Internal.CS.Messages.MsgD
		, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int id = _payLoad.ReadInt();
			string name = string.Empty;
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.ClassMetadata yapClass = Stream().ClassMetadataForId(id);
				if (yapClass != null)
				{
					name = yapClass.GetName();
				}
			}
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.CLASS_NAME_FOR_ID.GetWriterForString
				(Transaction(), name));
			return true;
		}
	}
}
