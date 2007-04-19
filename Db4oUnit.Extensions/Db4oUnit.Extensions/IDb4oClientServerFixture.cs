using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions
{
	public interface IDb4oClientServerFixture : IDb4oFixture
	{
		IObjectServer Server();

		IExtObjectContainer OpenNewClient();
	}
}
