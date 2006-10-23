namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public sealed class YDouble : Db4objects.Db4o.YLong
	{
		private static readonly double i_primitive = System.Convert.ToDouble(0);

		public YDouble(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return Db4objects.Db4o.Foundation.Coercion4.ToDouble(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 5;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(double);
		}

		internal override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.YapReader a_bytes)
		{
			return Db4objects.Db4o.Platform4.LongToDouble(ReadLong(a_bytes));
		}

		public override void Write(object a_object, Db4objects.Db4o.YapReader a_bytes)
		{
			a_bytes.WriteLong(Db4objects.Db4o.Platform4.DoubleToLong(((double)a_object)));
		}

		private double i_compareToDouble;

		private double Dval(object obj)
		{
			return ((double)obj);
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareToDouble = Dval(obj);
		}

		public override object Current1()
		{
			return i_compareToDouble;
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is double && Dval(obj) == i_compareToDouble;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is double && Dval(obj) > i_compareToDouble;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is double && Dval(obj) < i_compareToDouble;
		}
	}
}