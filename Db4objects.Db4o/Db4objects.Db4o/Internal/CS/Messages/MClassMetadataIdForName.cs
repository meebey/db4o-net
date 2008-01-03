/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MClassMetadataIdForName : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			string name = ReadString();
			ObjectContainerBase stream = Stream();
			Transaction trans = stream.SystemTransaction();
			bool ok = false;
			try
			{
				lock (StreamLock())
				{
					int id = stream.ClassMetadataIdForName(name);
					MsgD msg = Msg.ClassId.GetWriterForInt(trans, id);
					Write(msg);
					ok = true;
				}
			}
			catch (Db4oException)
			{
			}
			finally
			{
				if (!ok)
				{
					Write(Msg.Failed);
				}
			}
			return true;
		}
	}
}
