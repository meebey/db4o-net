namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MClassMeta : Db4objects.Db4o.Internal.CS.Messages.MsgObject
	{
		public override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			Unmarshall();
			try
			{
				Db4objects.Db4o.Internal.CS.ClassInfo classMeta = (Db4objects.Db4o.Internal.CS.ClassInfo
					)Stream().Unmarshall(_payLoad);
				Db4objects.Db4o.Reflect.Generic.GenericClass genericClass = stream.GetClassMetaHelper
					().ClassMetaToGenericClass(Stream().Reflector(), classMeta);
				if (genericClass != null)
				{
					lock (StreamLock())
					{
						Db4objects.Db4o.Internal.Transaction trans = stream.GetSystemTransaction();
						Db4objects.Db4o.Internal.ClassMetadata yapClass = stream.ProduceYapClass(genericClass
							);
						if (yapClass != null)
						{
							stream.CheckStillToSet();
							yapClass.SetStateDirty();
							yapClass.Write(trans);
							trans.Commit();
							Db4objects.Db4o.Internal.StatefulBuffer returnBytes = stream.ReadWriterByID(trans
								, yapClass.GetID());
							serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter
								(returnBytes));
							return true;
						}
					}
				}
			}
			catch (System.Exception e)
			{
			}
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED);
			return true;
		}
	}
}
