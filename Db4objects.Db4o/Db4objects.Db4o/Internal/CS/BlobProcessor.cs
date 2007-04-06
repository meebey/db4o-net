using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	internal class BlobProcessor : Thread
	{
		private ClientObjectContainer stream;

		private IQueue4 queue = new NonblockingQueue();

		private bool terminated = false;

		internal BlobProcessor(ClientObjectContainer aStream)
		{
			stream = aStream;
			SetPriority(MIN_PRIORITY);
		}

		internal virtual void Add(MsgBlob msg)
		{
			lock (queue)
			{
				queue.Add(msg);
			}
		}

		internal virtual bool IsTerminated()
		{
			lock (this)
			{
				return terminated;
			}
		}

		public override void Run()
		{
			try
			{
				ISocket4 socket = stream.CreateParalellSocket();
				MsgBlob msg = null;
				lock (queue)
				{
					msg = (MsgBlob)queue.Next();
				}
				while (msg != null)
				{
					msg.Write(stream, socket);
					msg.ProcessClient(socket);
					lock (stream.blobLock)
					{
						lock (queue)
						{
							msg = (MsgBlob)queue.Next();
						}
						if (msg == null)
						{
							terminated = true;
							Msg.CLOSE.Write(stream, socket);
							try
							{
								socket.Close();
							}
							catch (Exception)
							{
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}
	}
}
