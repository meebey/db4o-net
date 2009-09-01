/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.IO;
using System.Net.Sockets;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Sharpen.IO;

namespace Db4objects.Db4o.Foundation.Network
{
	public class NetworkSocket : ISocket4
	{
		private Sharpen.Net.Socket _socket;

		private IOutputStream _out;

		private IInputStream _in;

		private string _hostName;

		/// <exception cref="System.IO.IOException"></exception>
		public NetworkSocket(string hostName, int port)
		{
			Sharpen.Net.Socket socket = CreateSocket(hostName, port);
			InitSocket(socket);
			_hostName = hostName;
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected virtual Sharpen.Net.Socket CreateSocket(string hostName, int port)
		{
			return new Sharpen.Net.Socket(hostName, port);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public NetworkSocket(Sharpen.Net.Socket socket)
		{
			InitSocket(socket);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void InitSocket(Sharpen.Net.Socket socket)
		{
			_socket = socket;
			_out = _socket.GetOutputStream();
			_in = _socket.GetInputStream();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			_socket.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Flush()
		{
			_out.Flush();
		}

		public virtual bool IsConnected()
		{
			return Platform4.IsConnected(_socket);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual int Read()
		{
			int ret = _in.Read();
			CheckEOF(ret);
			return ret;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual int Read(byte[] a_bytes, int a_offset, int a_length)
		{
			int ret = _in.Read(a_bytes, a_offset, a_length);
			CheckEOF(ret);
			return ret;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void CheckEOF(int ret)
		{
			if (ret == -1)
			{
				throw new IOException();
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

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Write(byte[] bytes, int off, int len)
		{
			_out.Write(bytes, off, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Write(int i)
		{
			_out.Write(i);
		}

		/// <exception cref="System.IO.IOException"></exception>
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
