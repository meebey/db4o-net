namespace Db4oUnit.Extensions.Fixtures
{
	public class GlobalConfigurationSource : Db4oUnit.Extensions.Fixtures.IConfigurationSource
	{
		private readonly Db4objects.Db4o.Config.IConfiguration _config = Db4objects.Db4o.Db4oFactory
			.NewConfiguration();

		public virtual Db4objects.Db4o.Config.IConfiguration Config()
		{
			return _config;
		}
	}
}
