namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public interface IServerMessageDispatcher : Db4objects.Db4o.Internal.CS.Messages.IMessageDispatcher
	{
		string Name();

		void QueryResultFinalized(int queryResultID);

		Db4objects.Db4o.Foundation.Network.ISocket4 Socket();

		int DispatcherID();

		Db4objects.Db4o.Internal.CS.LazyClientObjectSetStub QueryResultForID(int queryResultID
			);

		void SwitchToMainFile();

		void SwitchToFile(Db4objects.Db4o.Internal.CS.Messages.MSwitchToFile file);

		void UseTransaction(Db4objects.Db4o.Internal.CS.Messages.MUseTransaction transaction
			);

		void MapQueryResultToID(Db4objects.Db4o.Internal.CS.LazyClientObjectSetStub stub, 
			int queryResultId);

		Db4objects.Db4o.Internal.CS.ObjectServerImpl Server();

		void Login();

		bool Close();
	}
}
