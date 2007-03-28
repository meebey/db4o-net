namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCreateClass : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			Db4objects.Db4o.Internal.Transaction trans = stream.SystemTransaction();
			Db4objects.Db4o.Reflect.IReflectClass claxx = trans.Reflector().ForName(ReadString
				());
			bool ok = false;
			try
			{
				if (claxx != null)
				{
					lock (StreamLock())
					{
						Db4objects.Db4o.Internal.ClassMetadata yapClass = stream.ProduceClassMetadata(claxx
							);
						if (yapClass != null)
						{
							stream.CheckStillToSet();
							yapClass.SetStateDirty();
							yapClass.Write(trans);
							trans.Commit();
							Db4objects.Db4o.Internal.StatefulBuffer returnBytes = stream.ReadWriterByID(trans
								, yapClass.GetID());
							Db4objects.Db4o.Internal.CS.Messages.MsgD createdClass = Db4objects.Db4o.Internal.CS.Messages.Msg
								.OBJECT_TO_CLIENT.GetWriter(returnBytes);
							Write(createdClass);
							ok = true;
						}
					}
				}
			}
			catch (Db4objects.Db4o.Ext.Db4oException)
			{
			}
			finally
			{
				if (!ok)
				{
					Write(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED);
				}
			}
			return true;
		}
	}
}
