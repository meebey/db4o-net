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
		internal const int Length = Const4.CharBytes + Const4.AddedLength;

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
			return Length;
		}

		protected override Type PrimitiveJavaClass()
		{
			return typeof(char);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(ByteArrayBuffer a_bytes)
		{
			byte b1 = a_bytes.ReadByte();
			byte b2 = a_bytes.ReadByte();
			char ret = (char)((b1 & unchecked((int)(0xff))) | ((b2 & unchecked((int)(0xff))) 
				<< 8));
			return ret;
		}

		public override void Write(object a_object, ByteArrayBuffer a_bytes)
		{
			char char_ = ((char)a_object);
			a_bytes.WriteByte((byte)(char_ & unchecked((int)(0xff))));
			a_bytes.WriteByte((byte)(char_ >> 8));
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
			return new _IPreparedComparison_99(this, sourceChar);
		}

		private sealed class _IPreparedComparison_99 : IPreparedComparison
		{
			public _IPreparedComparison_99(CharHandler _enclosing, char sourceChar)
			{
				this._enclosing = _enclosing;
				this.sourceChar = sourceChar;
			}

			public int CompareTo(object target)
			{
				if (target == null)
				{
					return 1;
				}
				char targetChar = ((char)target);
				return sourceChar == targetChar ? 0 : (sourceChar < targetChar ? -1 : 1);
			}

			private readonly CharHandler _enclosing;

			private readonly char sourceChar;
		}
	}
}
