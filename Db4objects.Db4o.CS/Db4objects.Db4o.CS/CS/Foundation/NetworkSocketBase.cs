/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using System.IO;
using System.Net.Sockets;
using Db4objects.Db4o.CS.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Foundation
{
	public abstract class NetworkSocketBase : ISocket4
	{
		private string _hostName;

		public NetworkSocketBase() : this(null)
		{
		}

		public NetworkSocketBase(string hostName)
		{
			_hostName = hostName;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			Socket().Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Flush()
		{
			Socket().GetOutputStream().Flush();
		}

		public virtual bool IsConnected()
		{
			return Platform4.IsConnected(Socket());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual int Read(byte[] a_bytes, int a_offset, int a_length)
		{
			int ret = Socket().GetInputStream().Read(a_bytes, a_offset, a_length);
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
				Socket().SetSoTimeout(timeout);
			}
			catch (SocketException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Write(byte[] bytes, int off, int len)
		{
			Socket().GetOutputStream().Write(bytes, off, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual ISocket4 OpenParallelSocket()
		{
			if (_hostName == null)
			{
				throw new InvalidOperationException();
			}
			return CreateParallelSocket(_hostName, Socket().GetPort());
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected abstract ISocket4 CreateParallelSocket(string hostName, int port);

		public override string ToString()
		{
			return Socket().ToString();
		}

		protected abstract Sharpen.Net.Socket Socket();
	}
}
