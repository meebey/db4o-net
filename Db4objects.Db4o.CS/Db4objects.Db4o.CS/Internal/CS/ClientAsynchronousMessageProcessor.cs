/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientAsynchronousMessageProcessor : Thread
	{
		private readonly BlockingQueue _messageQueue;

		private bool _stopped;

		public ClientAsynchronousMessageProcessor(BlockingQueue messageQueue)
		{
			_messageQueue = messageQueue;
		}

		public override void Run()
		{
			while (!_stopped)
			{
				try
				{
					IClientSideMessage message = (IClientSideMessage)_messageQueue.Next();
					if (message != null)
					{
						message.ProcessAtClient();
					}
				}
				catch (BlockingQueueStoppedException)
				{
					break;
				}
			}
		}

		public virtual void StartProcessing()
		{
			Start();
		}

		public virtual void StopProcessing()
		{
			_stopped = true;
		}
	}
}
