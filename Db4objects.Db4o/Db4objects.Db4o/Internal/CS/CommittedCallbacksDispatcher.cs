using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	public class CommittedCallbacksDispatcher : IRunnable
	{
		private bool _stopped;

		private readonly BlockingQueue _committedInfosQueue;

		private readonly ObjectServerImpl _server;

		public CommittedCallbacksDispatcher(ObjectServerImpl server, BlockingQueue committedInfosQueue
			)
		{
			_server = server;
			_committedInfosQueue = committedInfosQueue;
		}

		public virtual void Run()
		{
			while (!_stopped)
			{
				MCommittedInfo committedInfos = (MCommittedInfo)_committedInfosQueue.Next();
				_server.SendCommittedInfoMsg(committedInfos);
			}
		}

		public virtual void Stop()
		{
			_stopped = true;
		}
	}
}
