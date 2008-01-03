/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	public class TALinkedListItem : ActivatableImpl
	{
		public TALinkedList list;

		public TALinkedListItem()
		{
		}

		public virtual TALinkedList List()
		{
			Activate(ActivationPurpose.Read);
			return list;
		}
	}
}
