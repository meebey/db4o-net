namespace Db4objects.Db4o.CS.Messages
{
	public class MClassMeta : Db4objects.Db4o.CS.Messages.MsgObject
	{
		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			Unmarshall();
			Db4objects.Db4o.CS.ClassMeta classMeta = (Db4objects.Db4o.CS.ClassMeta)Stream().Unmarshall
				(_payLoad);
			Db4objects.Db4o.Reflect.Generic.GenericClass genericClass = stream.GetClassMetaHelper
				().ClassMetaToGenericClass(Stream().Reflector(), classMeta);
			if (genericClass != null)
			{
				lock (StreamLock())
				{
					Db4objects.Db4o.Transaction trans = stream.GetSystemTransaction();
					Db4objects.Db4o.YapWriter returnBytes = new Db4objects.Db4o.YapWriter(trans, 0);
					Db4objects.Db4o.YapClass yapClass = stream.GetYapClass(genericClass, true);
					if (yapClass != null)
					{
						stream.CheckStillToSet();
						yapClass.SetStateDirty();
						yapClass.Write(trans);
						trans.Commit();
						returnBytes = stream.ReadWriterByID(trans, yapClass.GetID());
						serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(returnBytes
							));
						return true;
					}
				}
			}
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.FAILED);
			return true;
		}
	}
}
