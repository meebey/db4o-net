namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class KeySpecHashtable4 : Db4objects.Db4o.Foundation.Hashtable4
	{
		private KeySpecHashtable4() : base((Db4objects.Db4o.Foundation.IDeepClone)null)
		{
		}

		public KeySpecHashtable4(int a_size) : base(a_size)
		{
		}

		public virtual void Put(Db4objects.Db4o.Foundation.KeySpec spec, byte value)
		{
			base.Put(spec, value);
		}

		public virtual void Put(Db4objects.Db4o.Foundation.KeySpec spec, bool value)
		{
			base.Put(spec, value);
		}

		public virtual void Put(Db4objects.Db4o.Foundation.KeySpec spec, int value)
		{
			base.Put(spec, value);
		}

		public virtual void Put(Db4objects.Db4o.Foundation.KeySpec spec, object value)
		{
			base.Put(spec, value);
		}

		public virtual byte GetAsByte(Db4objects.Db4o.Foundation.KeySpec spec)
		{
			return ((byte)Get(spec));
		}

		public virtual bool GetAsBoolean(Db4objects.Db4o.Foundation.KeySpec spec)
		{
			return ((bool)Get(spec));
		}

		public virtual int GetAsInt(Db4objects.Db4o.Foundation.KeySpec spec)
		{
			return ((int)Get(spec));
		}

		public virtual string GetAsString(Db4objects.Db4o.Foundation.KeySpec spec)
		{
			return (string)Get(spec);
		}

		public virtual object Get(Db4objects.Db4o.Foundation.KeySpec spec)
		{
			object value = base.Get(spec);
			if (value == null)
			{
				value = spec.DefaultValue();
				if (value != null)
				{
					base.Put(spec, value);
				}
			}
			return value;
		}

		public override object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.KeySpecHashtable4(), obj);
		}
	}
}
