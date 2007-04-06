using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MRaiseVersion : MsgD, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			long minimumVersion = ReadLong();
			ObjectContainerBase stream = Stream();
			lock (stream)
			{
				stream.RaiseVersion(minimumVersion);
			}
			return true;
		}
	}
}
