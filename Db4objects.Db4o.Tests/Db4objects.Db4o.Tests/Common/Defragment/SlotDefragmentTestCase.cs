using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : ITestLifeCycle
	{
		public virtual void TestPrimitiveIndex()
		{
			SlotDefragmentFixture.AssertIndex(SlotDefragmentFixture.PRIMITIVE_FIELDNAME);
		}

		public virtual void TestWrapperIndex()
		{
			SlotDefragmentFixture.AssertIndex(SlotDefragmentFixture.WRAPPER_FIELDNAME);
		}

		public virtual void TestTypedObjectIndex()
		{
			SlotDefragmentFixture.ForceIndex();
			Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.FILENAME
				, SlotDefragmentTestConstants.BACKUPFILENAME);
			IObjectContainer db = Db4oFactory.OpenFile(Db4oFactory.NewConfiguration(), SlotDefragmentTestConstants
				.FILENAME);
			IQuery query = db.Query();
			query.Constrain(typeof(SlotDefragmentFixture.Data));
			query.Descend(SlotDefragmentFixture.TYPEDOBJECT_FIELDNAME).Descend(SlotDefragmentFixture
				.PRIMITIVE_FIELDNAME).Constrain(SlotDefragmentFixture.VALUE);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			db.Close();
		}

		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.FILENAME
				, SlotDefragmentTestConstants.BACKUPFILENAME);
			Assert.Expect(typeof(IOException), new _AnonymousInnerClass37(this));
		}

		private sealed class _AnonymousInnerClass37 : ICodeBlock
		{
			public _AnonymousInnerClass37(SlotDefragmentTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.FILENAME
					, SlotDefragmentTestConstants.BACKUPFILENAME);
			}

			private readonly SlotDefragmentTestCase _enclosing;
		}

		public virtual void SetUp()
		{
			new Sharpen.IO.File(SlotDefragmentTestConstants.FILENAME).Delete();
			new Sharpen.IO.File(SlotDefragmentTestConstants.BACKUPFILENAME).Delete();
			SlotDefragmentFixture.CreateFile(SlotDefragmentTestConstants.FILENAME);
		}

		public virtual void TearDown()
		{
		}
	}
}
