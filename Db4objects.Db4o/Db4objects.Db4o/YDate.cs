namespace Db4objects.Db4o
{
	internal sealed class YDate : Db4objects.Db4o.YLong
	{
		private static readonly Sharpen.Util.Date PROTO = new Sharpen.Util.Date(0);

		public YDate(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return CanHold(claxx) ? obj : Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		public override void CopyValue(object a_from, object a_to)
		{
			try
			{
				((Sharpen.Util.Date)a_to).SetTime(((Sharpen.Util.Date)a_from).GetTime());
			}
			catch (System.Exception e)
			{
			}
		}

		public override object DefaultValue()
		{
			return PROTO;
		}

		public override int GetID()
		{
			return 10;
		}

		public override bool IndexNullHandling()
		{
			return true;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return null;
		}

		internal override object PrimitiveNull()
		{
			return null;
		}

		internal override object Read1(Db4objects.Db4o.YapReader a_bytes)
		{
			return new Sharpen.Util.Date(ReadLong(a_bytes));
		}

		public override void Write(object a_object, Db4objects.Db4o.YapReader a_bytes)
		{
			if (a_object == null)
			{
				a_object = new Sharpen.Util.Date(0);
			}
			a_bytes.WriteLong(((Sharpen.Util.Date)a_object).GetTime());
		}

		public override object Current1()
		{
			return new Sharpen.Util.Date(CurrentLong());
		}

		internal static string Now()
		{
			return Db4objects.Db4o.Platform4.Format(new Sharpen.Util.Date(), true);
		}

		internal override long Val(object obj)
		{
			return ((Sharpen.Util.Date)obj).GetTime();
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is Sharpen.Util.Date && Val(obj) == CurrentLong();
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is Sharpen.Util.Date && Val(obj) > CurrentLong();
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is Sharpen.Util.Date && Val(obj) < CurrentLong();
		}
	}
}
