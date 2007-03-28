namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MVersion : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			long ver = 0;
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			lock (stream)
			{
				ver = stream.CurrentVersion();
			}
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.ID_LIST.GetWriterForLong(Transaction
				(), ver));
			return true;
		}
	}
}
