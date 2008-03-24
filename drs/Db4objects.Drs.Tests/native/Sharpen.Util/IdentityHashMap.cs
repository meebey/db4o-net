using System;
using System.Collections;

namespace Sharpen.Util
{
	public class IdentityHashMap : Hashtable
	{
		public IdentityHashMap() : base(EqualityComparer.Default)
		{	
		}

		public override void Add(object key, object value)
		{
			if (ContainsKey(key)) return;
			base.Add(key, value);
		}

		class EqualityComparer : IEqualityComparer
		{
			public static readonly IEqualityComparer Default = new EqualityComparer();

			public bool Equals(object x, object y)
			{
				return x == y;
			}

			public int GetHashCode(object obj)
			{
				return Sharpen.Runtime.IdentityHashCode(obj);
			}
		}
	}
}
