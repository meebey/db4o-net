using System;
using System.Reflection;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4oUnit.Extensions
{
	public class Db4oTestSuiteBuilder : ReflectionTestSuiteBuilder
	{
		private sealed class Db4oLabelProvider : TestMethod.ILabelProvider
		{
			public static readonly TestMethod.ILabelProvider DEFAULT = new Db4oTestSuiteBuilder.Db4oLabelProvider
				();

			public string GetLabel(TestMethod method)
			{
				return "[" + FixtureLabel(method) + "] " + TestMethod.DEFAULT_LABEL_PROVIDER.GetLabel
					(method);
			}

			private string FixtureLabel(TestMethod method)
			{
				return ((AbstractDb4oTestCase)method.GetSubject()).Fixture().GetLabel();
			}
		}

		private IDb4oFixture _fixture;

		public Db4oTestSuiteBuilder(IDb4oFixture fixture, Type clazz) : base(clazz)
		{
			SetFixture(fixture);
		}

		public Db4oTestSuiteBuilder(IDb4oFixture fixture, Type[] classes) : base(classes)
		{
			SetFixture(fixture);
		}

		private void SetFixture(IDb4oFixture fixture)
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
				return new TestMethod(instance, method, Db4oTestSuiteBuilder.Db4oLabelProvider.DEFAULT
					);
			}
			return base.CreateTest(instance, method);
		}
	}
}
