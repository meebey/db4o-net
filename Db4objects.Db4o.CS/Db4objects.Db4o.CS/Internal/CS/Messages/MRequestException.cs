/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MRequestException : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			throw new Db4oException();
		}
	}
}
