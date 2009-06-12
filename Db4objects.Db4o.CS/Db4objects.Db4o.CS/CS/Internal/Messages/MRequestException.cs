/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MRequestException : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			throw new Db4oException();
		}
	}
}
