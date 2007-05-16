/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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

		private OutputStream _out;

		private InputStream _in;

		private string _hostName;

		public NetworkSocket(string hostName, int port) : this(new Sharpen.Net.Socket(hostName
			, port))
		{
			_hostName = hostName;
		}

		public NetworkSocket(Sharpen.Net.Socket socket)
		{
			_socket = socket;
			_out = _socket.GetOutputStream();
			_in = _socket.GetInputStream();
		}

		public virtual void Close()
		{
			_socket.Close();
		}

		public virtual void Flush()
		{
			_out.Flush();
		}

		public virtual bool IsConnected()
		{
			return Platform4.IsConnected(_socket);
		}

		public virtual int Read()
		{
			return _in.Read();
		}

		public virtual int Read(byte[] a_bytes, int a_offset, int a_length)
		{
			return _in.Read(a_bytes, a_offset, a_length);
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
			_out.Write(bytes);
		}

		public virtual void Write(byte[] bytes, int off, int len)
		{
			_out.Write(bytes, off, len);
		}

		public virtual void Write(int i)
		{
			_out.Write(i);
		}

		public virtual ISocket4 OpenParalellSocket()
		{
			if (_hostName == null)
			{
				throw new IOException("Cannot open parallel socket - invalid state.");
			}
			return new Db4objects.Db4o.Foundation.Network.NetworkSocket(_hostName, _socket.GetPort
				());
		}
	}
}
