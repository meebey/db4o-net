/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MCommit : Msg, IMessageWithResponse
	{
		private CallbackObjectInfoCollections committedInfo = null;

		public Msg ReplyFromServer()
		{
			IServerMessageDispatcher dispatcher = ServerMessageDispatcher();
			lock (ContainerLock())
			{
				ServerTransaction().Commit(dispatcher);
				committedInfo = dispatcher.CommittedInfo();
			}
			return Msg.Ok;
		}

		public override void PostProcessAtServer()
		{
			try
			{
				if (committedInfo != null)
				{
					AddCommittedInfoMsg(committedInfo, ServerTransaction());
				}
			}
			catch (Exception exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
		}

		private void AddCommittedInfoMsg(CallbackObjectInfoCollections committedInfo, LocalTransaction
			 serverTransaction)
		{
			lock (ContainerLock())
			{
				Msg.CommittedInfo.SetTransaction(serverTransaction);
				MCommittedInfo message = Msg.CommittedInfo.Encode(committedInfo);
				message.SetMessageDispatcher(ServerMessageDispatcher());
				ServerMessageDispatcher().Server().AddCommittedInfoMsg(message);
			}
		}
	}
}
