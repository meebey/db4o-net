/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Btree;
using Db4objects.Db4o.Tests.Common.References;

namespace Db4objects.Db4o.Tests.Common.References
{
	public class ReferenceSystemTestCase : AbstractDb4oTestCase
	{
		private static readonly int[] IDS = new int[] { 100, 134, 689, 666, 775 };

		private static readonly object[] REFERENCES = CreateReferences();

		public static void Main(string[] args)
		{
			new ReferenceSystemTestCase().RunSolo();
		}

		public virtual void TestTransactionalReferenceSystem()
		{
			TransactionalReferenceSystem transactionalReferenceSystem = new TransactionalReferenceSystem
				();
			AssertAllRerefencesAvailableOnNew(transactionalReferenceSystem);
			transactionalReferenceSystem.Rollback();
			AssertEmpty(transactionalReferenceSystem);
			AssertAllRerefencesAvailableOnCommit(transactionalReferenceSystem);
		}

		public virtual void TestHashCodeReferenceSystem()
		{
			HashcodeReferenceSystem hashcodeReferenceSystem = new HashcodeReferenceSystem();
			AssertAllRerefencesAvailableOnNew(hashcodeReferenceSystem);
		}

		private void AssertAllRerefencesAvailableOnCommit(IReferenceSystem referenceSystem
			)
		{
			FillReferenceSystem(referenceSystem);
			referenceSystem.Commit();
			AssertAllReferencesAvailable(referenceSystem);
		}

		private void AssertAllRerefencesAvailableOnNew(IReferenceSystem referenceSystem)
		{
			FillReferenceSystem(referenceSystem);
			AssertAllReferencesAvailable(referenceSystem);
		}

		private void AssertEmpty(IReferenceSystem referenceSystem)
		{
			AssertContains(referenceSystem, new object[] {  });
		}

		private void AssertAllReferencesAvailable(IReferenceSystem referenceSystem)
		{
			AssertContains(referenceSystem, REFERENCES);
		}

		private void AssertContains(IReferenceSystem referenceSystem, object[] objects)
		{
			ExpectingVisitor expectingVisitor = new ExpectingVisitor(objects);
			referenceSystem.TraverseReferences(expectingVisitor);
			expectingVisitor.AssertExpectations();
		}

		private void FillReferenceSystem(IReferenceSystem referenceSystem)
		{
			for (int i = 0; i < REFERENCES.Length; i++)
			{
				referenceSystem.AddNewReference((ObjectReference)REFERENCES[i]);
			}
		}

		private static object[] CreateReferences()
		{
			object[] references = new object[IDS.Length];
			for (int i = 0; i < IDS.Length; i++)
			{
				ObjectReference @ref = new ObjectReference(IDS[i]);
				@ref.SetObject(IDS[i].ToString());
				references[i] = @ref;
			}
			return references;
		}
	}
}
