/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadMultipleObjects : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int size = ReadInt();
			MsgD[] ret = new MsgD[size];
			int length = (1 + size) * Const4.IntLength;
			lock (StreamLock())
			{
				for (int i = 0; i < size; i++)
				{
					int id = this._payLoad.ReadInt();
					try
					{
						StatefulBuffer bytes = Stream().ReadWriterByID(Transaction(), id);
						if (bytes != null)
						{
							ret[i] = Msg.ObjectToClient.GetWriter(bytes);
							length += ret[i]._payLoad.Length();
						}
					}
					catch (Exception e)
					{
					}
				}
			}
			MsgD multibytes = Msg.ReadMultipleObjects.GetWriterForLength(Transaction(), length
				);
			multibytes.WriteInt(size);
			for (int i = 0; i < size; i++)
			{
				if (ret[i] == null)
				{
					multibytes.WriteInt(0);
				}
				else
				{
					multibytes.WriteInt(ret[i]._payLoad.Length());
					multibytes._payLoad.Append(ret[i]._payLoad._buffer);
				}
			}
			Write(multibytes);
			return true;
		}
	}
}
