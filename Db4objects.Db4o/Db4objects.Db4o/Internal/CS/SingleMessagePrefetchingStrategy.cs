using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS
{
	/// <summary>Prefetchs multiples objects at once (in a single message).</summary>
	/// <remarks>Prefetchs multiples objects at once (in a single message).</remarks>
	/// <exclude></exclude>
	public class SingleMessagePrefetchingStrategy : IPrefetchingStrategy
	{
		public static readonly IPrefetchingStrategy INSTANCE = new Db4objects.Db4o.Internal.CS.SingleMessagePrefetchingStrategy
			();

		private SingleMessagePrefetchingStrategy()
		{
		}

		public virtual int PrefetchObjects(ClientObjectContainer container, IIntIterator4
			 ids, object[] prefetched, int prefetchCount)
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
				MsgD msg = Msg.READ_MULTIPLE_OBJECTS.GetWriterForIntArray(container.GetTransaction
					(), idsToGet, toGet);
				container.WriteMsg(msg, true);
				MsgD response = (MsgD)container.ExpectedResponse(Msg.READ_MULTIPLE_OBJECTS);
				int embeddedMessageCount = response.ReadInt();
				for (int i = 0; i < embeddedMessageCount; i++)
				{
					MsgObject mso = (MsgObject)Msg.OBJECT_TO_CLIENT.PublicClone();
					mso.SetTransaction(container.GetTransaction());
					mso.PayLoad(response.PayLoad().ReadYapBytes());
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Const4.MESSAGE_LENGTH);
						StatefulBuffer reader = mso.Unmarshall(Const4.MESSAGE_LENGTH);
						object obj = container.ObjectForIdFromCache(idsToGet[i]);
						if (obj != null)
						{
							prefetched[position[i]] = obj;
						}
						else
						{
							prefetched[position[i]] = new ObjectReference(idsToGet[i]).ReadPrefetch(container
								, reader);
						}
					}
				}
			}
			return count;
		}
	}
}
