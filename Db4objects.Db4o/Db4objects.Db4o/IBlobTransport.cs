/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface IBlobTransport
	{
		/// <exception cref="IOException"></exception>
		void WriteBlobTo(Transaction trans, BlobImpl blob, Sharpen.IO.File file);

		/// <exception cref="IOException"></exception>
		void ReadBlobFrom(Transaction trans, BlobImpl blob, Sharpen.IO.File file);

		void DeleteBlobFile(Transaction trans, BlobImpl blob);
	}
}
