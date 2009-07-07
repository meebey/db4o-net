/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MClassMeta : MsgObject, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			ObjectContainerBase stream = Stream();
			Unmarshall();
			try
			{
				lock (StreamLock())
				{
					ClassInfo classInfo = (ClassInfo)ReadObjectFromPayLoad();
					ClassInfoHelper classInfoHelper = ServerMessageDispatcher().ClassInfoHelper();
					GenericClass genericClass = classInfoHelper.ClassMetaToGenericClass(Stream().Reflector
						(), classInfo);
					if (genericClass != null)
					{
						Transaction trans = stream.SystemTransaction();
						ClassMetadata classMetadata = stream.ProduceClassMetadata(genericClass);
						if (classMetadata != null)
						{
							stream.CheckStillToSet();
							classMetadata.SetStateDirty();
							classMetadata.Write(trans);
							trans.Commit();
							StatefulBuffer returnBytes = stream.ReadWriterByID(trans, classMetadata.GetID());
							Write(Msg.ObjectToClient.GetWriter(returnBytes));
							return true;
						}
					}
				}
			}
			catch (Exception e)
			{
			}
			Write(Msg.Failed);
			return true;
		}
	}
}
