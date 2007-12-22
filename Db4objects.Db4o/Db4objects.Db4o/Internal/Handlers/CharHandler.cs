/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class CharHandler : PrimitiveHandler
	{
		internal const int LENGTH = Const4.CHAR_BYTES + Const4.ADDED_LENGTH;

		private static readonly char i_primitive = (char)0;

		public CharHandler(ObjectContainerBase stream) : base(stream)
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
			return typeof(char);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(BufferImpl a_bytes)
		{
			byte b1 = a_bytes.ReadByte();
			byte b2 = a_bytes.ReadByte();
			char ret = (char)((b1 & unchecked((int)(0xff))) | ((b2 & unchecked((int)(0xff))) 
				<< 8));
			return ret;
		}

		public override void Write(object a_object, BufferImpl a_bytes)
		{
			char char_ = ((char)a_object);
			a_bytes.WriteByte((byte)(char_ & unchecked((int)(0xff))));
			a_bytes.WriteByte((byte)(char_ >> 8));
		}

		private char i_compareTo;

		private char Val(object obj)
		{
			return ((char)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Val(obj);
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is char && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is char && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is char && Val(obj) < i_compareTo;
		}

		public override object Read(IReadContext context)
		{
			byte b1 = context.ReadByte();
			byte b2 = context.ReadByte();
			char charValue = (char)((b1 & unchecked((int)(0xff))) | ((b2 & unchecked((int)(0xff
				))) << 8));
			return charValue;
		}

		public override void Write(IWriteContext context, object obj)
		{
			char charValue = ((char)obj);
			context.WriteBytes(new byte[] { (byte)(charValue & unchecked((int)(0xff))), (byte
				)(charValue >> 8) });
		}

		public override IPreparedComparison InternalPrepareComparison(object source)
		{
			char sourceChar = ((char)source);
			return new _IPreparedComparison_123(this, sourceChar);
		}

		private sealed class _IPreparedComparison_123 : IPreparedComparison
		{
			public _IPreparedComparison_123(CharHandler _enclosing, char sourceChar)
			{
				this._enclosing = _enclosing;
				this.sourceChar = sourceChar;
			}

			public int CompareTo(object target)
			{
				char targetChar = ((char)target);
				return sourceChar == targetChar ? 0 : (sourceChar < targetChar ? -1 : 1);
			}

			private readonly CharHandler _enclosing;

			private readonly char sourceChar;
		}
	}
}
