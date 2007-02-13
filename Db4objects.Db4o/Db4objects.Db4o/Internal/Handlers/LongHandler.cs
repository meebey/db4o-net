namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class LongHandler : Db4objects.Db4o.Internal.Handlers.PrimitiveHandler
	{
		private static readonly long i_primitive = System.Convert.ToInt64(0);

		public LongHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(stream
			)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return Db4objects.Db4o.Foundation.Coercion4.ToLong(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 2;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(long);
		}

		public override int LinkLength()
		{
			return Db4objects.Db4o.Internal.Const4.LONG_LENGTH;
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return ReadLong(a_bytes);
		}

		public static long ReadLong(Db4objects.Db4o.Internal.Buffer bytes)
		{
			long ret = 0;
			ret = Db4objects.Db4o.Foundation.PrimitiveCodec.ReadLong(bytes._buffer, bytes._offset
				);
			IncrementOffset(bytes);
			return ret;
		}

		public override void Write(object obj, Db4objects.Db4o.Internal.Buffer buffer)
		{
			WriteLong(((long)obj), buffer);
		}

		public static void WriteLong(long val, Db4objects.Db4o.Internal.Buffer bytes)
		{
			Db4objects.Db4o.Foundation.PrimitiveCodec.WriteLong(bytes._buffer, bytes._offset, 
				val);
			IncrementOffset(bytes);
		}

		private static void IncrementOffset(Db4objects.Db4o.Internal.Buffer buffer)
		{
			buffer.IncrementOffset(Db4objects.Db4o.Internal.Const4.LONG_BYTES);
		}

		private long i_compareTo;

		protected long CurrentLong()
		{
			return i_compareTo;
		}

		internal virtual long Val(object obj)
		{
			return ((long)obj);
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
			return obj is long && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is long && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is long && Val(obj) < i_compareTo;
		}
	}
}
