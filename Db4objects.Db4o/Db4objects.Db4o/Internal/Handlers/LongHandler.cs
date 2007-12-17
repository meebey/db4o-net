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

		/// <exception cref="CorruptionException"></exception>
		public override object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return mf._primitive.ReadLong(buffer);
		}

		internal override object Read1(BufferImpl a_bytes)
		{
			return a_bytes.ReadLong();
		}

		public override void Write(object obj, BufferImpl buffer)
		{
			WriteLong(buffer, ((long)obj));
		}

		public static void WriteLong(IWriteBuffer buffer, long val)
		{
			if (Deploy.debug && Deploy.debugLong)
			{
				string l_s = "                                " + val;
				new LatinStringIO().Write(buffer, Sharpen.Runtime.Substring(l_s, l_s.Length - Const4
					.LONG_BYTES));
			}
			else
			{
				for (int i = 0; i < Const4.LONG_BYTES; i++)
				{
					buffer.WriteByte((byte)(val >> ((Const4.LONG_BYTES - 1 - i) * 8)));
				}
			}
		}

		public static long ReadLong(IReadBuffer buffer)
		{
			long ret = 0;
			if (Deploy.debug && Deploy.debugLong)
			{
				ret = long.Parse(new LatinStringIO().Read(buffer, Const4.LONG_BYTES).Trim());
			}
			else
			{
				for (int i = 0; i < Const4.LONG_BYTES; i++)
				{
					ret = (ret << 8) + (buffer.ReadByte() & unchecked((int)(0xff)));
				}
			}
			return ret;
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

		public override object Read(IReadContext context)
		{
			return context.ReadLong();
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteLong(((long)obj));
		}
	}
}
