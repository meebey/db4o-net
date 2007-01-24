namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeToArray : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Atom
		{
			public Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom child;

			public string name;

			public Atom()
			{
			}

			public Atom(Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom child)
			{
				this.child = child;
			}

			public Atom(string name)
			{
				this.name = name;
			}

			public Atom(Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom child, string
				 name) : this(child)
			{
				this.name = name;
			}
		}

		public object[] objects;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration conf)
		{
			conf.ObjectClass(this).CascadeOnUpdate(true);
			conf.ObjectClass(this).CascadeOnDelete(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeToArray cta = new Db4objects.Db4o.Tests.Common.Querying.CascadeToArray
				();
			cta.objects = new object[] { new Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom
				("stored1"), new Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom(new Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom
				("storedChild1"), "stored2") };
			Db().Set(cta);
		}

		public virtual void Test()
		{
			Foreach(GetType(), new _AnonymousInnerClass52(this));
			Reopen();
			Foreach(GetType(), new _AnonymousInnerClass69(this));
			Db().Commit();
			Reopen();
			Db4objects.Db4o.IObjectSet os = NewQuery(GetType()).Execute();
			while (os.HasNext())
			{
				Db().Delete(os.Next());
			}
			Db4oUnit.Assert.AreEqual(1, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom)
				));
		}

		private sealed class _AnonymousInnerClass52 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass52(CascadeToArray _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Tests.Common.Querying.CascadeToArray cta = (Db4objects.Db4o.Tests.Common.Querying.CascadeToArray
					)obj;
				for (int i = 0; i < cta.objects.Length; i++)
				{
					Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom atom = (Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom
						)cta.objects[i];
					atom.name = "updated";
					if (atom.child != null)
					{
						atom.child.name = "updated";
					}
				}
				this._enclosing.Db().Set(cta);
			}

			private readonly CascadeToArray _enclosing;
		}

		private sealed class _AnonymousInnerClass69 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass69(CascadeToArray _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Tests.Common.Querying.CascadeToArray cta = (Db4objects.Db4o.Tests.Common.Querying.CascadeToArray
					)obj;
				for (int i = 0; i < cta.objects.Length; i++)
				{
					Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom atom = (Db4objects.Db4o.Tests.Common.Querying.CascadeToArray.Atom
						)cta.objects[i];
					Db4oUnit.Assert.AreEqual("updated", atom.name);
					if (atom.child != null)
					{
						Db4oUnit.Assert.AreNotEqual("updated", atom.child.name);
					}
				}
			}

			private readonly CascadeToArray _enclosing;
		}
	}
}
