/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public class TAIntItem : ActivatableImpl
	{
		public int value;

		public object obj;

		public int i;

		public TAIntItem()
		{
		}

		public virtual int Value()
		{
			Activate(ActivationPurpose.READ);
			return value;
		}

		public virtual int IntegerValue()
		{
			Activate(ActivationPurpose.READ);
			return i;
		}

		public virtual object Object()
		{
			Activate(ActivationPurpose.READ);
			return obj;
		}
	}
}
