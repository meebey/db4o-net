/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MIsAlive : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Write(Msg.IsAlive);
			return true;
		}
	}
}
