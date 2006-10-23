namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MCreateClass : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			Db4objects.Db4o.Transaction trans = stream.GetSystemTransaction();
			Db4objects.Db4o.YapWriter returnBytes = new Db4objects.Db4o.YapWriter(trans, 0);
			try
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = trans.Reflector().ForName(ReadString
					());
				if (claxx != null)
				{
					lock (stream.i_lock)
					{
						try
						{
							Db4objects.Db4o.YapClass yapClass = stream.GetYapClass(claxx, true);
							if (yapClass != null)
							{
								stream.CheckStillToSet();
								yapClass.SetStateDirty();
								yapClass.Write(trans);
								trans.Commit();
								returnBytes = stream.ReadWriterByID(trans, yapClass.GetID());
								Db4objects.Db4o.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(returnBytes).Write(stream
									, sock);
								return true;
							}
						}
						catch (System.Exception t)
						{
						}
					}
				}
			}
			catch (System.Exception th)
			{
			}
			Db4objects.Db4o.CS.Messages.Msg.FAILED.Write(stream, sock);
			return true;
		}
	}
}
