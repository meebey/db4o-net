namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class FloatHandler : Db4objects.Db4o.Internal.Handlers.IntHandler
	{
		private static readonly float i_primitive = System.Convert.ToSingle(0);

		public FloatHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(stream
			)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return Db4objects.Db4o.Foundation.Coercion4.ToFloat(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 3;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(float);
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return Sharpen.Runtime.IntBitsToFloat(a_bytes.ReadInt());
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			WriteInt(Sharpen.Runtime.FloatToIntBits(((float)a_object)), a_bytes);
		}

		private float i_compareTo;

		private float Valu(object obj)
		{
			return ((float)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = Valu(obj);
		}

		public override object Current1()
		{
			return i_compareTo;
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is float && Valu(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is float && Valu(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is float && Valu(obj) < i_compareTo;
		}
	}
}
