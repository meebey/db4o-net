namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class YapTypeAbstract : Db4objects.Db4o.YapJavaClass, Db4objects.Db4o.IYapType
	{
		public YapTypeAbstract(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		private int i_linkLength;

		private object i_compareTo;

		public abstract int Compare(object compare, object with);

		public virtual string DotNetClassName()
		{
			string className = this.GetType().FullName;
			int pos = className.IndexOf(".Net");
			if (pos >= 0)
			{
				return "System." + Sharpen.Runtime.Substring(className, pos + 4) + ", mscorlib";
			}
			return DefaultValue().GetType().FullName;
		}

		public abstract bool IsEqual(object compare, object with);

		internal virtual void Initialize()
		{
			byte[] bytes = new byte[65];
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = 55;
			}
			Write(PrimitiveNull(), bytes, 0);
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 55)
				{
					i_linkLength = i;
					break;
				}
			}
		}

		public override int GetID()
		{
			return TypeID();
		}

		public virtual string GetName()
		{
			return DotNetClassName();
		}

		public override int LinkLength()
		{
			return i_linkLength;
		}

		protected override System.Type PrimitiveJavaClass()
		{
			return null;
		}

		internal override object PrimitiveNull()
		{
			return DefaultValue();
		}

		public abstract object Read(byte[] bytes, int offset);

		internal override object Read1(Db4objects.Db4o.YapReader a_bytes)
		{
			int offset = a_bytes._offset;
			object ret = Read(a_bytes._buffer, a_bytes._offset);
			a_bytes._offset = offset + LinkLength();
			return ret;
		}

		public abstract int TypeID();

		public abstract void Write(object obj, byte[] bytes, int offset);

		public override void Write(object a_object, Db4objects.Db4o.YapReader a_bytes)
		{
			int offset = a_bytes._offset;
			if (a_object != null)
			{
				Write(a_object, a_bytes._buffer, a_bytes._offset);
			}
			a_bytes._offset = offset + LinkLength();
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = obj;
		}

		public override object Current1()
		{
			return i_compareTo;
		}

		internal override bool IsEqual1(object obj)
		{
			return IsEqual(i_compareTo, obj);
		}

		internal override bool IsGreater1(object obj)
		{
			if (ClassReflector().IsInstance(obj) && !IsEqual(i_compareTo, obj))
			{
				return Compare(i_compareTo, obj) > 0;
			}
			return false;
		}

		internal override bool IsSmaller1(object obj)
		{
			if (ClassReflector().IsInstance(obj) && !IsEqual(i_compareTo, obj))
			{
				return Compare(i_compareTo, obj) < 0;
			}
			return false;
		}
	}
}
