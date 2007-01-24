namespace Db4oUnit.Extensions.Fixtures
{
	public class IndependentConfigurationSource : Db4oUnit.Extensions.Fixtures.IConfigurationSource
	{
		public virtual Db4objects.Db4o.Config.IConfiguration Config()
		{
			return Db4objects.Db4o.Db4oFactory.NewConfiguration();
		}
	}
}
