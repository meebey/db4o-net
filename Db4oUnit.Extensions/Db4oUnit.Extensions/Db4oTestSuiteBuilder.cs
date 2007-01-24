namespace Db4oUnit.Extensions
{
	public class Db4oTestSuiteBuilder : Db4oUnit.ReflectionTestSuiteBuilder
	{
		private sealed class Db4oLabelProvider : Db4oUnit.TestMethod.ILabelProvider
		{
			public static readonly Db4oUnit.TestMethod.ILabelProvider DEFAULT = new Db4oUnit.Extensions.Db4oTestSuiteBuilder.Db4oLabelProvider
				();

			public string GetLabel(Db4oUnit.TestMethod method)
			{
				return "[" + FixtureLabel(method) + "] " + Db4oUnit.TestMethod.DEFAULT_LABEL_PROVIDER
					.GetLabel(method);
			}

			private string FixtureLabel(Db4oUnit.TestMethod method)
			{
				return ((Db4oUnit.Extensions.AbstractDb4oTestCase)method.GetSubject()).Fixture().
					GetLabel();
			}
		}

		private Db4oUnit.Extensions.IDb4oFixture _fixture;

		public Db4oTestSuiteBuilder(Db4oUnit.Extensions.IDb4oFixture fixture, System.Type
			 clazz) : base(clazz)
		{
			SetFixture(fixture);
		}

		public Db4oTestSuiteBuilder(Db4oUnit.Extensions.IDb4oFixture fixture, System.Type[]
			 classes) : base(classes)
		{
			SetFixture(fixture);
		}

		private void SetFixture(Db4oUnit.Extensions.IDb4oFixture fixture)
		{
			if (null == fixture)
			{
				throw new System.ArgumentNullException("fixture");
			}
			_fixture = fixture;
		}

		protected override bool IsApplicable(System.Type clazz)
		{
			return _fixture.Accept(clazz);
		}

		protected override object NewInstance(System.Type clazz)
		{
			object instance = base.NewInstance(clazz);
			if (instance is Db4oUnit.Extensions.AbstractDb4oTestCase)
			{
				((Db4oUnit.Extensions.AbstractDb4oTestCase)instance).Fixture(_fixture);
			}
			return instance;
		}

		protected override Db4oUnit.ITest CreateTest(object instance, System.Reflection.MethodInfo
			 method)
		{
			if (instance is Db4oUnit.Extensions.AbstractDb4oTestCase)
			{
				return new Db4oUnit.TestMethod(instance, method, Db4oUnit.Extensions.Db4oTestSuiteBuilder.Db4oLabelProvider
					.DEFAULT);
			}
			return base.CreateTest(instance, method);
		}
	}
}
