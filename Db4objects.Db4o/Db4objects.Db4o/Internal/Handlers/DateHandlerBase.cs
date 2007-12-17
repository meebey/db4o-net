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
	/// <summary>Shared (java/.net) logic for Date handling.</summary>
	/// <remarks>Shared (java/.net) logic for Date handling.</remarks>
	public abstract class DateHandlerBase : LongHandler
	{
		public DateHandlerBase(ObjectContainerBase container) : base(container)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Handlers4.HandlerCanHold(this, claxx) ? obj : No4.INSTANCE;
		}

		public abstract object CopyValue(object from, object to);

		public abstract override object DefaultValue();

		public abstract override object PrimitiveNull();

		public abstract override object NullRepresentationInUntypedArrays();

		protected override Type PrimitiveJavaClass()
		{
			return null;
		}

		/// <exception cref="CorruptionException"></exception>
		public override object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect
			)
		{
			return mf._primitive.ReadDate(writer);
		}

		internal override object Read1(BufferImpl a_bytes)
		{
			return PrimitiveMarshaller().ReadDate(a_bytes);
		}

		public override void Write(object a_object, BufferImpl a_bytes)
		{
			if (a_object == null)
			{
				a_object = new DateTime(0);
			}
			a_bytes.WriteLong(Sharpen.Runtime.ToJavaMilliseconds(((DateTime)a_object)));
		}

		public static string Now()
		{
			return Platform4.Format(Platform4.Now(), true);
		}

		internal override long Val(object obj)
		{
			return Sharpen.Runtime.ToJavaMilliseconds(((DateTime)obj));
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is DateTime && Val(obj) == CurrentLong();
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is DateTime && Val(obj) > CurrentLong();
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is DateTime && Val(obj) < CurrentLong();
		}

		public override object Read(IReadContext context)
		{
			long milliseconds = ((long)base.Read(context));
			return new DateTime(milliseconds);
		}

		public override void Write(IWriteContext context, object obj)
		{
			long milliseconds = Sharpen.Runtime.ToJavaMilliseconds(((DateTime)obj));
			base.Write(context, milliseconds);
		}
	}
}
