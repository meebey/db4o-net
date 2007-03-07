namespace Db4objects.Db4o.Internal.CS
{
	/// <summary>Prefetchs multiples objects at once (in a single message).</summary>
	/// <remarks>Prefetchs multiples objects at once (in a single message).</remarks>
	/// <exclude></exclude>
	public class SingleMessagePrefetchingStrategy : Db4objects.Db4o.Internal.CS.IPrefetchingStrategy
	{
		public static readonly Db4objects.Db4o.Internal.CS.IPrefetchingStrategy INSTANCE = 
			new Db4objects.Db4o.Internal.CS.SingleMessagePrefetchingStrategy();

		private SingleMessagePrefetchingStrategy()
		{
		}

		public virtual int PrefetchObjects(Db4objects.Db4o.Internal.CS.ClientObjectContainer
			 container, Db4objects.Db4o.Foundation.IIntIterator4 ids, object[] prefetched, int
			 prefetchCount)
		{
			int count = 0;
			int toGet = 0;
			int[] idsToGet = new int[prefetchCount];
			int[] position = new int[prefetchCount];
			while (count < prefetchCount)
			{
				if (!ids.MoveNext())
				{
					break;
				}
				int id = ids.CurrentInt();
				if (id > 0)
				{
					object obj = container.ObjectForIdFromCache(id);
					if (obj != null)
					{
						prefetched[count] = obj;
					}
					else
					{
						idsToGet[toGet] = id;
						position[toGet] = count;
						toGet++;
					}
					count++;
				}
			}
			if (toGet > 0)
			{
				Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.READ_MULTIPLE_OBJECTS.GetWriterForIntArray(container.GetTransaction(), idsToGet
					, toGet);
				container.WriteMsg(msg, true);
				Db4objects.Db4o.Internal.CS.Messages.MsgD message = (Db4objects.Db4o.Internal.CS.Messages.MsgD
					)container.ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.READ_MULTIPLE_OBJECTS
					);
				int embeddedMessageCount = message.ReadInt();
				for (int i = 0; i < embeddedMessageCount; i++)
				{
					Db4objects.Db4o.Internal.CS.Messages.MsgObject mso = (Db4objects.Db4o.Internal.CS.Messages.MsgObject
						)Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT.Clone(container.GetTransaction
						());
					mso.PayLoad(message.PayLoad().ReadYapBytes());
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Db4objects.Db4o.Internal.Const4.MESSAGE_LENGTH);
						Db4objects.Db4o.Internal.StatefulBuffer reader = mso.Unmarshall(Db4objects.Db4o.Internal.Const4
							.MESSAGE_LENGTH);
						object obj = container.ObjectForIdFromCache(idsToGet[i]);
						if (obj != null)
						{
							prefetched[position[i]] = obj;
						}
						else
						{
							prefetched[position[i]] = new Db4objects.Db4o.Internal.ObjectReference(idsToGet[i
								]).ReadPrefetch(container, reader);
						}
					}
				}
			}
			return count;
		}
	}
}
