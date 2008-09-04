/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	/// <summary>Regression test case for COR-1117</summary>
	public class CallbackTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new CallbackTestCase().RunAll();
		}

		public virtual void TestPublicCallback()
		{
			RunTest(new CallbackTestCase.PublicCallback());
		}

		/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
		public virtual void TestPrivateCallback()
		{
			RunTest(new CallbackTestCase.PrivateCallback());
		}

		/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
		public virtual void TestPackageCallback()
		{
			RunTest(new CallbackTestCase.PackageCallback());
		}

		public virtual void TestInheritedPublicCallback()
		{
			RunTest(new CallbackTestCase.InheritedPublicCallback());
		}

		/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
		public virtual void TestInheritedPrivateCallback()
		{
			RunTest(new CallbackTestCase.InheritedPrivateCallback());
		}

		/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
		public virtual void TestInheritedPackageCallback()
		{
			RunTest(new CallbackTestCase.InheritedPackageCallback());
		}

		private void RunTest(CallbackTestCase.Item item)
		{
			Store(item);
			Db().Commit();
			Assert.IsTrue(item.IsStored());
			Assert.IsTrue(Db().Ext().IsStored(item));
		}

		public class Item
		{
			[System.NonSerialized]
			public IObjectContainer _objectContainer;

			public virtual bool IsStored()
			{
				return _objectContainer.Ext().IsStored(this);
			}
		}

		public class PackageCallback : CallbackTestCase.Item
		{
			internal virtual void ObjectOnNew(IObjectContainer container)
			{
				_objectContainer = container;
			}
		}

		public class InheritedPackageCallback : CallbackTestCase.PackageCallback
		{
		}

		public class PrivateCallback : CallbackTestCase.Item
		{
			private void ObjectOnNew(IObjectContainer container)
			{
				_objectContainer = container;
			}
		}

		public class InheritedPrivateCallback : CallbackTestCase.PrivateCallback
		{
		}

		public class PublicCallback : CallbackTestCase.Item
		{
			public virtual void ObjectOnNew(IObjectContainer container)
			{
				_objectContainer = container;
			}
		}

		public class InheritedPublicCallback : CallbackTestCase.PublicCallback
		{
		}
	}
}
