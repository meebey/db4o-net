/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class SlotFormat
	{
		private static readonly Hashtable4 _versions = new Hashtable4();

		private static readonly SlotFormat CurrentSlotFormat = new SlotFormatCurrent();

		static SlotFormat()
		{
			new SlotFormat0();
			new SlotFormat2();
		}

		protected SlotFormat()
		{
			_versions.Put(HandlerVersion(), this);
		}

		public static SlotFormat ForHandlerVersion(int handlerVersion)
		{
			if (handlerVersion == HandlerRegistry.HandlerVersion)
			{
				return CurrentSlotFormat;
			}
			if (handlerVersion < 0 || handlerVersion > CurrentSlotFormat.HandlerVersion())
			{
				throw new ArgumentException();
			}
			SlotFormat slotFormat = (SlotFormat)_versions.Get(handlerVersion);
			if (slotFormat != null)
			{
				return slotFormat;
			}
			return ForHandlerVersion(handlerVersion + 1);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SlotFormat))
			{
				return false;
			}
			return HandlerVersion() == ((SlotFormat)obj).HandlerVersion();
		}

		public override int GetHashCode()
		{
			return HandlerVersion();
		}

		protected abstract int HandlerVersion();

		public abstract bool IsIndirectedWithinSlot(ITypeHandler4 handler);

		public static SlotFormat Current()
		{
			return CurrentSlotFormat;
		}

		protected virtual bool IsVariableLength(ITypeHandler4 handler)
		{
			return handler is IVariableLengthTypeHandler;
		}

		public static bool IsEmbedded(ITypeHandler4 handler)
		{
			return handler is IEmbeddedTypeHandler;
		}

		public virtual object DoWithSlotIndirection(IReadBuffer buffer, ITypeHandler4 typeHandler
			, IClosure4 closure)
		{
			if (!IsIndirectedWithinSlot(typeHandler))
			{
				return closure.Run();
			}
			return DoWithSlotIndirection(buffer, closure);
		}

		public virtual object DoWithSlotIndirection(IReadBuffer buffer, IClosure4 closure
			)
		{
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

		public virtual bool HandleAsObject(ITypeHandler4 typeHandler)
		{
			if (IsEmbedded(typeHandler))
			{
				return false;
			}
			if (typeHandler is UntypedFieldHandler)
			{
				return false;
			}
			return true;
		}

		public virtual void WriteObjectClassID(ByteArrayBuffer buffer, int id)
		{
			buffer.WriteInt(-id);
		}

		public virtual void SkipMarshallerInfo(ByteArrayBuffer reader)
		{
			reader.IncrementOffset(1);
		}

		public virtual ObjectHeaderAttributes ReadHeaderAttributes(ByteArrayBuffer reader
			)
		{
			return new ObjectHeaderAttributes(reader);
		}
	}
}
