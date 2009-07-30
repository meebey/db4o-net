/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MGetClasses : MsgD, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			ObjectContainerBase stream = Stream();
			lock (StreamLock())
			{
				try
				{
					// Since every new Client reads the class
					// collection from the file, we have to 
					// make sure, it has been written.
					stream.ClassCollection().Write(Transaction());
				}
				catch (Exception)
				{
				}
			}
			MsgD message = Msg.GetClasses.GetWriterForLength(Transaction(), Const4.IntLength 
				+ 1);
			ByteArrayBuffer writer = message.PayLoad();
			writer.WriteInt(stream.ClassCollection().GetID());
			writer.WriteByte(stream.StringIO().EncodingByte());
			return message;
		}
	}
}
