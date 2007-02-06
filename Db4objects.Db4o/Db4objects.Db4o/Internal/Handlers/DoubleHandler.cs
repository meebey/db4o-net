namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public sealed class DoubleHandler : Db4objects.Db4o.Internal.Handlers.LongHandler
	{
		private static readonly double DEFAULT_VALUE = System.Convert.ToDouble(0);

		public DoubleHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(
			stream)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return Db4objects.Db4o.Foundation.Coercion4.ToDouble(obj);
		}

		public override object DefaultValue()
		{
			return DEFAULT_VALUE;
		}

		public override int GetID()
		{
			return 5;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(double);
		}

		public override object PrimitiveNull()
		{
			return DEFAULT_VALUE;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return Db4objects.Db4o.Internal.Platform4.LongToDouble(ReadLong(a_bytes));
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			a_bytes.WriteLong(Db4objects.Db4o.Internal.Platform4.DoubleToLong(((double)a_object
				)));
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
