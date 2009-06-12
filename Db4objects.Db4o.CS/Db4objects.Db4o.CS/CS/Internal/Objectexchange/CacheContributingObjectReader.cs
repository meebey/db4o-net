/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.CS.Caching;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Objectexchange;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Objectexchange
{
	public class CacheContributingObjectReader
	{
		private readonly ByteArrayBuffer _reader;

		private readonly ClientTransaction _transaction;

		private readonly IClientSlotCache _slotCache = ((IClientSlotCache)Environments.My
			(typeof(IClientSlotCache)));

		public CacheContributingObjectReader(ClientTransaction transaction, ByteArrayBuffer
			 reader)
		{
			_reader = reader;
			_transaction = transaction;
		}

		public virtual IEnumerator Buffers()
		{
			IDictionary slots = ReadSlots();
			return Iterators.Map(ReadRootIds(), new _IFunction4_27(slots));
		}

		private sealed class _IFunction4_27 : IFunction4
		{
			public _IFunction4_27(IDictionary slots)
			{
				this.slots = slots;
			}

			public object Apply(object arg)
			{
				return ((Pair)slots[((int)arg)]);
			}

			private readonly IDictionary slots;
		}

		public virtual IFixedSizeIntIterator4 Iterator()
		{
			ContributeSlotsToCache();
			return ReadRootIds();
		}

		private IFixedSizeIntIterator4 ReadRootIds()
		{
			int size = _reader.ReadInt();
			return new _FixedSizeIntIterator4Base_44(this, size);
		}

		private sealed class _FixedSizeIntIterator4Base_44 : FixedSizeIntIterator4Base
		{
			public _FixedSizeIntIterator4Base_44(CacheContributingObjectReader _enclosing, int
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override int NextInt()
			{
				return this._enclosing._reader.ReadInt();
			}

			private readonly CacheContributingObjectReader _enclosing;
		}

		private void ContributeSlotsToCache()
		{
			int size = _reader.ReadInt();
			for (int i = 0; i < size; ++i)
			{
				ReadNextSlot();
			}
		}

		private IDictionary ReadSlots()
		{
			IDictionary slots = new Hashtable();
			int size = _reader.ReadInt();
			for (int i = 0; i < size; ++i)
			{
				Pair slot = ReadNextSlot();
				slots[((int)slot.first)] = slot;
			}
			return slots;
		}

		private Pair ReadNextSlot()
		{
			int id = _reader.ReadInt();
			int length = _reader.ReadInt();
			// slot length
			if (length == 0)
			{
				return Pair.Of(id, null);
			}
			ByteArrayBuffer slot = ReadNextSlot(length);
			ContributeToCache(id, slot);
			return Pair.Of(id, slot);
		}

		private void ContributeToCache(int id, ByteArrayBuffer slot)
		{
			_slotCache.Add(_transaction, id, slot);
		}

		private ByteArrayBuffer ReadNextSlot(int length)
		{
			ByteArrayBuffer slot = _reader.ReadPayloadReader(_reader.Offset(), length);
			_reader.Skip(length);
			return slot;
		}
	}
}
