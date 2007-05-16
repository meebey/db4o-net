/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCommit : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			try
			{
				CallbackObjectInfoCollections committedInfo = null;
				LocalTransaction serverTransaction = ServerTransaction();
				IServerMessageDispatcher dispatcher = ServerMessageDispatcher();
				lock (StreamLock())
				{
					serverTransaction.Commit(dispatcher);
					committedInfo = dispatcher.CommittedInfo();
				}
				Write(Msg.OK);
				if (committedInfo != null)
				{
					AddCommittedInfoMsg(committedInfo, serverTransaction);
				}
			}
			catch (Db4oException e)
			{
				WriteException(e);
			}
			return true;
		}

		private void AddCommittedInfoMsg(CallbackObjectInfoCollections committedInfo, LocalTransaction
			 serverTransaction)
		{
			Msg.COMMITTED_INFO.SetTransaction(serverTransaction);
			MCommittedInfo message = Msg.COMMITTED_INFO.Encode(committedInfo);
			message.SetMessageDispatcher(ServerMessageDispatcher());
			ServerMessageDispatcher().Server().AddCommittedInfoMsg(message);
		}
	}
}
