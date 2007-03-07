namespace Db4objects.Db4o.Tests.Common.Header
{
	public class ConfigurationSettingsTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public virtual void TestChangingUuidSettings()
		{
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Db4oUnit.Assert.AreEqual(Db4objects.Db4o.Config.ConfigScope.GLOBALLY, GenerateUUIDs
				());
			Db().Configure().GenerateUUIDs(-1);
			Db4oUnit.Assert.AreEqual(Db4objects.Db4o.Config.ConfigScope.DISABLED, GenerateUUIDs
				());
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Db4oUnit.Assert.AreEqual(Db4objects.Db4o.Config.ConfigScope.GLOBALLY, GenerateUUIDs
				());
		}

		private Db4objects.Db4o.Config.ConfigScope GenerateUUIDs()
		{
			return ((Db4objects.Db4o.Internal.LocalObjectContainer)Db()).Config().GenerateUUIDs
				();
		}
	}
}
