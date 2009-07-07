/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <summary>get the classname for an internal ID</summary>
	public sealed class MClassNameForID : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			int id = _payLoad.ReadInt();
			string name = string.Empty;
			lock (StreamLock())
			{
				ClassMetadata classMetadata = Stream().ClassMetadataForID(id);
				if (classMetadata != null)
				{
					name = classMetadata.GetName();
				}
			}
			Write(Msg.ClassNameForId.GetWriterForString(Transaction(), name));
			return true;
		}
	}
}
