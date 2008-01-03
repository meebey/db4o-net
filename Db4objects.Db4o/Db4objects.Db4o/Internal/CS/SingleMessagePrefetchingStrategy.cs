/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
		public static readonly IPrefetchingStrategy Instance = new Db4objects.Db4o.Internal.CS.SingleMessagePrefetchingStrategy
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
					object obj = container.Transaction().ObjectForIdFromCache(id);
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
				Transaction trans = container.Transaction();
				MsgD msg = Msg.ReadMultipleObjects.GetWriterForIntArray(trans, idsToGet, toGet);
				container.Write(msg);
				MsgD response = (MsgD)container.ExpectedResponse(Msg.ReadMultipleObjects);
				int embeddedMessageCount = response.ReadInt();
				for (int i = 0; i < embeddedMessageCount; i++)
				{
					MsgObject mso = (MsgObject)Msg.ObjectToClient.PublicClone();
					mso.SetTransaction(trans);
					mso.PayLoad(response.PayLoad().ReadYapBytes());
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Const4.MessageLength);
						StatefulBuffer reader = mso.Unmarshall(Const4.MessageLength);
						object obj = trans.ObjectForIdFromCache(idsToGet[i]);
						if (obj != null)
						{
							prefetched[position[i]] = obj;
						}
						else
						{
							prefetched[position[i]] = new ObjectReference(idsToGet[i]).ReadPrefetch(trans, reader
								);
						}
					}
				}
			}
			return count;
		}
	}
}
