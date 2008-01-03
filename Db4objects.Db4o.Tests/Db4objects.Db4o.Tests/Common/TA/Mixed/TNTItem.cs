/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.Mixed;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	/// <exclude></exclude>
	public class TNTItem : ActivatableImpl
	{
		public NTItem ntItem;

		public TNTItem()
		{
		}

		public TNTItem(int v)
		{
			ntItem = new NTItem(v);
		}

		public virtual NTItem Value()
		{
			Activate(ActivationPurpose.Read);
			return ntItem;
		}
	}
}
