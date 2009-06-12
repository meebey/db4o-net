/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MSetSemaphore : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			int timeout = ReadInt();
			string name = ReadString();
			LocalObjectContainer stream = (LocalObjectContainer)Stream();
			bool res = stream.SetSemaphore(Transaction(), name, timeout);
			if (res)
			{
				Write(Msg.Success);
			}
			else
			{
				Write(Msg.Failed);
			}
			return true;
		}
	}
}
