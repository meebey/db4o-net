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
	public class IntHandler : PrimitiveHandler
	{
		private static readonly int i_primitive = 0;

		public IntHandler(ObjectContainerBase container) : base(container)
		{
		}

		public override object Coerce(IReflectClass claxx, object obj)
		{
			return Coercion4.ToInt(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(int);
		}

		public override int LinkLength()
		{
			return Const4.INT_LENGTH;
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		/// <exception cref="CorruptionException"></exception>
		public override object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect
			)
		{
			return mf._primitive.ReadInteger(writer);
		}

		internal override object Read1(BufferImpl a_bytes)
		{
			return a_bytes.ReadInt();
		}

		public override void Write(object obj, BufferImpl writer)
		{
			Write(((int)obj), writer);
		}

		public virtual void Write(int intValue, BufferImpl writer)
		{
			WriteInt(intValue, writer);
		}

		public static void WriteInt(int a_int, BufferImpl a_bytes)
		{
			a_bytes.WriteInt(a_int);
		}

		private int i_compareTo;

		protected int Val(object obj)
		{
			return ((int)obj);
		}

		public virtual int CompareTo(int other)
		{
			return other - i_compareTo;
		}

		public virtual void PrepareComparison(int i)
		{
			i_compareTo = i;
		}

		internal override void PrepareComparison1(object obj)
		{
			PrepareComparison(Val(obj));
		}

		public virtual int CurrentInt()
		{
			return i_compareTo;
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is int && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is int && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is int && Val(obj) < i_compareTo;
		}

		public override void DefragIndexEntry(DefragmentContextImpl context)
		{
			context.IncrementIntSize();
		}

		public override object Read(IReadContext context)
		{
			return context.ReadInt();
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteInt(((int)obj));
		}

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			return NewPrepareCompare(((int)source));
		}

		public virtual IPreparedComparison NewPrepareCompare(int i)
		{
			return new IntHandler.PreparedIntComparison(this, i);
		}

		public sealed class PreparedIntComparison : IPreparedComparison
		{
			private readonly int _sourceInt;

			public PreparedIntComparison(IntHandler _enclosing, int sourceInt)
			{
				this._enclosing = _enclosing;
				this._sourceInt = sourceInt;
			}

			public int CompareTo(object target)
			{
				int targetInt = ((int)target);
				return this._sourceInt == targetInt ? 0 : (this._sourceInt < targetInt ? -1 : 1);
			}

			private readonly IntHandler _enclosing;
		}
	}
}
