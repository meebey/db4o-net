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
	public class DoubleHandler : LongHandler
	{
		private static readonly double DefaultDoubleValue = System.Convert.ToDouble(0);

		public DoubleHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToDouble(obj);
		}

		public override object DefaultValue()
		{
			return DefaultDoubleValue;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(double);
		}

		public override object PrimitiveNull()
		{
			return DefaultDoubleValue;
		}

		/// <exception cref="CorruptionException"></exception>
		public override object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return mf._primitive.ReadDouble(buffer);
		}

		internal override object Read1(ByteArrayBuffer buffer)
		{
			return PrimitiveMarshaller().ReadDouble(buffer);
		}

		public override void Write(object a_object, ByteArrayBuffer a_bytes)
		{
			a_bytes.WriteLong(Platform4.DoubleToLong(((double)a_object)));
		}

		public override object Read(IReadContext context)
		{
			long l = (long)base.Read(context);
			return Platform4.LongToDouble(l);
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteLong(Platform4.DoubleToLong(((double)obj)));
		}

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			double sourceDouble = ((double)source);
			return new _IPreparedComparison_64(this, sourceDouble);
		}

		private sealed class _IPreparedComparison_64 : IPreparedComparison
		{
			public _IPreparedComparison_64(DoubleHandler _enclosing, double sourceDouble)
			{
				this._enclosing = _enclosing;
				this.sourceDouble = sourceDouble;
			}

			public int CompareTo(object target)
			{
				if (target == null)
				{
					return 1;
				}
				double targetDouble = ((double)target);
				return sourceDouble == targetDouble ? 0 : (sourceDouble < targetDouble ? -1 : 1);
			}

			private readonly DoubleHandler _enclosing;

			private readonly double sourceDouble;
		}
	}
}
