namespace Db4objects.Db4o.Tests.Common.Fatalerror
{
	public class NativeQueryTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
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
			new Db4objects.Db4o.Tests.Common.Fatalerror.NativeQueryTestCase().RunSoloAndClientServer
				();
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Fatalerror.NativeQueryTestCase.Item("hello"
				));
		}

		public virtual void _test()
		{
			Db4oUnit.Assert.Expect(typeof(Db4objects.Db4o.Tests.Common.Fatalerror.NativeQueryTestCase.NQError)
				, new _AnonymousInnerClass30(this));
			Db4oUnit.Assert.IsTrue(Db().IsClosed());
		}

		private sealed class _AnonymousInnerClass30 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass30(NativeQueryTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4objects.Db4o.Query.Predicate fatalErrorPredicate = new Db4objects.Db4o.Tests.Common.Fatalerror.NativeQueryTestCase.FatalErrorPredicate
					();
				this._enclosing.Db().Query(fatalErrorPredicate);
			}

			private readonly NativeQueryTestCase _enclosing;
		}

		[System.Serializable]
		public class FatalErrorPredicate : Db4objects.Db4o.Query.Predicate
		{
			public virtual bool Match(object item)
			{
				throw new Db4objects.Db4o.Tests.Common.Fatalerror.NativeQueryTestCase.NQError("nq error!"
					);
			}
		}

		[System.Serializable]
		public class NQError : System.Exception
		{
			public NQError(string msg) : base(msg)
			{
			}
		}
	}
}
