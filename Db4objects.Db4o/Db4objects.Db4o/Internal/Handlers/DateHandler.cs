namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class DateHandler : Db4objects.Db4o.Internal.Handlers.LongHandler
	{
		private static readonly Sharpen.Util.Date PROTO = new Sharpen.Util.Date(0);

		public DateHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(stream
			)
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
			catch
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

		public override object PrimitiveNull()
		{
			return null;
		}

		public override object Read(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.StatefulBuffer writer, bool redirect)
		{
			return mf._primitive.ReadDate(writer);
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return PrimitiveMarshaller().ReadDate(a_bytes);
		}

		private Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller PrimitiveMarshaller
			()
		{
			return Db4objects.Db4o.Internal.Marshall.MarshallerFamily.Current()._primitive;
		}

		public override void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			)
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

		public static string Now()
		{
			return Db4objects.Db4o.Internal.Platform4.Format(new Sharpen.Util.Date(), true);
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
