/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class LongHandler : PrimitiveHandler
	{
		private static readonly long i_primitive = System.Convert.ToInt64(0);

		public LongHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToLong(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 2;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(long);
		}

		public override int LinkLength()
		{
			return Const4.LONG_LENGTH;
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return mf._primitive.ReadLong(buffer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return a_bytes.ReadLong();
		}

		public override void Write(object obj, Db4objects.Db4o.Internal.Buffer buffer)
		{
			WriteLong(((long)obj), buffer);
		}

		public static void WriteLong(long val, Db4objects.Db4o.Internal.Buffer bytes)
		{
			PrimitiveCodec.WriteLong(bytes._buffer, bytes._offset, val);
			IncrementOffset(bytes);
		}

		private static void IncrementOffset(Db4objects.Db4o.Internal.Buffer buffer)
		{
			buffer.IncrementOffset(Const4.LONG_BYTES);
		}

		private long i_compareTo;

		protected long CurrentLong()
		{
			return i_compareTo;
		}

		internal virtual long Val(object obj)
		{
			return ((long)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Val(obj);
		}

		public override object Current1()
		{
			return i_compareTo;
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is long && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is long && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is long && Val(obj) < i_compareTo;
		}
	}
}
