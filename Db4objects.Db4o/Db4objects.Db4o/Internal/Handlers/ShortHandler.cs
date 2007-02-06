namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class ShortHandler : Db4objects.Db4o.Internal.Handlers.PrimitiveHandler
	{
		internal const int LENGTH = Db4objects.Db4o.Internal.Const4.SHORT_BYTES + Db4objects.Db4o.Internal.Const4
			.ADDED_LENGTH;

		private static readonly short i_primitive = (short)0;

		public ShortHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(stream
			)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return Db4objects.Db4o.Foundation.Coercion4.ToShort(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 8;
		}

		public override int LinkLength()
		{
			return LENGTH;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(short);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return ReadShort(a_bytes);
		}

		internal static short ReadShort(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			int ret = 0;
			for (int i = 0; i < Db4objects.Db4o.Internal.Const4.SHORT_BYTES; i++)
			{
				ret = (ret << 8) + (a_bytes._buffer[a_bytes._offset++] & unchecked((int)(0xff)));
			}
			return (short)ret;
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			WriteShort(((short)a_object), a_bytes);
		}

		internal static void WriteShort(int a_short, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			for (int i = 0; i < Db4objects.Db4o.Internal.Const4.SHORT_BYTES; i++)
			{
				a_bytes._buffer[a_bytes._offset++] = (byte)(a_short >> ((Db4objects.Db4o.Internal.Const4
					.SHORT_BYTES - 1 - i) * 8));
			}
		}

		private short i_compareTo;

		private short Val(object obj)
		{
			return ((short)obj);
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
			return obj is short && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is short && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is short && Val(obj) < i_compareTo;
		}
	}
}
