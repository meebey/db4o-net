/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Foundation.Network
{
	public interface ISocket4
	{
		/// <exception cref="Db4oIOException"></exception>
		void Close();

		/// <exception cref="Db4oIOException"></exception>
		void Flush();

		bool IsConnected();

		/// <exception cref="Db4oIOException"></exception>
		int Read();

		/// <exception cref="Db4oIOException"></exception>
		int Read(byte[] a_bytes, int a_offset, int a_length);

		void SetSoTimeout(int timeout);

		/// <exception cref="Db4oIOException"></exception>
		void Write(byte[] bytes);

		/// <exception cref="Db4oIOException"></exception>
		void Write(byte[] bytes, int off, int len);

		/// <exception cref="Db4oIOException"></exception>
		void Write(int i);

		/// <exception cref="Db4oIOException"></exception>
		ISocket4 OpenParalellSocket();
	}
}
