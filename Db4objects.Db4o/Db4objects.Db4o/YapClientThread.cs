namespace Db4objects.Db4o
{
	internal class YapClientThread : Sharpen.Lang.Thread
	{
		private Db4objects.Db4o.YapClient i_stream;

		private Db4objects.Db4o.Foundation.Network.IYapSocket i_socket;

		internal readonly Db4objects.Db4o.Foundation.Queue4 messageQueue;

		internal readonly Db4objects.Db4o.Foundation.Lock4 messageQueueLock;

		internal YapClientThread(Db4objects.Db4o.YapClient client, Db4objects.Db4o.Foundation.Network.IYapSocket
			 a_socket, Db4objects.Db4o.Foundation.Queue4 messageQueue_, Db4objects.Db4o.Foundation.Lock4
			 messageQueueLock_)
		{
			lock (this)
			{
				i_stream = client;
				messageQueue = messageQueue_;
				i_socket = a_socket;
				messageQueueLock = messageQueueLock_;
			}
		}

		internal virtual bool IsClosed()
		{
			lock (this)
			{
				return i_socket == null;
			}
		}

		internal virtual void Close()
		{
			lock (this)
			{
				i_stream = null;
				i_socket = null;
			}
		}

		public override void Run()
		{
			while (i_socket != null)
			{
				try
				{
					if (i_stream == null)
					{
						return;
					}
					Db4objects.Db4o.Msg message;
					try
					{
						message = Db4objects.Db4o.Msg.ReadMessage(i_stream.GetTransaction(), i_socket);
					}
					catch (System.Exception exc)
					{
						messageQueueLock.Run(new _AnonymousInnerClass46(this));
						Close();
						return;
					}
					if (i_stream == null)
					{
						return;
					}
					if (Db4objects.Db4o.Msg.PING.Equals(message))
					{
						i_stream.WriteMsg(Db4objects.Db4o.Msg.OK);
					}
					else
					{
						if (Db4objects.Db4o.Msg.CLOSE.Equals(message))
						{
							i_stream.LogMsg(35, i_stream.ToString());
							if (i_stream == null)
							{
								return;
							}
							i_stream = null;
							i_socket = null;
						}
						else
						{
							messageQueueLock.Run(new _AnonymousInnerClass82(this, message));
						}
					}
				}
				catch (System.Exception exc)
				{
					Close();
					return;
				}
			}
		}

		private sealed class _AnonymousInnerClass46 : Db4objects.Db4o.Foundation.IClosure4
		{
			public _AnonymousInnerClass46(YapClientThread _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				this._enclosing.messageQueue.Add(Db4objects.Db4o.Msg.ERROR);
				this._enclosing.Close();
				this._enclosing.messageQueueLock.Awake();
				return null;
			}

			private readonly YapClientThread _enclosing;
		}

		private sealed class _AnonymousInnerClass82 : Db4objects.Db4o.Foundation.IClosure4
		{
			public _AnonymousInnerClass82(YapClientThread _enclosing, Db4objects.Db4o.Msg message
				)
			{
				this._enclosing = _enclosing;
				this.message = message;
			}

			public object Run()
			{
				this._enclosing.messageQueue.Add(message);
				this._enclosing.messageQueueLock.Awake();
				return null;
			}

			private readonly YapClientThread _enclosing;

			private readonly Db4objects.Db4o.Msg message;
		}
	}
}
