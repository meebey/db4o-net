namespace Db4objects.Db4o
{
	public sealed class YBoolean : Db4objects.Db4o.YapJavaClass
	{
		internal const int LENGTH = 1 + Db4objects.Db4o.YapConst.ADDED_LENGTH;

		private const byte TRUE = (byte)'T';

		private const byte FALSE = (byte)'F';

		private static readonly bool i_primitive = false;

		public YBoolean(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		public override int GetID()
		{
			return 4;
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int LinkLength()
		{
			return LENGTH;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(bool);
		}

		internal override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.YapReader a_bytes)
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

		public override void Write(object a_object, Db4objects.Db4o.YapReader a_bytes)
		{
			byte set;
			if (((bool)a_object))
			{
				set = TRUE;
			}
			else
			{
				set = FALSE;
			}
			a_bytes.Append(set);
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

		public override object Current1()
		{
			return i_compareTo;
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
	}
}
