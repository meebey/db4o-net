namespace Db4objects.Db4o.Internal
{
	using Db4objects.Db4o.Config;
	using Db4objects.Db4o.Internal.CS.Config;

	public partial class Config4Impl
	{
		private static IClientServerFactory DefaultClientServerFactory()
		{
			return new ClientServerFactoryImpl();
		}
	}
}
