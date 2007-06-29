/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class DateHandler : LongHandler
	{
		private static readonly Date PROTO = new Date(0);

		public DateHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Handlers4.HandlerCanHold(this, claxx) ? obj : No4.INSTANCE;
		}

		public void CopyValue(object a_from, object a_to)
		{
			try
			{
				((Date)a_to).SetTime(((Date)a_from).GetTime());
			}
			catch (Exception)
			{
			}
		}

		public override object DefaultValue()
		{
			return PROTO;
		}

		public override int GetID()
		{
			return 10;
		}

		protected override Type PrimitiveJavaClass()
		{
			return null;
		}

		public override object PrimitiveNull()
		{
			return null;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect
			)
		{
			return mf._primitive.ReadDate(writer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return PrimitiveMarshaller().ReadDate(a_bytes);
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			if (a_object == null)
			{
				a_object = new Date(0);
			}
			a_bytes.WriteLong(((Date)a_object).GetTime());
		}

		public static string Now()
		{
			return Platform4.Format(new Date(), true);
		}

		internal override long Val(object obj)
		{
			return ((Date)obj).GetTime();
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is Date && Val(obj) == CurrentLong();
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is Date && Val(obj) > CurrentLong();
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is Date && Val(obj) < CurrentLong();
		}
	}
}
