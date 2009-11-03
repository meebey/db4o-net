/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MClassMetadataIdForName : MsgD, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			string name = ReadString();
			ObjectContainerBase stream = Stream();
			Transaction trans = stream.SystemTransaction();
			try
			{
				lock (StreamLock())
				{
					int id = stream.ClassMetadataIdForName(name);
					return Msg.ClassId.GetWriterForInt(trans, id);
				}
			}
			catch (Db4oException)
			{
			}
			// TODO: send the exception to the client
			return Msg.Failed;
		}
	}
}
