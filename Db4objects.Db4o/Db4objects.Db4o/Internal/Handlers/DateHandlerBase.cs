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
			return Handlers4.HandlerCanHold(this, claxx) ? obj : No4.Instance;
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

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			long sourceDate = Sharpen.Runtime.ToJavaMilliseconds(((DateTime)source));
			return new _IPreparedComparison_70(this, sourceDate);
		}

		private sealed class _IPreparedComparison_70 : IPreparedComparison
		{
			public _IPreparedComparison_70(DateHandlerBase _enclosing, long sourceDate)
			{
				this._enclosing = _enclosing;
				this.sourceDate = sourceDate;
			}

			public int CompareTo(object target)
			{
				if (target == null)
				{
					return 1;
				}
				long targetDate = Sharpen.Runtime.ToJavaMilliseconds(((DateTime)target));
				return sourceDate == targetDate ? 0 : (sourceDate < targetDate ? -1 : 1);
			}

			private readonly DateHandlerBase _enclosing;

			private readonly long sourceDate;
		}
	}
}
