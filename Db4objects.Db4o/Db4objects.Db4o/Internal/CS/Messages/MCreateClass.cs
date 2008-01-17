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
			bool ok = false;
			try
			{
				lock (StreamLock())
				{
					IReflectClass claxx = trans.Reflector().ForName(ReadString());
					if (claxx != null)
					{
						ClassMetadata classMetadata = stream.ProduceClassMetadata(claxx);
						if (classMetadata != null)
						{
							stream.CheckStillToSet();
							StatefulBuffer returnBytes = stream.ReadWriterByID(trans, classMetadata.GetID());
							MsgD createdClass = Msg.ObjectToClient.GetWriter(returnBytes);
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
				// TODO: send the exception to the client
				if (!ok)
				{
					Write(Msg.Failed);
				}
			}
			return true;
		}
	}
}
