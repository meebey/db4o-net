/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Constraints;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Constraints;

namespace Db4objects.Db4o.Tests.Common.Constraints
{
	public class UniqueFieldIndexTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new UniqueFieldIndexTestCase().RunAll();
		}

		public class Item
		{
			public string _str;

			public Item()
			{
			}

			public Item(string str)
			{
				_str = str;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(UniqueFieldIndexTestCase.Item)).ObjectField("_str").Indexed
				(true);
			config.Add(new UniqueFieldValueConstraint(typeof(UniqueFieldIndexTestCase.Item), 
				"_str"));
		}

		protected override void Store()
		{
			Store(new UniqueFieldIndexTestCase.Item("1"));
			Store(new UniqueFieldIndexTestCase.Item("2"));
			Store(new UniqueFieldIndexTestCase.Item("3"));
		}

		public virtual void TestNewViolates()
		{
			Store(new UniqueFieldIndexTestCase.Item("2"));
			Assert.Expect(typeof(UniqueFieldValueConstraintViolationException), new _AnonymousInnerClass46
				(this));
			Db().Rollback();
		}

		private sealed class _AnonymousInnerClass46 : ICodeBlock
		{
			public _AnonymousInnerClass46(UniqueFieldIndexTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly UniqueFieldIndexTestCase _enclosing;
		}

		public virtual void TestUpdateViolates()
		{
			IQuery q = NewQuery(typeof(UniqueFieldIndexTestCase.Item));
			q.Descend("_str").Constrain("2");
			UniqueFieldIndexTestCase.Item item = (UniqueFieldIndexTestCase.Item)q.Execute().Next
				();
			item._str = "3";
			Store(item);
			Assert.Expect(typeof(UniqueFieldValueConstraintViolationException), new _AnonymousInnerClass60
				(this));
			Db().Rollback();
		}

		private sealed class _AnonymousInnerClass60 : ICodeBlock
		{
			public _AnonymousInnerClass60(UniqueFieldIndexTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly UniqueFieldIndexTestCase _enclosing;
		}

		public virtual void TestUpdateDoesNotViolate()
		{
			IQuery q = NewQuery(typeof(UniqueFieldIndexTestCase.Item));
			q.Descend("_str").Constrain("2");
			UniqueFieldIndexTestCase.Item item = (UniqueFieldIndexTestCase.Item)q.Execute().Next
				();
			item._str = "4";
			Store(item);
			Db().Commit();
		}
	}
}
