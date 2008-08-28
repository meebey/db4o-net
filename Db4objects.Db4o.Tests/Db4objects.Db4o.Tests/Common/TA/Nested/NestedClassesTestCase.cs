/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA.Nested;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.TA.Nested
{
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class NestedClassesTestCase : AbstractDb4oTestCase, IOptOutTA
	{
		public static void Main(string[] args)
		{
			new NestedClassesTestCase().RunSolo();
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			OuterClass outerObject = new OuterClass();
			outerObject._foo = 42;
			IActivatable objOne = (IActivatable)outerObject.CreateInnerObject();
			Store(objOne);
			IActivatable objTwo = (IActivatable)outerObject.CreateInnerObject();
			Store(objTwo);
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			string property = Runtime.GetProperty("java.version");
			if (property != null && property.StartsWith("1.3"))
			{
				Sharpen.Runtime.Err.WriteLine("IGNORED: " + GetType() + " will fail when run against JDK1.3/JDK1.4"
					);
				return;
			}
			IObjectSet query = Db().Query(typeof(OuterClass.InnerClass));
			while (query.HasNext())
			{
				OuterClass.InnerClass innerObject = (OuterClass.InnerClass)query.Next();
				Assert.IsNull(innerObject.GetOuterObjectWithoutActivation());
				Assert.AreEqual(42, innerObject.GetOuterObject().Foo());
			}
		}
	}
}
