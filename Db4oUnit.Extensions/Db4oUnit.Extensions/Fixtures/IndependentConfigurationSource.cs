using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions.Fixtures
{
	public class IndependentConfigurationSource : IConfigurationSource
	{
		public virtual IConfiguration Config()
		{
			return Db4oFactory.NewConfiguration();
		}
	}
}
