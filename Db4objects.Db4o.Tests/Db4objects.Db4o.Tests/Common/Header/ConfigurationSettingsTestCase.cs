namespace Db4objects.Db4o.Tests.Common.Header
{
	public class ConfigurationSettingsTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public virtual void TestChangingUuidSettings()
		{
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Db4oUnit.Assert.AreEqual(0, GenerateUUIDs());
			Db().Configure().GenerateUUIDs(-1);
			Db4oUnit.Assert.AreEqual(-1, GenerateUUIDs());
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Db4oUnit.Assert.AreEqual(0, GenerateUUIDs());
		}

		private int GenerateUUIDs()
		{
			return ((Db4objects.Db4o.Internal.LocalObjectContainer)Db()).Config().GenerateUUIDs
				();
		}
	}
}
