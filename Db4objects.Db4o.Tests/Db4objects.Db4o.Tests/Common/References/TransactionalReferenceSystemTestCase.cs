/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.References;

namespace Db4objects.Db4o.Tests.Common.References
{
	public class TransactionalReferenceSystemTestCase : ReferenceSystemTestCaseBase
	{
		protected override IReferenceSystem CreateReferenceSystem()
		{
			return new TransactionalReferenceSystem();
		}
	}
}
