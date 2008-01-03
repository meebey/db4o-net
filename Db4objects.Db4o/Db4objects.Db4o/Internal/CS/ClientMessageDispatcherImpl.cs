/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	internal class ClientMessageDispatcherImpl : Thread, IClientMessageDispatcher
	{
		private ClientObjectContainer i_stream;

		private ISocket4 i_socket;

		private readonly BlockingQueue _messageQueue;

		private bool _isClosed;

		internal ClientMessageDispatcherImpl(ClientObjectContainer client, ISocket4 a_socket
			, BlockingQueue messageQueue_)
		{
			i_stream = client;
			_messageQueue = messageQueue_;
			i_socket = a_socket;
		}

		public virtual bool IsMessageDispatcherAlive()
		{
			lock (this)
			{
				return !_isClosed;
			}
		}

		public virtual bool Close()
		{
			lock (this)
			{
				if (_isClosed)
				{
					return true;
				}
				_isClosed = true;
				if (i_socket != null)
				{
					try
					{
						i_socket.Close();
					}
					catch (Db4oIOException)
					{
					}
				}
				_messageQueue.Stop();
				return true;
			}
		}

		public override void Run()
		{
			MessageLoop();
			Close();
		}

		public virtual void MessageLoop()
		{
			while (IsMessageDispatcherAlive())
			{
				Msg message = null;
				try
				{
					message = Msg.ReadMessage(this, Transaction(), i_socket);
				}
				catch (Db4oIOException exc)
				{
					if (DTrace.enabled)
					{
						DTrace.ClientMessageLoopException.Log(exc.ToString());
					}
					return;
				}
				if (message == null)
				{
					continue;
				}
				if (IsClientSideMessage(message))
				{
					if (((IClientSideMessage)message).ProcessAtClient())
					{
						continue;
					}
				}
				_messageQueue.Add(message);
			}
		}

		private bool IsClientSideMessage(Msg message)
		{
			return message is IClientSideMessage;
		}

		public virtual bool Write(Msg msg)
		{
			i_stream.Write(msg);
			return true;
		}

		public virtual void SetDispatcherName(string name)
		{
			SetName("db4o client side message dispather for " + name);
		}

		public virtual void StartDispatcher()
		{
			Start();
		}

		private Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return i_stream.Transaction();
		}
	}
}
