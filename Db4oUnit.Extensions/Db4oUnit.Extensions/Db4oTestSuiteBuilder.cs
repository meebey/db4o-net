/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4oUnit.Extensions
{
	public class Db4oTestSuiteBuilder : ReflectionTestSuiteBuilder
	{
		private IDb4oFixture _fixture;

		public Db4oTestSuiteBuilder(IDb4oFixture fixture, Type clazz) : this(fixture, new 
			Type[] { clazz })
		{
		}

		public Db4oTestSuiteBuilder(IDb4oFixture fixture, Type[] classes) : base(classes)
		{
			Fixture(fixture);
		}

		private void Fixture(IDb4oFixture fixture)
		{
			if (null == fixture)
			{
				throw new ArgumentNullException("fixture");
			}
			_fixture = fixture;
		}

		protected override bool IsApplicable(Type clazz)
		{
			return _fixture.Accept(clazz);
		}

		protected override object NewInstance(Type clazz)
		{
			object instance = base.NewInstance(clazz);
			if (instance is AbstractDb4oTestCase)
			{
				((AbstractDb4oTestCase)instance).Fixture(_fixture);
			}
			return instance;
		}

		protected override ITest CreateTest(object instance, MethodInfo method)
		{
			if (instance is AbstractDb4oTestCase)
			{
				return new TestMethod(instance, method, Db4oFixtureLabelProvider.Default);
			}
			return base.CreateTest(instance, method);
		}
	}
}
