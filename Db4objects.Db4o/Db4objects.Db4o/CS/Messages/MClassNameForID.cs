namespace Db4objects.Db4o.CS.Messages
{
	/// <summary>get the classname for an internal ID</summary>
	internal sealed class MClassNameForID : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int id = _payLoad.ReadInt();
			string name = string.Empty;
			lock (StreamLock())
			{
				try
				{
					Db4objects.Db4o.YapClass yapClass = Stream().GetYapClass(id);
					if (yapClass != null)
					{
						name = yapClass.GetName();
					}
				}
				catch
				{
				}
			}
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.CLASS_NAME_FOR_ID.GetWriterForString
				(Transaction(), name));
			return true;
		}
	}
}
