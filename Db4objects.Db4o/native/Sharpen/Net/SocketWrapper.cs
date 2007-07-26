/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Net.Sockets;
using NativeSocket=System.Net.Sockets.Socket;

namespace Sharpen.Net
{
	public class SocketWrapper
	{
		protected NativeSocket _delegate;

	    private int _soTimeout = 0;

        public int SoTimeout
        {
            get { return _soTimeout; }
        }

		public NativeSocket UnderlyingSocket
		{
			get { return _delegate;  }
		}

	    protected virtual void Initialize(NativeSocket socket)
		{
			_delegate = socket;
		}

		public void SetSoTimeout(int timeout)
		{
			_soTimeout = timeout;
		}

		public void Close()
		{
			if (_delegate.Connected)
			{
				try
				{
					_delegate.Shutdown(SocketShutdown.Both);
				}
				catch (Exception)
				{	
				}
			}
			_delegate.Close();
		}

        public bool IsConnected() 
        {
            return _delegate.Connected;
        }
	}
}
