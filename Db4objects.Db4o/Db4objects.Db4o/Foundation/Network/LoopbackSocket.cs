/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Foundation.Network
{
	/// <summary>Fakes a socket connection for an embedded client.</summary>
	/// <remarks>Fakes a socket connection for an embedded client.</remarks>
	public class LoopbackSocket : ISocket4
	{
		private readonly ILoopbackSocketServer _server;

		private Db4objects.Db4o.Foundation.Network.LoopbackSocket _affiliate;

		private BlockingByteChannel _uploadBuffer;

		private BlockingByteChannel _downloadBuffer;

		public LoopbackSocket(ILoopbackSocketServer a_server, int timeout)
		{
			_server = a_server;
			_uploadBuffer = new BlockingByteChannel(timeout);
			_downloadBuffer = new BlockingByteChannel(timeout);
		}

		public LoopbackSocket(ILoopbackSocketServer a_server, int timeout, Db4objects.Db4o.Foundation.Network.LoopbackSocket
			 affiliate) : this(a_server, timeout)
		{
			_affiliate = affiliate;
			affiliate._affiliate = this;
			_downloadBuffer = affiliate._uploadBuffer;
			_uploadBuffer = affiliate._downloadBuffer;
		}

		public virtual void Close()
		{
			CloseAffiliate();
			CloseSocket();
		}

		private void CloseAffiliate()
		{
			if (_affiliate != null)
			{
				Db4objects.Db4o.Foundation.Network.LoopbackSocket temp = _affiliate;
				_affiliate = null;
				temp.Close();
			}
		}

		private void CloseSocket()
		{
			_downloadBuffer.Close();
			_uploadBuffer.Close();
		}

		public virtual void Flush()
		{
		}

		public virtual bool IsConnected()
		{
			return _affiliate != null;
		}

		public virtual int Read()
		{
			return _downloadBuffer.Read();
		}

		public virtual int Read(byte[] a_bytes, int a_offset, int a_length)
		{
			return _downloadBuffer.Read(a_bytes, a_offset, a_length);
		}

		public virtual void SetSoTimeout(int a_timeout)
		{
			_uploadBuffer.SetTimeout(a_timeout);
			_downloadBuffer.SetTimeout(a_timeout);
		}

		public virtual void Write(byte[] bytes)
		{
			_uploadBuffer.Write(bytes);
		}

		public virtual void Write(byte[] bytes, int off, int len)
		{
			_uploadBuffer.Write(bytes, off, len);
		}

		public virtual void Write(int i)
		{
			_uploadBuffer.Write(i);
		}

		public virtual ISocket4 OpenParalellSocket()
		{
			return _server.OpenClientSocket();
		}
	}
}
