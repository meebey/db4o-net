/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class ByteHandler : PrimitiveHandler
	{
		internal const int Length = 1 + Const4.AddedLength;

		private static readonly byte DefaultByteValue = (byte)0;

		public ByteHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToSByte(obj);
		}

		public override object DefaultValue()
		{
			return DefaultByteValue;
		}

		public override int LinkLength()
		{
			return Length;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(byte);
		}

		public override object PrimitiveNull()
		{
			return DefaultByteValue;
		}

		internal override object Read1(ByteArrayBuffer a_bytes)
		{
			byte ret = a_bytes.ReadByte();
			return ret;
		}

		public override void Write(object a_object, ByteArrayBuffer a_bytes)
		{
			a_bytes.WriteByte(((byte)a_object));
		}

		public override object Read(IReadContext context)
		{
			byte byteValue = context.ReadByte();
			return byteValue;
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteByte(((byte)obj));
		}

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			byte sourceByte = ((byte)source);
			return new _IPreparedComparison_91(this, sourceByte);
		}

		private sealed class _IPreparedComparison_91 : IPreparedComparison
		{
			public _IPreparedComparison_91(ByteHandler _enclosing, byte sourceByte)
			{
				this._enclosing = _enclosing;
				this.sourceByte = sourceByte;
			}

			public int CompareTo(object target)
			{
				if (target == null)
				{
					return 1;
				}
				byte targetByte = ((byte)target);
				return sourceByte == targetByte ? 0 : (sourceByte < targetByte ? -1 : 1);
			}

			private readonly ByteHandler _enclosing;

			private readonly byte sourceByte;
		}
	}
}
