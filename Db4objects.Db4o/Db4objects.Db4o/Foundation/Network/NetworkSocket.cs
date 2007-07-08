/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using System.Net.Sockets;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Sharpen.IO;

namespace Db4objects.Db4o.Foundation.Network
{
	public class NetworkSocket : ISocket4
	{
		private Sharpen.Net.Socket _socket;

		private OutputStream _out;

		private InputStream _in;

		private string _hostName;

		public NetworkSocket(string hostName, int port)
		{
			try
			{
				Sharpen.Net.Socket socket = new Sharpen.Net.Socket(hostName, port);
				InitSocket(socket);
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
			_hostName = hostName;
		}

		public NetworkSocket(Sharpen.Net.Socket socket)
		{
			InitSocket(socket);
		}

		private void InitSocket(Sharpen.Net.Socket socket)
		{
			_socket = socket;
			_out = _socket.GetOutputStream();
			_in = _socket.GetInputStream();
		}

		public virtual void Close()
		{
			try
			{
				_socket.Close();
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual void Flush()
		{
			try
			{
				_out.Flush();
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual bool IsConnected()
		{
			return Platform4.IsConnected(_socket);
		}

		public virtual int Read()
		{
			try
			{
				return _in.Read();
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual int Read(byte[] a_bytes, int a_offset, int a_length)
		{
			try
			{
				return _in.Read(a_bytes, a_offset, a_length);
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual void SetSoTimeout(int timeout)
		{
			try
			{
				_socket.SetSoTimeout(timeout);
			}
			catch (SocketException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public virtual void Write(byte[] bytes)
		{
			try
			{
				_out.Write(bytes);
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual void Write(byte[] bytes, int off, int len)
		{
			try
			{
				_out.Write(bytes, off, len);
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual void Write(int i)
		{
			try
			{
				_out.Write(i);
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
		}

		public virtual ISocket4 OpenParalellSocket()
		{
			if (_hostName == null)
			{
				throw new InvalidOperationException();
			}
			return new Db4objects.Db4o.Foundation.Network.NetworkSocket(_hostName, _socket.GetPort
				());
		}
	}
}
