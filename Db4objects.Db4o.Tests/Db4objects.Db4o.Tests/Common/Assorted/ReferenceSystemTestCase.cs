namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ReferenceSystemTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static readonly int[] IDS = new int[] { 100, 134, 689, 666, 775 };

		private static readonly object[] REFERENCES = CreateReferences();

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.ReferenceSystemTestCase().RunSolo();
		}

		public virtual void TestTransactionalReferenceSystem()
		{
			Db4objects.Db4o.Internal.TransactionalReferenceSystem transactionalReferenceSystem
				 = new Db4objects.Db4o.Internal.TransactionalReferenceSystem();
			AssertAllRerefencesAvailableOnNew(transactionalReferenceSystem);
			transactionalReferenceSystem.Rollback();
			AssertEmpty(transactionalReferenceSystem);
			AssertAllRerefencesAvailableOnCommit(transactionalReferenceSystem);
		}

		public virtual void TestHashCodeReferenceSystem()
		{
			Db4objects.Db4o.Internal.HashcodeReferenceSystem hashcodeReferenceSystem = new Db4objects.Db4o.Internal.HashcodeReferenceSystem
				();
			AssertAllRerefencesAvailableOnNew(hashcodeReferenceSystem);
		}

		private void AssertAllRerefencesAvailableOnCommit(Db4objects.Db4o.Internal.IReferenceSystem
			 referenceSystem)
		{
			FillReferenceSystem(referenceSystem);
			referenceSystem.Commit();
			AssertAllReferencesAvailable(referenceSystem);
		}

		private void AssertAllRerefencesAvailableOnNew(Db4objects.Db4o.Internal.IReferenceSystem
			 referenceSystem)
		{
			FillReferenceSystem(referenceSystem);
			AssertAllReferencesAvailable(referenceSystem);
		}

		private void AssertEmpty(Db4objects.Db4o.Internal.IReferenceSystem referenceSystem
			)
		{
			AssertContains(referenceSystem, new object[] {  });
		}

		private void AssertAllReferencesAvailable(Db4objects.Db4o.Internal.IReferenceSystem
			 referenceSystem)
		{
			AssertContains(referenceSystem, REFERENCES);
		}

		private void AssertContains(Db4objects.Db4o.Internal.IReferenceSystem referenceSystem
			, object[] objects)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(objects);
			referenceSystem.TraverseReferences(expectingVisitor);
			expectingVisitor.AssertExpectations();
		}

		private void FillReferenceSystem(Db4objects.Db4o.Internal.IReferenceSystem referenceSystem
			)
		{
			for (int i = 0; i < REFERENCES.Length; i++)
			{
				referenceSystem.AddNewReference((Db4objects.Db4o.Internal.ObjectReference)REFERENCES
					[i]);
			}
		}

		private static object[] CreateReferences()
		{
			object[] references = new object[IDS.Length];
			for (int i = 0; i < IDS.Length; i++)
			{
				Db4objects.Db4o.Internal.ObjectReference @ref = new Db4objects.Db4o.Internal.ObjectReference
					(IDS[i]);
				@ref.SetObject(IDS[i].ToString());
				references[i] = @ref;
			}
			return references;
		}
	}
}
