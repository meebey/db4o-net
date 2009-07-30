/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MIdentity : Msg, IMessageWithResponse
	{
		public virtual Msg ReplyFromServer()
		{
			ObjectContainerBase stream = Stream();
			return RespondInt(stream.GetID(Transaction(), ((IInternalObjectContainer)stream).
				Identity()));
		}
	}
}
