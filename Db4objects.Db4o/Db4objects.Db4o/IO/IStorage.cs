/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public interface IStorage
	{
		/// <exception cref="Db4oIOException"></exception>
		IBin Open(string uri, bool lockFile, long initialLength, bool readOnly);

		bool Exists(string uri);
	}
}
