/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Fatalerror;

namespace Db4objects.Db4o.Tests.Common.Fatalerror
{
	public class NativeQueryTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public string str;

			public Item(string s)
			{
				str = s;
			}
		}

		public static void Main(string[] args)
		{
			new NativeQueryTestCase().RunSoloAndClientServer();
		}

		protected override void Store()
		{
			Store(new NativeQueryTestCase.Item("hello"));
		}

		public virtual void _test()
		{
			Assert.Expect(typeof(NativeQueryTestCase.NQError), new _AnonymousInnerClass30(this
				));
			Assert.IsTrue(Db().IsClosed());
		}

		private sealed class _AnonymousInnerClass30 : ICodeBlock
		{
			public _AnonymousInnerClass30(NativeQueryTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Predicate fatalErrorPredicate = new NativeQueryTestCase.FatalErrorPredicate();
				this._enclosing.Db().Query(fatalErrorPredicate);
			}

			private readonly NativeQueryTestCase _enclosing;
		}

		[System.Serializable]
		public class FatalErrorPredicate : Predicate
		{
			public virtual bool Match(object item)
			{
				throw new NativeQueryTestCase.NQError("nq error!");
			}
		}

		[System.Serializable]
		public class NQError : Exception
		{
			public NQError(string msg) : base(msg)
			{
			}
		}
	}
}
