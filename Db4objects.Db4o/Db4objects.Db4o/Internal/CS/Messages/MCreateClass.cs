/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCreateClass : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			ObjectContainerBase stream = Stream();
			Transaction trans = stream.SystemTransaction();
			IReflectClass claxx = trans.Reflector().ForName(ReadString());
			bool ok = false;
			try
			{
				if (claxx != null)
				{
					lock (StreamLock())
					{
						ClassMetadata classMetadata = stream.ProduceClassMetadata(claxx);
						if (classMetadata != null)
						{
							stream.CheckStillToSet();
							StatefulBuffer returnBytes = stream.ReadWriterByID(trans, classMetadata.GetID());
							MsgD createdClass = Msg.OBJECT_TO_CLIENT.GetWriter(returnBytes);
							Write(createdClass);
							ok = true;
						}
					}
				}
			}
			catch (Db4oException)
			{
			}
			finally
			{
				if (!ok)
				{
					Write(Msg.FAILED);
				}
			}
			return true;
		}
	}
}
