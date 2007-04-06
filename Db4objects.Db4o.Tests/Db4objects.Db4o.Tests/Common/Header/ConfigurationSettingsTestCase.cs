using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Header
{
	public class ConfigurationSettingsTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public virtual void TestChangingUuidSettings()
		{
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Assert.AreEqual(ConfigScope.GLOBALLY, GenerateUUIDs());
			Db().Configure().GenerateUUIDs(-1);
			Assert.AreEqual(ConfigScope.DISABLED, GenerateUUIDs());
			Fixture().Config().GenerateUUIDs(0);
			Reopen();
			Assert.AreEqual(ConfigScope.GLOBALLY, GenerateUUIDs());
		}

		private ConfigScope GenerateUUIDs()
		{
			return ((LocalObjectContainer)Db()).Config().GenerateUUIDs();
		}
	}
}
