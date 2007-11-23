/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ObjectNotStorableExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ObjectNotStorableExceptionTestCase().RunSolo();
		}

		public class Item
		{
			public object nakedObject;
		}

		public virtual void TestObjectContainerAliveAfterObjectNotStorableException()
		{
			ObjectNotStorableExceptionTestCase.Item item = new ObjectNotStorableExceptionTestCase.Item
				();
			item.nakedObject = new object();
			Assert.Expect(typeof(ObjectNotStorableException), new _ICodeBlock_26(this, item));
			Store(new ObjectNotStorableExceptionTestCase.Item());
			Assert.IsNotNull(RetrieveOnlyInstance(typeof(ObjectNotStorableExceptionTestCase.Item
				)));
		}

		private sealed class _ICodeBlock_26 : ICodeBlock
		{
			public _ICodeBlock_26(ObjectNotStorableExceptionTestCase _enclosing, ObjectNotStorableExceptionTestCase.Item
				 item)
			{
				this._enclosing = _enclosing;
				this.item = item;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Store(item);
			}

			private readonly ObjectNotStorableExceptionTestCase _enclosing;

			private readonly ObjectNotStorableExceptionTestCase.Item item;
		}
	}
}
