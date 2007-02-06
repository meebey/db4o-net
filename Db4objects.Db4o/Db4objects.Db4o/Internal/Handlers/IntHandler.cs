namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class IntHandler : Db4objects.Db4o.Internal.Handlers.PrimitiveHandler
	{
		private static readonly int i_primitive = 0;

		public IntHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(stream
			)
		{
		}

		public override object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object
			 obj)
		{
			return Db4objects.Db4o.Foundation.Coercion4.ToInt(obj);
		}

		public override object DefaultValue()
		{
			return i_primitive;
		}

		public override int GetID()
		{
			return 1;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return typeof(int);
		}

		public override int LinkLength()
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH;
		}

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return a_bytes.ReadInt();
		}

		public static int ReadInt(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			return a_bytes.ReadInt();
		}

		public override void Write(object obj, Db4objects.Db4o.Internal.Buffer writer)
		{
			Write(((int)obj), writer);
		}

		public virtual void Write(int intValue, Db4objects.Db4o.Internal.Buffer writer)
		{
			WriteInt(intValue, writer);
		}

		public static void WriteInt(int a_int, Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			a_bytes.WriteInt(a_int);
		}

		private int i_compareTo;

		protected int Val(object obj)
		{
			return ((int)obj);
		}

		public virtual int CompareTo(int other)
		{
			return other - i_compareTo;
		}

		public virtual void PrepareComparison(int i)
		{
			i_compareTo = i;
		}

		internal override void PrepareComparison1(object obj)
		{
			PrepareComparison(Val(obj));
		}

		public override object Current1()
		{
			return CurrentInt();
		}

		public virtual int CurrentInt()
		{
			return i_compareTo;
		}

		internal override bool IsEqual1(object obj)
		{
			return obj is int && Val(obj) == i_compareTo;
		}

		internal override bool IsGreater1(object obj)
		{
			return obj is int && Val(obj) > i_compareTo;
		}

		internal override bool IsSmaller1(object obj)
		{
			return obj is int && Val(obj) < i_compareTo;
		}

		public override void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers
			)
		{
			readers.IncrementIntSize();
		}
	}
}
