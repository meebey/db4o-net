/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	/// <exclude></exclude>
	public class ExceptionsOnNotStorableIsDefaultTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ExceptionsOnNotStorableIsDefaultTestCase().RunSolo();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.CallConstructors(true);
		}

		public class Item
		{
			public Item(object obj)
			{
				if (obj == null)
				{
					throw new Exception();
				}
			}

			public static ExceptionsOnNotStorableIsDefaultTestCase.Item NewItem()
			{
				return new ExceptionsOnNotStorableIsDefaultTestCase.Item(new object());
			}
		}

		public virtual void TestObjectContainerAliveAfterObjectNotStorableException()
		{
			ExceptionsOnNotStorableIsDefaultTestCase.Item item = ExceptionsOnNotStorableIsDefaultTestCase.Item
				.NewItem();
			Assert.Expect(typeof(ObjectNotStorableException), new _ICodeBlock_39(this, item));
		}

		private sealed class _ICodeBlock_39 : ICodeBlock
		{
			public _ICodeBlock_39(ExceptionsOnNotStorableIsDefaultTestCase _enclosing, ExceptionsOnNotStorableIsDefaultTestCase.Item
				 item)
			{
				this._enclosing = _enclosing;
				this.item = item;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				this._enclosing.Store(item);
			}

			private readonly ExceptionsOnNotStorableIsDefaultTestCase _enclosing;

			private readonly ExceptionsOnNotStorableIsDefaultTestCase.Item item;
		}
	}
}
