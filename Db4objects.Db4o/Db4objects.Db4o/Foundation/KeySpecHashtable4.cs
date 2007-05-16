/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class KeySpecHashtable4 : Hashtable4
	{
		private KeySpecHashtable4() : base((IDeepClone)null)
		{
		}

		public KeySpecHashtable4(int a_size) : base(a_size)
		{
		}

		public virtual void Put(KeySpec spec, byte value)
		{
			base.Put(spec, value);
		}

		public virtual void Put(KeySpec spec, bool value)
		{
			base.Put(spec, value);
		}

		public virtual void Put(KeySpec spec, int value)
		{
			base.Put(spec, value);
		}

		public virtual void Put(KeySpec spec, object value)
		{
			base.Put(spec, value);
		}

		public virtual byte GetAsByte(KeySpec spec)
		{
			return ((byte)Get(spec));
		}

		public virtual bool GetAsBoolean(KeySpec spec)
		{
			return ((bool)Get(spec));
		}

		public virtual int GetAsInt(KeySpec spec)
		{
			return ((int)Get(spec));
		}

		public virtual TernaryBool GetAsTernaryBool(KeySpec spec)
		{
			return (TernaryBool)Get(spec);
		}

		public virtual string GetAsString(KeySpec spec)
		{
			return (string)Get(spec);
		}

		public virtual object Get(KeySpec spec)
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
