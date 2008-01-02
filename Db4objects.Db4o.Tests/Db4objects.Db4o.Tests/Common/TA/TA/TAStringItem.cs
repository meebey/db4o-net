/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public class TAStringItem : ActivatableImpl
	{
		public string value;

		public object obj;

		public TAStringItem()
		{
		}

		public virtual string Value()
		{
			Activate(ActivationPurpose.READ);
			return value;
		}

		public virtual object Object()
		{
			Activate(ActivationPurpose.READ);
			return obj;
		}
	}
}
