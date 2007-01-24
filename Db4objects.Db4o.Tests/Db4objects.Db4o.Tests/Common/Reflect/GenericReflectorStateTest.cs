namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class GenericReflectorStateTest : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		protected override void Store()
		{
		}

		public virtual void TestKnownClasses()
		{
			Db().Reflector().KnownClasses();
		}
	}
}
