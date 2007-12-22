/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public sealed class BooleanHandler : PrimitiveHandler
	{
		internal const int LENGTH = 1 + Const4.ADDED_LENGTH;

		private const byte TRUE = (byte)'T';

		private const byte FALSE = (byte)'F';

		private const byte NULL = (byte)'N';

		private static readonly bool i_primitive = false;

		public BooleanHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int LinkLength()
		{
			return LENGTH;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(bool);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(BufferImpl a_bytes)
		{
			byte ret = a_bytes.ReadByte();
			if (ret == TRUE)
			{
				return true;
			}
			if (ret == FALSE)
			{
				return false;
			}
			return null;
		}

		public override void Write(object obj, BufferImpl buffer)
		{
			buffer.WriteByte(GetEncodedByteValue(obj));
		}

		private byte GetEncodedByteValue(object obj)
		{
			if (obj == null)
			{
				return NULL;
			}
			if (((bool)obj))
			{
				return TRUE;
			}
			return FALSE;
		}

		private bool i_compareTo;

		private bool Val(object obj)
		{
			return ((bool)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Val(obj);
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is bool && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			if (i_compareTo)
			{
				return false;
			}
			return obj is bool && Val(obj);
		}

		internal override bool IsSmaller1(object obj)
		{
			if (!i_compareTo)
			{
				return false;
			}
			return obj is bool && !Val(obj);
		}

		public override object Read(IReadContext context)
		{
			byte ret = context.ReadByte();
			if (ret == TRUE)
			{
				return true;
			}
			if (ret == FALSE)
			{
				return false;
			}
			return null;
		}

		public override void Write(IWriteContext context, object obj)
		{
			context.WriteByte(GetEncodedByteValue(obj));
		}

		public override object NullRepresentationInUntypedArrays()
		{
			return null;
		}

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			bool sourceBoolean = ((bool)source);
			return new _IPreparedComparison_151(this, sourceBoolean);
		}

		private sealed class _IPreparedComparison_151 : IPreparedComparison
		{
			public _IPreparedComparison_151(BooleanHandler _enclosing, bool sourceBoolean)
			{
				this._enclosing = _enclosing;
				this.sourceBoolean = sourceBoolean;
			}

			public int CompareTo(object target)
			{
				bool targetBoolean = ((bool)target);
				return sourceBoolean == targetBoolean ? 0 : (sourceBoolean ? 1 : -1);
			}

			private readonly BooleanHandler _enclosing;

			private readonly bool sourceBoolean;
		}
	}
}
