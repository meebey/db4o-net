/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Foundation.Network
{
	public interface IServerSocket4
	{
		void SetSoTimeout(int timeout);

		int GetLocalPort();

		/// <exception cref="System.IO.IOException"></exception>
		ISocket4 Accept();

		/// <exception cref="System.IO.IOException"></exception>
		void Close();
	}
}
