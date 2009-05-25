/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Objectexchange;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.CS.Objectexchange
{
	public class EagerObjectWriter
	{
		private LocalTransaction _transaction;

		private ObjectExchangeConfiguration _config;

		public EagerObjectWriter(ObjectExchangeConfiguration config, LocalTransaction transaction
			)
		{
			_config = config;
			_transaction = transaction;
		}

		/// <summary>
		/// options:
		/// (1) precalculate complete buffer size by iterating through slots
		/// (1') keep the slots in a arraylist first to avoid IO overhead
		/// (2) resize the buffer as needed
		/// (2') allow garbage to be transmitted to avoid reallocation of the complete buffer
		/// (3) stream directly to the output socket
		/// </summary>
		/// <returns></returns>
		public virtual ByteArrayBuffer Write(IIntIterator4 idIterator, int maxCount)
		{
			IList rootSlots = ReadSlots(idIterator, maxCount);
			IList childSlots = ChildSlotsFor(rootSlots);
			int marshalledSize = MarshalledSizeFor(rootSlots) + MarshalledSizeFor(childSlots);
			ByteArrayBuffer buffer = new ByteArrayBuffer(marshalledSize);
			WriteIdSlotPairsTo(childSlots, buffer);
			WriteIdSlotPairsTo(rootSlots, buffer);
			return buffer;
		}

		private IList ChildSlotsFor(IList slots)
		{
			ArrayList result = new ArrayList();
			if (_config.prefetchDepth < 2)
			{
				return result;
			}
			for (IEnumerator pairIter = slots.GetEnumerator(); pairIter.MoveNext(); )
			{
				Pair pair = ((Pair)pairIter.Current);
				Slot slot = ((Slot)pair.second);
				if (slot == null)
				{
					break;
				}
				int id = (((int)pair.first));
				IEnumerator childIds = CollectChildIdsFor(id);
				while (childIds.MoveNext())
				{
					int childId = (int)childIds.Current;
					result.Add(IdSlotPairFor(childId));
				}
			}
			return result;
		}

		private IEnumerator CollectChildIdsFor(int id)
		{
			CollectIdContext context = CollectIdContext.ForID(_transaction, id);
			ClassMetadata classMetadata = context.ClassMetadata();
			if (null == classMetadata)
			{
				// most probably ClassMetadata reading
				return Iterators.EmptyIterator;
			}
			if (classMetadata.IsPrimitive())
			{
				throw new InvalidOperationException(classMetadata.ToString());
			}
			if (!Handlers4.IsCascading(classMetadata.TypeHandler()))
			{
				return Iterators.EmptyIterator;
			}
			classMetadata.CollectIDs(context);
			return new TreeKeyIterator(context.Ids());
		}

		private void WriteIdSlotPairsTo(IList slots, ByteArrayBuffer buffer)
		{
			buffer.WriteInt(slots.Count);
			for (IEnumerator idSlotPairIter = slots.GetEnumerator(); idSlotPairIter.MoveNext(
				); )
			{
				Pair idSlotPair = ((Pair)idSlotPairIter.Current);
				int id = (((int)idSlotPair.first));
				Slot slot = ((Slot)idSlotPair.second);
				if (slot == null || slot.IsNull())
				{
					buffer.WriteInt(id);
					buffer.WriteInt(0);
					continue;
				}
				ByteArrayBuffer slotBuffer = _transaction.File().ReadSlotBuffer(slot);
				buffer.WriteInt(id);
				buffer.WriteInt(slot.Length());
				buffer.WriteBytes(slotBuffer._buffer);
			}
		}

		private int MarshalledSizeFor(IList slots)
		{
			int total = Const4.IntLength;
			// count
			for (IEnumerator idSlotPairIter = slots.GetEnumerator(); idSlotPairIter.MoveNext(
				); )
			{
				Pair idSlotPair = ((Pair)idSlotPairIter.Current);
				total += Const4.IntLength;
				// id
				total += Const4.IntLength;
				// length
				Slot slot = ((Slot)idSlotPair.second);
				if (slot != null)
				{
					total += slot.Length();
				}
			}
			return total;
		}

		private IList ReadSlots(IIntIterator4 idIterator, int maxCount)
		{
			int prefetchObjectCount = ConfiguredPrefetchObjectCount();
			ArrayList slots = new ArrayList();
			while (idIterator.MoveNext())
			{
				int id = idIterator.CurrentInt();
				if (slots.Count < prefetchObjectCount)
				{
					slots.Add(IdSlotPairFor(id));
				}
				else
				{
					slots.Add(Pair.Of(id, null));
				}
				if (slots.Count >= maxCount)
				{
					break;
				}
			}
			return slots;
		}

		private Pair IdSlotPairFor(int id)
		{
			Slot slot = _transaction.GetCurrentSlotOfID(id);
			Pair pair = Pair.Of(id, slot);
			return pair;
		}

		private int ConfiguredPrefetchObjectCount()
		{
			return _config.prefetchCount;
		}
	}
}
