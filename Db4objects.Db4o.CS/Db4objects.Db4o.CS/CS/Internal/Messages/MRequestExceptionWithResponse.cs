/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Internal.Messages;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MRequestExceptionWithResponse : MsgD, IMessageWithResponse
	{
		public virtual Msg ReplyFromServer()
		{
			throw ((Exception)ReadSingleObject());
		}
	}
}
