namespace Db4oUnit.Extensions
{
	public class Db4oTestSuiteBuilder : Db4oUnit.ReflectionTestSuiteBuilder
	{
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

		protected override object NewInstance(System.Type clazz)
		{
			object instance = base.NewInstance(clazz);
			if (instance is Db4oUnit.Extensions.AbstractDb4oTestCase)
			{
				((Db4oUnit.Extensions.AbstractDb4oTestCase)instance).Fixture(_fixture);
			}
			return instance;
		}
	}
}
