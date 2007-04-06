using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetReset : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Stub(ReadInt()).Reset();
			return true;
		}
	}
}
