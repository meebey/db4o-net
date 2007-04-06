using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Foundation.Network
{
	public interface ILoopbackSocketServer
	{
		LoopbackSocket OpenClientSocket();
	}
}
