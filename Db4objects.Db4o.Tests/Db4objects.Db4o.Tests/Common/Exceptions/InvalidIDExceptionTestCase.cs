/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class InvalidIDExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new InvalidIDExceptionTestCase().RunAll();
		}

		public virtual void TestBigId()
		{
			Item item = new Item();
			Store(item);
			long id = Db().GetID(item);
			Assert.Expect(typeof(InvalidIDException), typeof(InvalidIDException), new _ICodeBlock_18
				(this, id));
		}

		private sealed class _ICodeBlock_18 : ICodeBlock
		{
			public _ICodeBlock_18(InvalidIDExceptionTestCase _enclosing, long id)
			{
				this._enclosing = _enclosing;
				this.id = id;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().GetByID(id + 10000000);
			}

			private readonly InvalidIDExceptionTestCase _enclosing;

			private readonly long id;
		}

		//		Assert.expect(InvalidIDException.class, new CodeBlock() {
		//			public void run() throws Throwable {
		//				db().getByID(id + 1);
		//			}
		//			
		//		});
		/// <exception cref="Exception"></exception>
		public virtual void TestSmallId()
		{
			Assert.Expect(typeof(InvalidIDException), new _ICodeBlock_34(this));
		}

		private sealed class _ICodeBlock_34 : ICodeBlock
		{
			public _ICodeBlock_34(InvalidIDExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().GetByID(1000);
			}

			private readonly InvalidIDExceptionTestCase _enclosing;
		}
	}
}
