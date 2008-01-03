/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			SetPriority(MinPriority);
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
					msg.Write(socket);
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
							Msg.CloseSocket.Write(socket);
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
