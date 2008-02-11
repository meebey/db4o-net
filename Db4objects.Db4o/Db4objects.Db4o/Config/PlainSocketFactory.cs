/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Config;
using Sharpen.Net;

namespace Db4objects.Db4o.Config
{
	/// <summary>Create raw platform native sockets.</summary>
	/// <remarks>Create raw platform native sockets.</remarks>
	public class PlainSocketFactory : INativeSocketFactory
	{
		/// <exception cref="IOException"></exception>
		public virtual ServerSocket CreateServerSocket(int port)
		{
			return new ServerSocket(port);
		}

		/// <exception cref="IOException"></exception>
		public virtual Sharpen.Net.Socket CreateSocket(string hostName, int port)
		{
			return new Sharpen.Net.Socket(hostName, port);
		}

		public virtual object DeepClone(object context)
		{
			return this;
		}
	}
}
