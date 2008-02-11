/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectByUuid : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			long uuid = ReadLong();
			byte[] signature = ReadBytes();
			int id = 0;
			Transaction trans = Transaction();
			lock (StreamLock())
			{
				try
				{
					HardObjectReference hardRef = trans.GetHardReferenceBySignature(uuid, signature);
					if (hardRef._reference != null)
					{
						id = hardRef._reference.GetID();
					}
				}
				catch (Exception e)
				{
				}
			}
			Write(Msg.ObjectByUuid.GetWriterForInt(trans, id));
			return true;
		}
	}
}
