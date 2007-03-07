namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCreateClass : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			Db4objects.Db4o.Internal.Transaction trans = stream.GetSystemTransaction();
			Db4objects.Db4o.Reflect.IReflectClass claxx = trans.Reflector().ForName(ReadString
				());
			if (claxx == null)
			{
				return WriteFailedMessage(serverThread);
			}
			lock (StreamLock())
			{
				try
				{
					Db4objects.Db4o.Internal.ClassMetadata yapClass = stream.ProduceYapClass(claxx);
					if (yapClass == null)
					{
						return WriteFailedMessage(serverThread);
					}
					stream.CheckStillToSet();
					yapClass.SetStateDirty();
					yapClass.Write(trans);
					trans.Commit();
					Db4objects.Db4o.Internal.StatefulBuffer returnBytes = stream.ReadWriterByID(trans
						, yapClass.GetID());
					Db4objects.Db4o.Internal.CS.Messages.MsgD createdClass = Db4objects.Db4o.Internal.CS.Messages.Msg
						.OBJECT_TO_CLIENT.GetWriter(returnBytes);
					serverThread.Write(createdClass);
				}
				catch (Db4objects.Db4o.Foundation.Db4oRuntimeException)
				{
					WriteFailedMessage(serverThread);
				}
			}
			return true;
		}
	}
}
