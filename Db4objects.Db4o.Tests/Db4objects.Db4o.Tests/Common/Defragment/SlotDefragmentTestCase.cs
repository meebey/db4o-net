/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : ITestLifeCycle
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrimitiveIndex()
		{
			SlotDefragmentFixture.AssertIndex(SlotDefragmentFixture.PrimitiveFieldname);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWrapperIndex()
		{
			SlotDefragmentFixture.AssertIndex(SlotDefragmentFixture.WrapperFieldname);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTypedObjectIndex()
		{
			SlotDefragmentFixture.ForceIndex();
			Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.Filename
				, SlotDefragmentTestConstants.Backupfilename);
			IObjectContainer db = Db4oFactory.OpenFile(Db4oFactory.NewConfiguration(), SlotDefragmentTestConstants
				.Filename);
			IQuery query = db.Query();
			query.Constrain(typeof(SlotDefragmentFixture.Data));
			query.Descend(SlotDefragmentFixture.TypedobjectFieldname).Descend(SlotDefragmentFixture
				.PrimitiveFieldname).Constrain(SlotDefragmentFixture.Value);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Count);
			db.Close();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.Filename
				, SlotDefragmentTestConstants.Backupfilename);
			Assert.Expect(typeof(IOException), new _ICodeBlock_37());
		}

		private sealed class _ICodeBlock_37 : ICodeBlock
		{
			public _ICodeBlock_37()
			{
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4objects.Db4o.Defragment.Defragment.Defrag(SlotDefragmentTestConstants.Filename
					, SlotDefragmentTestConstants.Backupfilename);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			new Sharpen.IO.File(SlotDefragmentTestConstants.Filename).Delete();
			new Sharpen.IO.File(SlotDefragmentTestConstants.Backupfilename).Delete();
			SlotDefragmentFixture.CreateFile(SlotDefragmentTestConstants.Filename);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
