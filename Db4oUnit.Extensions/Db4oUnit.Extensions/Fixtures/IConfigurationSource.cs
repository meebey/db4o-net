using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions.Fixtures
{
	public interface IConfigurationSource
	{
		IConfiguration Config();
	}
}
