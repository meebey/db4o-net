/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MReleaseSemaphore : MsgD, IServerSideMessage
	{
		public void ProcessAtServer()
		{
			string name = ReadString();
			((LocalObjectContainer)Stream()).ReleaseSemaphore(Transaction(), name);
		}
	}
}
