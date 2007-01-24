namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MCreateClass : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			Db4objects.Db4o.Transaction trans = stream.GetSystemTransaction();
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
							Db4objects.Db4o.YapClass yapClass = stream.ProduceYapClass(claxx);
							if (yapClass != null)
							{
								stream.CheckStillToSet();
								yapClass.SetStateDirty();
								yapClass.Write(trans);
								trans.Commit();
								Db4objects.Db4o.YapWriter returnBytes = stream.ReadWriterByID(trans, yapClass.GetID
									());
								serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(returnBytes
									));
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
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.FAILED);
			return true;
		}
	}
}
