/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCommit : Msg, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			CallbackObjectInfoCollections committedInfo = null;
			LocalTransaction serverTransaction = ServerTransaction();
			IServerMessageDispatcher dispatcher = ServerMessageDispatcher();
			lock (StreamLock())
			{
				serverTransaction.Commit(dispatcher);
				committedInfo = dispatcher.CommittedInfo();
			}
			Write(Msg.Ok);
			try
			{
				if (committedInfo != null)
				{
					AddCommittedInfoMsg(committedInfo, serverTransaction);
				}
			}
			catch (Exception exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
			return true;
		}

		private void AddCommittedInfoMsg(CallbackObjectInfoCollections committedInfo, LocalTransaction
			 serverTransaction)
		{
			lock (StreamLock())
			{
				Msg.CommittedInfo.SetTransaction(serverTransaction);
				MCommittedInfo message = Msg.CommittedInfo.Encode(committedInfo);
				message.SetMessageDispatcher(ServerMessageDispatcher());
				ServerMessageDispatcher().Server().AddCommittedInfoMsg(message);
			}
		}
	}
}
