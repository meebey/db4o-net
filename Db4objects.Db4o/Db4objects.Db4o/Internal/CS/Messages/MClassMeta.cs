/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MClassMeta : MsgObject, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			ObjectContainerBase stream = Stream();
			Unmarshall();
			try
			{
				lock (StreamLock())
				{
					ClassInfo classMeta = (ClassInfo)ReadObjectFromPayLoad();
					ClassInfoHelper classInfoHelper = ServerMessageDispatcher().ClassInfoHelper();
					GenericClass genericClass = classInfoHelper.ClassMetaToGenericClass(Stream().Reflector
						(), classMeta);
					if (genericClass != null)
					{
						Transaction trans = stream.SystemTransaction();
						ClassMetadata yapClass = stream.ProduceClassMetadata(genericClass);
						if (yapClass != null)
						{
							stream.CheckStillToSet();
							yapClass.SetStateDirty();
							yapClass.Write(trans);
							trans.Commit();
							StatefulBuffer returnBytes = stream.ReadWriterByID(trans, yapClass.GetID());
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
