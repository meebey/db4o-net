namespace Db4objects.Db4o.Foundation.Network
{
	/// <summary>Fakes a socket connection for an embedded client.</summary>
	/// <remarks>Fakes a socket connection for an embedded client.</remarks>
	public class YapSocketFake : Db4objects.Db4o.Foundation.Network.IYapSocket
	{
		private readonly Db4objects.Db4o.Foundation.Network.IYapSocketFakeServer _server;

		private Db4objects.Db4o.Foundation.Network.YapSocketFake _affiliate;

		private Db4objects.Db4o.Foundation.Network.ByteBuffer4 _uploadBuffer;

		private Db4objects.Db4o.Foundation.Network.ByteBuffer4 _downloadBuffer;

		public YapSocketFake(Db4objects.Db4o.Foundation.Network.IYapSocketFakeServer a_server
			, int timeout)
		{
			_server = a_server;
			_uploadBuffer = new Db4objects.Db4o.Foundation.Network.ByteBuffer4(timeout);
			_downloadBuffer = new Db4objects.Db4o.Foundation.Network.ByteBuffer4(timeout);
		}

		public YapSocketFake(Db4objects.Db4o.Foundation.Network.IYapSocketFakeServer a_server
			, int timeout, Db4objects.Db4o.Foundation.Network.YapSocketFake affiliate) : this
			(a_server, timeout)
		{
			_affiliate = affiliate;
			affiliate._affiliate = this;
			_downloadBuffer = affiliate._uploadBuffer;
			_uploadBuffer = affiliate._downloadBuffer;
		}

		public virtual void Close()
		{
			if (_affiliate != null)
			{
				Db4objects.Db4o.Foundation.Network.YapSocketFake temp = _affiliate;
				_affiliate = null;
				temp.Close();
			}
			_affiliate = null;
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

		public virtual Db4objects.Db4o.Foundation.Network.IYapSocket OpenParalellSocket()
		{
			return _server.OpenClientSocket();
		}
	}
}
