/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MRaiseVersion : MsgD, IServerSideMessage
	{
		public virtual void ProcessAtServer()
		{
			long minimumVersion = ReadLong();
			ObjectContainerBase stream = Stream();
			lock (stream)
			{
				stream.RaiseVersion(minimumVersion);
			}
		}
	}
}
