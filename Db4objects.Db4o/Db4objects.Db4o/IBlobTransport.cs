/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	#if !SILVERLIGHT
	public interface IBlobTransport
	{
		/// <exception cref="System.IO.IOException"></exception>
		void WriteBlobTo(Transaction trans, BlobImpl blob);

		/// <exception cref="System.IO.IOException"></exception>
		void ReadBlobFrom(Transaction trans, BlobImpl blob);

		void DeleteBlobFile(Transaction trans, BlobImpl blob);
	}
	#endif // !SILVERLIGHT
}
