namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : Db4oUnit.ITestLifeCycle
	{
		public virtual void TestPrimitiveIndex()
		{
			Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.AssertIndex(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture
				.PRIMITIVE_FIELDNAME);
		}

		public virtual void TestWrapperIndex()
		{
			Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.AssertIndex(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture
				.WRAPPER_FIELDNAME);
		}

		public virtual void TestTypedObjectIndex()
		{
			Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.ForceIndex();
			Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants.BACKUPFILENAME
				);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Db4oFactory
				.NewConfiguration(), Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME);
			Db4objects.Db4o.Query.IQuery query = db.Query();
			query.Constrain(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				);
			query.Descend(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.TYPEDOBJECT_FIELDNAME
				).Descend(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.PRIMITIVE_FIELDNAME
				).Constrain(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.VALUE);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			db.Close();
		}

		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants.BACKUPFILENAME
				);
			Db4oUnit.Assert.Expect(typeof(System.IO.IOException), new _AnonymousInnerClass37(
				this));
		}

		private sealed class _AnonymousInnerClass37 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass37(SlotDefragmentTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
					.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants.BACKUPFILENAME
					);
			}

			private readonly SlotDefragmentTestCase _enclosing;
		}

		public virtual void SetUp()
		{
			new Sharpen.IO.File(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME).Delete();
			new Sharpen.IO.File(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.BACKUPFILENAME).Delete();
			Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.CreateFile(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME);
		}

		public virtual void TearDown()
		{
		}
	}
}
