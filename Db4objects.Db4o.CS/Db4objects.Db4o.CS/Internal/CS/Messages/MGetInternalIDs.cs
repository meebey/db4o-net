/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.CS.Objectexchange;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetInternalIDs : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			ByteArrayBuffer bytes = GetByteLoad();
			int classMetadataID = bytes.ReadInt();
			int prefetchDepth = bytes.ReadInt();
			int prefetchCount = bytes.ReadInt();
			long[] ids = IdsFor(classMetadataID);
			ByteArrayBuffer payload = ObjectExchangeStrategyFactory.ForConfig(new ObjectExchangeConfiguration
				(prefetchDepth, prefetchCount)).Marshall((LocalTransaction)Transaction(), IntIterators
				.ForLongs(ids), ids.Length);
			MsgD message = Msg.IdList.GetWriterForLength(Transaction(), payload.Length());
			message.PayLoad().WriteBytes(payload._buffer);
			Write(message);
			return true;
		}

		private long[] IdsFor(int classMetadataID)
		{
			lock (StreamLock())
			{
				try
				{
					return Stream().ClassMetadataForID(classMetadataID).GetIDs(Transaction());
				}
				catch (Exception)
				{
				}
			}
			return new long[0];
		}
	}
}
