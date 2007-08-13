/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class TransactionTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		private const int TEST_ID = 5;

		public virtual void TestRemoveReferenceSystemOnClose()
		{
			LocalObjectContainer container = (LocalObjectContainer)Db();
			TransactionalReferenceSystem referenceSystem = container.CreateReferenceSystem();
			Transaction transaction = container.NewTransaction(container.SystemTransaction(), 
				referenceSystem);
			referenceSystem.AddNewReference(new ObjectReference(TEST_ID));
			referenceSystem.AddNewReference(new ObjectReference(TEST_ID + 1));
			container.ReferenceSystemRegistry().RemoveId(TEST_ID);
			Assert.IsNull(referenceSystem.ReferenceForId(TEST_ID));
			transaction.Close(false);
			container.ReferenceSystemRegistry().RemoveId(TEST_ID + 1);
			Assert.IsNotNull(referenceSystem.ReferenceForId(TEST_ID + 1));
		}
	}
}
