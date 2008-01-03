/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MSetSemaphore : MsgD, IServerSideMessage
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
