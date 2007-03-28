namespace Db4objects.Db4o.Internal.CS
{
	internal class ClientMessageDispatcherImpl : Sharpen.Lang.Thread, Db4objects.Db4o.Internal.CS.IClientMessageDispatcher
	{
		private Db4objects.Db4o.Internal.CS.ClientObjectContainer i_stream;

		private Db4objects.Db4o.Foundation.Network.ISocket4 i_socket;

		internal readonly Db4objects.Db4o.Foundation.Queue4 messageQueue;

		internal readonly Db4objects.Db4o.Foundation.Lock4 messageQueueLock;

		internal ClientMessageDispatcherImpl(Db4objects.Db4o.Internal.CS.ClientObjectContainer
			 client, Db4objects.Db4o.Foundation.Network.ISocket4 a_socket, Db4objects.Db4o.Foundation.Queue4
			 messageQueue_, Db4objects.Db4o.Foundation.Lock4 messageQueueLock_)
		{
			i_stream = client;
			messageQueue = messageQueue_;
			i_socket = a_socket;
			messageQueueLock = messageQueueLock_;
		}

		public virtual bool IsMessageDispatcherAlive()
		{
			lock (this)
			{
				return i_socket != null;
			}
		}

		public virtual bool Close()
		{
			lock (this)
			{
				i_stream = null;
				i_socket = null;
				return true;
			}
		}

		public override void Run()
		{
			while (IsMessageDispatcherAlive())
			{
				try
				{
					Db4objects.Db4o.Internal.CS.Messages.Msg message = null;
					try
					{
						message = Db4objects.Db4o.Internal.CS.Messages.Msg.ReadMessage(this, i_stream.GetTransaction
							(), i_socket);
						if (message is Db4objects.Db4o.Internal.CS.Messages.IClientSideMessage)
						{
							if (((Db4objects.Db4o.Internal.CS.Messages.IClientSideMessage)message).ProcessAtClient
								())
							{
								continue;
							}
						}
					}
					catch (System.Exception)
					{
						message = Db4objects.Db4o.Internal.CS.Messages.Msg.ERROR;
						Close();
					}
					Db4objects.Db4o.Internal.CS.Messages.Msg msgToBeAdded = message;
					messageQueueLock.Run(new _AnonymousInnerClass49(this, msgToBeAdded));
				}
				catch (System.Exception)
				{
					Close();
				}
			}
		}

		private sealed class _AnonymousInnerClass49 : Db4objects.Db4o.Foundation.IClosure4
		{
			public _AnonymousInnerClass49(ClientMessageDispatcherImpl _enclosing, Db4objects.Db4o.Internal.CS.Messages.Msg
				 msgToBeAdded)
			{
				this._enclosing = _enclosing;
				this.msgToBeAdded = msgToBeAdded;
			}

			public object Run()
			{
				this._enclosing.messageQueue.Add(msgToBeAdded);
				this._enclosing.messageQueueLock.Awake();
				return null;
			}

			private readonly ClientMessageDispatcherImpl _enclosing;

			private readonly Db4objects.Db4o.Internal.CS.Messages.Msg msgToBeAdded;
		}

		public virtual void Write(Db4objects.Db4o.Internal.CS.Messages.Msg msg)
		{
			i_stream.Write(msg);
		}

		public virtual void SetDispatcherName(string name)
		{
			SetName("db4o message client for user " + name);
		}

		public virtual void StartDispatcher()
		{
			Start();
		}
	}
}
