/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	/// <exclude></exclude>
	public class TItem : ActivatableImpl
	{
		public int value;

		public TItem()
		{
		}

		public TItem(int v)
		{
			value = v;
		}

		public virtual int Value()
		{
			Activate(ActivationPurpose.Read);
			return value;
		}
	}
}
