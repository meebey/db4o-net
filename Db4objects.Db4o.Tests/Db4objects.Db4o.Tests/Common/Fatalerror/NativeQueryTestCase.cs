/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new NativeQueryTestCase.Item("hello"));
		}

		public virtual void _test()
		{
			Assert.Expect(typeof(NativeQueryTestCase.NQError), new _ICodeBlock_30(this));
			Assert.IsTrue(Db().IsClosed());
		}

		private sealed class _ICodeBlock_30 : ICodeBlock
		{
			public _ICodeBlock_30(NativeQueryTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
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
