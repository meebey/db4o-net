/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Caching;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Objectexchange;

namespace Db4objects.Db4o.Internal.CS.Objectexchange
{
	public class EagerObjectReader
	{
		private readonly ByteArrayBuffer _reader;

		private readonly ClientTransaction _transaction;

		public EagerObjectReader(ClientTransaction transaction, ByteArrayBuffer reader)
		{
			_reader = reader;
			_transaction = transaction;
		}

		public virtual IFixedSizeIntIterator4 Iterator()
		{
			ReadChildSlots();
			return IterateRootSlots();
		}

		private IFixedSizeIntIterator4 IterateRootSlots()
		{
			int size = _reader.ReadInt();
			return new _FixedSizeIntIterator4Base_29(this, size);
		}

		private sealed class _FixedSizeIntIterator4Base_29 : FixedSizeIntIterator4Base
		{
			public _FixedSizeIntIterator4Base_29(EagerObjectReader _enclosing, int baseArg1) : 
				base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override int NextInt()
			{
				return this._enclosing.ReadNext();
			}

			private readonly EagerObjectReader _enclosing;
		}

		private void ReadChildSlots()
		{
			int childSlots = _reader.ReadInt();
			for (int i = 0; i < childSlots; ++i)
			{
				ReadNext();
			}
		}

		protected virtual void ContributeCachedSlot(int id, int length)
		{
			ByteArrayBuffer slot = _reader.ReadPayloadReader(_reader.Offset(), length);
			((IClientSlotCache)Environments.My(typeof(IClientSlotCache))).Add(_transaction, id
				, slot);
			_reader.Skip(length);
		}

		private int ReadNext()
		{
			int id = _reader.ReadInt();
			int length = _reader.ReadInt();
			// slot length
			if (length > 0)
			{
				ContributeCachedSlot(id, length);
			}
			return id;
		}
	}
}
