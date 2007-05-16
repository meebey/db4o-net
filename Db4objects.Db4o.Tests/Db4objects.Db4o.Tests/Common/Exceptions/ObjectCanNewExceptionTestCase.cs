/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class ObjectCanNewExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ObjectCanNewExceptionTestCase().RunSoloAndClientServer();
		}

		public class Item
		{
			public virtual bool ObjectCanNew(IObjectContainer container)
			{
				throw new ItemException();
			}
		}

		public virtual void Test()
		{
			Assert.Expect(typeof(ReflectException), typeof(ItemException), new _AnonymousInnerClass24
				(this));
		}

		private sealed class _AnonymousInnerClass24 : ICodeBlock
		{
			public _AnonymousInnerClass24(ObjectCanNewExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Store(new ObjectCanNewExceptionTestCase.Item());
			}

			private readonly ObjectCanNewExceptionTestCase _enclosing;
		}
	}
}
