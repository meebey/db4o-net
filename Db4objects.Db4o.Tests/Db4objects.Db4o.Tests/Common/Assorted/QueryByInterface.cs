/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class QueryByInterface : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new QueryByInterface().RunSolo();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			QueryByInterface.Ferrari f430 = new QueryByInterface.Ferrari(this, "F430");
			QueryByInterface.Ferrari f450 = new QueryByInterface.Ferrari(this, "F450");
			Store(f430);
			Store(f450);
			QueryByInterface.Bmw serie5 = new QueryByInterface.Bmw(this, "Serie 5");
			QueryByInterface.Bmw serie7 = new QueryByInterface.Bmw(this, "Serie 7");
			Store(serie5);
			Store(serie7);
		}

		public virtual void Test()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(QueryByInterface.ICar));
			q.Descend("name").Constrain("F450");
			IObjectSet result = q.Execute();
			Assert.AreEqual(1, result.Count);
			QueryByInterface.Ferrari car = (QueryByInterface.Ferrari)result.Next();
			Assert.AreEqual("F450", car.name);
		}

		public interface ICar
		{
		}

		public class Ferrari : QueryByInterface.ICar
		{
			public string name;

			public Ferrari(QueryByInterface _enclosing, string n)
			{
				this._enclosing = _enclosing;
				this.name = n;
			}

			public override string ToString()
			{
				return "Ferrari " + this.name;
			}

			private readonly QueryByInterface _enclosing;
		}

		public class Bmw : QueryByInterface.ICar
		{
			public string name;

			public Bmw(QueryByInterface _enclosing, string n)
			{
				this._enclosing = _enclosing;
				this.name = n;
			}

			public override string ToString()
			{
				return "BMW " + this.name;
			}

			private readonly QueryByInterface _enclosing;
		}
	}
}
