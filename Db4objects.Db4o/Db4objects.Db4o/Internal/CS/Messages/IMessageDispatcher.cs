namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public interface IMessageDispatcher
	{
		bool IsMessageDispatcherAlive();

		void Write(Db4objects.Db4o.Internal.CS.Messages.Msg msg);

		bool Close();

		void SetDispatcherName(string name);

		void StartDispatcher();
	}
}
