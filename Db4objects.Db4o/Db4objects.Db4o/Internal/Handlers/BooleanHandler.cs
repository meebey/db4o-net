namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public sealed class BooleanHandler : Db4objects.Db4o.Internal.Handlers.PrimitiveHandler
	{
		internal const int LENGTH = 1 + Db4objects.Db4o.Internal.Const4.ADDED_LENGTH;

		private const byte TRUE = (byte)'T';

		private const byte FALSE = (byte)'F';

		private const byte NULL = (byte)'N';

		private static readonly bool i_primitive = false;

		public BooleanHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base
			(stream)
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

		public override object PrimitiveNull()
		{
			return i_primitive;
		}

		internal override object Read1(Db4objects.Db4o.Internal.Buffer a_bytes)
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

		public override object WriteNew(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, object obj, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer buffer, 
			bool withIndirection, bool restoreLinkeOffset)
		{
			Write(obj, buffer);
			return obj;
		}

		public override void Write(object obj, Db4objects.Db4o.Internal.Buffer buffer)
		{
			buffer.Append(GetEncodedByteValue(obj));
		}

		private byte GetEncodedByteValue(object obj)
		{
			if (obj == null)
			{
				return NULL;
			}
			if (((bool)obj))
			{
				return TRUE;
			}
			return FALSE;
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
