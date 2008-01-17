/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetClasses : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
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
			BufferImpl writer = message.PayLoad();
			writer.WriteInt(stream.ClassCollection().GetID());
			writer.WriteByte(stream.StringIO().EncodingByte());
			Write(message);
			return true;
		}
	}
}
