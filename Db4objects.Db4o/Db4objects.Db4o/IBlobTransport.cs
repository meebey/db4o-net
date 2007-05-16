/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface IBlobTransport
	{
		void WriteBlobTo(Transaction trans, BlobImpl blob, Sharpen.IO.File file);

		void ReadBlobFrom(Transaction trans, BlobImpl blob, Sharpen.IO.File file);
	}
}
