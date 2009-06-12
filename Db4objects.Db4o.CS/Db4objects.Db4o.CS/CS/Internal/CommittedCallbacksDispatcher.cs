/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.CS.Internal
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
				MCommittedInfo committedInfos;
				try
				{
					committedInfos = (MCommittedInfo)_committedInfosQueue.Next();
				}
				catch (BlockingQueueStoppedException)
				{
					break;
				}
				_server.BroadcastMsg(committedInfos, new _IBroadcastFilter_28());
			}
		}

		private sealed class _IBroadcastFilter_28 : IBroadcastFilter
		{
			public _IBroadcastFilter_28()
			{
			}

			public bool Accept(IServerMessageDispatcher dispatcher)
			{
				return dispatcher.CaresAboutCommitted();
			}
		}

		public virtual void Stop()
		{
			_committedInfosQueue.Stop();
			_stopped = true;
		}
	}
}
