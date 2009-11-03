/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MSetSemaphore : MsgD, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			int timeout = ReadInt();
			string name = ReadString();
			LocalObjectContainer stream = (LocalObjectContainer)Stream();
			bool res = stream.SetSemaphore(Transaction(), name, timeout);
			return (res ? (Msg)Msg.Success : Msg.Failed);
		}
	}
}
