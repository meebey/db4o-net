using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public interface IMessageDispatcher
	{
		bool IsMessageDispatcherAlive();

		void Write(Msg msg);

		bool Close();

		void SetDispatcherName(string name);

		void StartDispatcher();
	}
}
