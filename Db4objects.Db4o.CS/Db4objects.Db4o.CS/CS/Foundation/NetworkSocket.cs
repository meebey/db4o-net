/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Foundation;

namespace Db4objects.Db4o.CS.Foundation
{
	public class NetworkSocket : NetworkSocketBase
	{
		private Sharpen.Net.Socket _socket;

		/// <exception cref="System.IO.IOException"></exception>
		public NetworkSocket(string hostName, int port) : base(hostName)
		{
			_socket = new Sharpen.Net.Socket(hostName, port);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public NetworkSocket(Sharpen.Net.Socket socket)
		{
			_socket = socket;
		}

		protected override Sharpen.Net.Socket Socket()
		{
			return _socket;
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected override ISocket4 CreateParallelSocket(string hostName, int port)
		{
			return new Db4objects.Db4o.CS.Foundation.NetworkSocket(hostName, port);
		}
	}
}
