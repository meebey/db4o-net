namespace Db4objects.Db4o.Internal.CS
{
	internal class BlobProcessor : Sharpen.Lang.Thread
	{
		private Db4objects.Db4o.Internal.CS.ClientObjectContainer stream;

		private Db4objects.Db4o.Foundation.Queue4 queue = new Db4objects.Db4o.Foundation.Queue4
			();

		private bool terminated = false;

		internal BlobProcessor(Db4objects.Db4o.Internal.CS.ClientObjectContainer aStream)
		{
			stream = aStream;
			SetPriority(MIN_PRIORITY);
		}

		internal virtual void Add(Db4objects.Db4o.Internal.CS.Messages.MsgBlob msg)
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
				Db4objects.Db4o.Foundation.Network.ISocket4 socket = stream.CreateParalellSocket(
					);
				Db4objects.Db4o.Internal.CS.Messages.MsgBlob msg = null;
				lock (queue)
				{
					msg = (Db4objects.Db4o.Internal.CS.Messages.MsgBlob)queue.Next();
				}
				while (msg != null)
				{
					msg.Write(stream, socket);
					msg.ProcessClient(socket);
					lock (stream.blobLock)
					{
						lock (queue)
						{
							msg = (Db4objects.Db4o.Internal.CS.Messages.MsgBlob)queue.Next();
						}
						if (msg == null)
						{
							terminated = true;
							Db4objects.Db4o.Internal.CS.Messages.Msg.CLOSE.Write(stream, socket);
							try
							{
								socket.Close();
							}
							catch
							{
							}
						}
					}
				}
			}
			catch (System.Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}
	}
}
