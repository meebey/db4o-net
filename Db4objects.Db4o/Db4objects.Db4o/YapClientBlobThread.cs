namespace Db4objects.Db4o
{
	internal class YapClientBlobThread : Sharpen.Lang.Thread
	{
		private Db4objects.Db4o.YapClient stream;

		private Db4objects.Db4o.Foundation.Queue4 queue = new Db4objects.Db4o.Foundation.Queue4
			();

		private bool terminated = false;

		internal YapClientBlobThread(Db4objects.Db4o.YapClient aStream)
		{
			stream = aStream;
			SetPriority(MIN_PRIORITY);
		}

		internal virtual void Add(Db4objects.Db4o.MsgBlob msg)
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
				Db4objects.Db4o.Foundation.Network.IYapSocket socket = stream.CreateParalellSocket
					();
				Db4objects.Db4o.MsgBlob msg = null;
				lock (queue)
				{
					msg = (Db4objects.Db4o.MsgBlob)queue.Next();
				}
				while (msg != null)
				{
					msg.Write(stream, socket);
					msg.ProcessClient(socket);
					lock (stream.blobLock)
					{
						lock (queue)
						{
							msg = (Db4objects.Db4o.MsgBlob)queue.Next();
						}
						if (msg == null)
						{
							terminated = true;
							Db4objects.Db4o.Msg.CLOSE.Write(stream, socket);
							try
							{
								socket.Close();
							}
							catch (System.Exception e)
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