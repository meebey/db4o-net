namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCreateClass : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			Db4objects.Db4o.Internal.Transaction trans = stream.GetSystemTransaction();
			try
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = trans.Reflector().ForName(ReadString
					());
				if (claxx != null)
				{
					lock (StreamLock())
					{
						try
						{
							Db4objects.Db4o.Internal.ClassMetadata yapClass = stream.ProduceYapClass(claxx);
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
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED);
			return true;
		}
	}
}
