/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeOnUpdate : AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new CascadeOnUpdate().RunClientServer();
		}

		public class Atom
		{
			public CascadeOnUpdate.Atom child;

			public string name;

			public Atom()
			{
			}

			public Atom(CascadeOnUpdate.Atom child)
			{
				this.child = child;
			}

			public Atom(string name)
			{
				this.name = name;
			}

			public Atom(CascadeOnUpdate.Atom child, string name) : this(child)
			{
				this.name = name;
			}
		}

		public object child;

		protected override void Configure(IConfiguration conf)
		{
			conf.ObjectClass(this).CascadeOnUpdate(true);
		}

		protected override void Store()
		{
			CascadeOnUpdate cou = new CascadeOnUpdate();
			cou.child = new CascadeOnUpdate.Atom(new CascadeOnUpdate.Atom("storedChild"), "stored"
				);
			Db().Store(cou);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			Foreach(GetType(), new _IVisitor4_52(this));
			Reopen();
			Foreach(GetType(), new _IVisitor4_63());
		}

		private sealed class _IVisitor4_52 : IVisitor4
		{
			public _IVisitor4_52(CascadeOnUpdate _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				CascadeOnUpdate cou = (CascadeOnUpdate)obj;
				((CascadeOnUpdate.Atom)cou.child).name = "updated";
				((CascadeOnUpdate.Atom)cou.child).child.name = "updated";
				this._enclosing.Db().Store(cou);
			}

			private readonly CascadeOnUpdate _enclosing;
		}

		private sealed class _IVisitor4_63 : IVisitor4
		{
			public _IVisitor4_63()
			{
			}

			public void Visit(object obj)
			{
				CascadeOnUpdate cou = (CascadeOnUpdate)obj;
				CascadeOnUpdate.Atom atom = (CascadeOnUpdate.Atom)cou.child;
				Assert.AreEqual("updated", atom.name);
				Assert.AreNotEqual("updated", atom.child.name);
			}
		}
	}
}
