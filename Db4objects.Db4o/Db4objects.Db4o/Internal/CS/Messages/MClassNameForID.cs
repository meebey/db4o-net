/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <summary>get the classname for an internal ID</summary>
	public sealed class MClassNameForID : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int id = _payLoad.ReadInt();
			string name = string.Empty;
			lock (StreamLock())
			{
				ClassMetadata yapClass = Stream().ClassMetadataForId(id);
				if (yapClass != null)
				{
					name = yapClass.GetName();
				}
			}
			Write(Msg.ClassNameForId.GetWriterForString(Transaction(), name));
			return true;
		}
	}
}
