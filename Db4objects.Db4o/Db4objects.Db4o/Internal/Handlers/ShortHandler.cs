/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class ShortHandler : PrimitiveHandler
	{
		internal const int Length = Const4.ShortBytes + Const4.AddedLength;

		private static readonly short i_primitive = (short)0;

		public ShortHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToShort(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int LinkLength()
		{
			return Length;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(short);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		/// <exception cref="CorruptionException"></exception>
		public override object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return mf._primitive.ReadShort(buffer);
		}

		internal override object Read1(BufferImpl buffer)
		{
			return PrimitiveMarshaller().ReadShort(buffer);
		}

		public override void Write(object a_object, BufferImpl a_bytes)
		{
			WriteShort(((short)a_object), a_bytes);
		}

		internal static void WriteShort(int a_short, BufferImpl a_bytes)
		{
			for (int i = 0; i < Const4.ShortBytes; i++)
			{
				a_bytes._buffer[a_bytes._offset++] = (byte)(a_short >> ((Const4.ShortBytes - 1 - 
					i) * 8));
			}
		}

		public override object Read(IReadContext context)
		{
			int value = ((context.ReadByte() & unchecked((int)(0xff))) << 8) + (context.ReadByte
				() & unchecked((int)(0xff)));
			return (short)value;
		}

		public override void Write(IWriteContext context, object obj)
		{
			int shortValue = ((short)obj);
			context.WriteBytes(new byte[] { (byte)(shortValue >> 8), (byte)shortValue });
		}

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			short sourceShort = ((short)source);
			return new _IPreparedComparison_95(this, sourceShort);
		}

		private sealed class _IPreparedComparison_95 : IPreparedComparison
		{
			public _IPreparedComparison_95(ShortHandler _enclosing, short sourceShort)
			{
				this._enclosing = _enclosing;
				this.sourceShort = sourceShort;
			}

			public int CompareTo(object target)
			{
				if (target == null)
				{
					return 1;
				}
				short targetShort = ((short)target);
				return sourceShort == targetShort ? 0 : (sourceShort < targetShort ? -1 : 1);
			}

			private readonly ShortHandler _enclosing;

			private readonly short sourceShort;
		}
	}
}
