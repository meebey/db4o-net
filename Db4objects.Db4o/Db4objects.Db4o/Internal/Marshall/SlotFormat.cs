/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class SlotFormat
	{
		private static readonly Hashtable4 _versions = new Hashtable4();

		private static readonly Db4objects.Db4o.Internal.Marshall.SlotFormat CurrentSlotFormat
			 = new SlotFormatCurrent();

		static SlotFormat()
		{
			new SlotFormat0();
			new SlotFormat2();
		}

		protected SlotFormat()
		{
			_versions.Put(HandlerVersion(), this);
		}

		public static Db4objects.Db4o.Internal.Marshall.SlotFormat ForHandlerVersion(int 
			handlerVersion)
		{
			if (handlerVersion == HandlerRegistry.HandlerVersion)
			{
				return CurrentSlotFormat;
			}
			if (handlerVersion < 0 || handlerVersion > CurrentSlotFormat.HandlerVersion())
			{
				throw new ArgumentException();
			}
			Db4objects.Db4o.Internal.Marshall.SlotFormat slotFormat = (Db4objects.Db4o.Internal.Marshall.SlotFormat
				)_versions.Get(handlerVersion);
			if (slotFormat != null)
			{
				return slotFormat;
			}
			return ForHandlerVersion(handlerVersion + 1);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Db4o.Internal.Marshall.SlotFormat))
			{
				return false;
			}
			return HandlerVersion() == ((Db4objects.Db4o.Internal.Marshall.SlotFormat)obj).HandlerVersion
				();
		}

		public override int GetHashCode()
		{
			return HandlerVersion();
		}

		protected abstract int HandlerVersion();

		public abstract bool IsIndirectedWithinSlot(ITypeHandler4 handler);

		public static Db4objects.Db4o.Internal.Marshall.SlotFormat Current()
		{
			return CurrentSlotFormat;
		}

		protected virtual bool IsVariableLength(ITypeHandler4 handler)
		{
			return handler is IVariableLengthTypeHandler;
		}

		public virtual object DoWithSlotIndirection(IReadBuffer buffer, ITypeHandler4 typeHandler
			, IClosure4 closure)
		{
			if (!IsIndirectedWithinSlot(typeHandler))
			{
				return closure.Run();
			}
			int payLoadOffset = buffer.ReadInt();
			buffer.ReadInt();
			// length, not used
			int savedOffset = buffer.Offset();
			object res = null;
			if (payLoadOffset != 0)
			{
				buffer.Seek(payLoadOffset);
				res = closure.Run();
			}
			buffer.Seek(savedOffset);
			return res;
		}
	}
}
