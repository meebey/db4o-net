namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MRaiseVersion : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			long minimumVersion = ReadLong();
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			lock (stream)
			{
				stream.RaiseVersion(minimumVersion);
			}
			return true;
		}
	}
}
