/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public interface IServerMessageDispatcher : IMessageDispatcher, ICommittedCallbackDispatcher
	{
		string Name();

		void QueryResultFinalized(int queryResultID);

		ISocket4 Socket();

		int DispatcherID();

		LazyClientObjectSetStub QueryResultForID(int queryResultID);

		void SwitchToMainFile();

		void SwitchToFile(MSwitchToFile file);

		void UseTransaction(MUseTransaction transaction);

		void MapQueryResultToID(LazyClientObjectSetStub stub, int queryResultId);

		ObjectServerImpl Server();

		void Login();

		bool Close();

		void CloseConnection();

		void CaresAboutCommitted(bool care);

		bool CaresAboutCommitted();

		bool Write(Msg msg);

		CallbackObjectInfoCollections CommittedInfo();

		Db4objects.Db4o.Internal.CS.ClassInfoHelper ClassInfoHelper();
	}
}
