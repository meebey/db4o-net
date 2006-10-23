namespace Db4objects.Db4o
{
	internal sealed class YChar : Db4objects.Db4o.YapJavaClass
	{
		internal const int LENGTH = Db4objects.Db4o.YapConst.CHAR_BYTES + Db4objects.Db4o.YapConst
			.ADDED_LENGTH;

		private static readonly char i_primitive = (char)0;

		public YChar(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 7;
		}

		public override int LinkLength()
		{
			return LENGTH;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(char);
		}

		internal override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.YapReader a_bytes)
		{
			byte b1 = a_bytes.ReadByte();
			byte b2 = a_bytes.ReadByte();
			char ret = (char)((b1 & unchecked((int)(0xff))) | ((b2 & unchecked((int)(0xff))) 
				<< 8));
			return ret;
		}

		public override void Write(object a_object, Db4objects.Db4o.YapReader a_bytes)
		{
			char char_ = ((char)a_object);
			a_bytes.Append((byte)(char_ & unchecked((int)(0xff))));
			a_bytes.Append((byte)(char_ >> 8));
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

		public override object Current1()
		{
			return i_compareTo;
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
	}
}