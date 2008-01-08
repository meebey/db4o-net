/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA.Nested;

namespace Db4objects.Db4o.Tests.Common.TA.Nested
{
	/// <summary>
	/// TODO: This test case will fail when run against JDK1.3/JDK1.4 (though it will run green against
	/// JDK1.2 and JDK1.5+) because the synthetic "this$0" field is final.
	/// </summary>
	/// <remarks>
	/// TODO: This test case will fail when run against JDK1.3/JDK1.4 (though it will run green against
	/// JDK1.2 and JDK1.5+) because the synthetic "this$0" field is final.
	/// See http://developer.db4o.com/Resources/view.aspx/Reference/Implementation_Strategies/Type_Handling/Final_Fields/Final_Fields_Specifics
	/// </remarks>
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
