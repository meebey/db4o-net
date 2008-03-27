/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Reflection;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;

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

		protected override IEnumerator FromClass(Type clazz)
		{
			return (IEnumerator)AbstractDb4oTestCase.FixtureVariable.With(_fixture, new _IClosure4_38
				(this, clazz));
		}

		private sealed class _IClosure4_38 : IClosure4
		{
			public _IClosure4_38(Db4oTestSuiteBuilder _enclosing, Type clazz)
			{
				this._enclosing = _enclosing;
				this.clazz = clazz;
			}

			public object Run()
			{
				return this._enclosing.BaseFromClass(clazz);
			}

			private readonly Db4oTestSuiteBuilder _enclosing;

			private readonly Type clazz;
		}

		protected override ITest FromMethod(Type clazz, MethodInfo method)
		{
			ITest test = base.FromMethod(clazz, method);
			if (typeof(AbstractDb4oTestCase).IsAssignableFrom(clazz))
			{
				return new FixtureDecoration(test, null, AbstractDb4oTestCase.FixtureVariable, _fixture
					);
			}
			return test;
		}

		private IEnumerator BaseFromClass(Type clazz)
		{
			return base.FromClass(clazz);
		}
	}
}
