/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.CS.Objectexchange;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MReadMultipleObjects : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			int prefetchDepth = ReadInt();
			int prefetchCount = ReadInt();
			IIntIterator4 ids = new _FixedSizeIntIterator4Base_14(this, prefetchCount);
			IObjectExchangeStrategy strategy = ObjectExchangeStrategyFactory.ForConfig(new ObjectExchangeConfiguration
				(prefetchDepth, prefetchCount));
			ByteArrayBuffer buffer = strategy.Marshall((LocalTransaction)Transaction(), ids, 
				prefetchCount);
			MsgD msg = Msg.ReadMultipleObjects.GetWriterForBuffer(Transaction(), buffer);
			Write(msg);
			return true;
		}

		private sealed class _FixedSizeIntIterator4Base_14 : FixedSizeIntIterator4Base
		{
			public _FixedSizeIntIterator4Base_14(MReadMultipleObjects _enclosing, int baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override int NextInt()
			{
				return this._enclosing.ReadInt();
			}

			private readonly MReadMultipleObjects _enclosing;
		}
	}
}
