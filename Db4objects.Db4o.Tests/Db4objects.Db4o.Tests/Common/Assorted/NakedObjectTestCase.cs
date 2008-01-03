/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class NakedObjectTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public object field = new object();
		}

		public virtual void TestStoreNakedObjects()
		{
			Assert.Expect(typeof(ObjectNotStorableException), new _ICodeBlock_16(this));
		}

		private sealed class _ICodeBlock_16 : ICodeBlock
		{
			public _ICodeBlock_16(NakedObjectTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Store(new NakedObjectTestCase.Item());
			}

			private readonly NakedObjectTestCase _enclosing;
		}
	}
}
