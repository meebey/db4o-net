/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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

		public virtual void Test()
		{
			Item item = new Item();
			Store(item);
			long id = Db().GetID(item);
			Assert.Expect(typeof(InvalidIDException), new _AnonymousInnerClass18(this, id));
		}

		private sealed class _AnonymousInnerClass18 : ICodeBlock
		{
			public _AnonymousInnerClass18(InvalidIDExceptionTestCase _enclosing, long id)
			{
				this._enclosing = _enclosing;
				this.id = id;
			}

			public void Run()
			{
				this._enclosing.Db().GetByID(id + 10000000);
			}

			private readonly InvalidIDExceptionTestCase _enclosing;

			private readonly long id;
		}
	}
}
