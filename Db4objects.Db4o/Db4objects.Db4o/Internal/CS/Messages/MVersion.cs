/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MVersion : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			long ver = 0;
			ObjectContainerBase stream = Stream();
			lock (stream)
			{
				ver = stream.CurrentVersion();
			}
			Write(Msg.IdList.GetWriterForLong(Transaction(), ver));
			return true;
		}
	}
}
