/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MRuntimeException : MsgD
	{
		public virtual void ThrowPayload()
		{
			throw ((Exception)ReadSingleObject());
		}
	}
}
