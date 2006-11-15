namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeOnUpdate : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Atom
		{
			public Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom child;

			public string name;

			public Atom()
			{
			}

			public Atom(Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom child)
			{
				this.child = child;
			}

			public Atom(string name)
			{
				this.name = name;
			}

			public Atom(Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom child, string
				 name) : this(child)
			{
				this.name = name;
			}
		}

		public object child;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration conf)
		{
			conf.ObjectClass(this).CascadeOnUpdate(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate cou = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate
				();
			cou.child = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom(new Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom
				("storedChild"), "stored");
			Db().Set(cou);
		}

		public virtual void Test()
		{
			Foreach(GetType(), new _AnonymousInnerClass48(this));
			Reopen();
			Foreach(GetType(), new _AnonymousInnerClass59(this));
		}

		private sealed class _AnonymousInnerClass48 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass48(CascadeOnUpdate _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate cou = (Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate
					)obj;
				((Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom)cou.child).name = "updated";
				((Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom)cou.child).child.name
					 = "updated";
				this._enclosing.Db().Set(cou);
			}

			private readonly CascadeOnUpdate _enclosing;
		}

		private sealed class _AnonymousInnerClass59 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass59(CascadeOnUpdate _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate cou = (Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate
					)obj;
				Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom atom = (Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate.Atom
					)cou.child;
				Db4oUnit.Assert.AreEqual("updated", atom.name);
				Db4oUnit.Assert.AreNotEqual("updated", atom.child.name);
			}

			private readonly CascadeOnUpdate _enclosing;
		}
	}
}
