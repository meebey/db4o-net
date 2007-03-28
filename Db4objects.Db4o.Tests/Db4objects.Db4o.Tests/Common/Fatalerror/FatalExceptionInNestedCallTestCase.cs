namespace Db4objects.Db4o.Tests.Common.Fatalerror
{
	public class FatalExceptionInNestedCallTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase().
				RunSolo();
		}

		public class Item
		{
			public Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				 _child;

			public int _depth;

			public Item()
			{
			}

			public Item(Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				 child, int depth)
			{
				_child = child;
				_depth = depth;
			}
		}

		[System.Serializable]
		public class FatalError : System.Exception
		{
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item childItem
				 = new Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				(null, 1);
			Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item parentItem
				 = new Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				(childItem, 0);
			Store(parentItem);
		}

		public virtual void Test()
		{
			EventRegistry().Updated += new Db4objects.Db4o.Events.ObjectEventHandler(new _AnonymousInnerClass48
				(this).OnEvent);
			Db4objects.Db4o.Query.IQuery q = this.NewQuery(typeof(Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item)
				);
			q.Descend("_depth").Constrain(0);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item parentItem
				 = (Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				)objectSet.Next();
			Db4oUnit.Assert.Expect(typeof(Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.FatalError)
				, new _AnonymousInnerClass62(this, parentItem));
		}

		private sealed class _AnonymousInnerClass48
		{
			public _AnonymousInnerClass48(FatalExceptionInNestedCallTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				Db4objects.Db4o.Events.ObjectEventArgs objectArgs = (Db4objects.Db4o.Events.ObjectEventArgs
					)args;
				Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item item
					 = (Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
					)objectArgs.Object;
				if (item._depth == 0)
				{
					throw new Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.FatalError
						();
				}
			}

			private readonly FatalExceptionInNestedCallTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass62 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass62(FatalExceptionInNestedCallTestCase _enclosing, Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				 parentItem)
			{
				this._enclosing = _enclosing;
				this.parentItem = parentItem;
			}

			public void Run()
			{
				this._enclosing.Db().Set(parentItem, 3);
			}

			private readonly FatalExceptionInNestedCallTestCase _enclosing;

			private readonly Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase.Item
				 parentItem;
		}

		private Db4objects.Db4o.Events.IEventRegistry EventRegistry()
		{
			return Db4objects.Db4o.Events.EventRegistryFactory.ForObjectContainer(Db());
		}
	}
}
